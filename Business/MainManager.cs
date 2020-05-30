using Business.Authorization;
using Business.Crypto;
using Business.enums;
using Business.KPS;
using Business.Model;
using Business.Model.Other;
using DataAccess;
using DataAccess.Entity;
using DataAccess.enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Notification;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business {
    public class MainManager : IMainManager {
        private readonly AppDBContext context;
        private readonly IEncryptor encryptor;
        private readonly IKPSService kPSService;
        private readonly IMailSender mailSender;
        private readonly IMemoryCache cache;
        private readonly IAuthorization authenticator;

        public MainManager(AppDBContext context, IEncryptor encryptor, IKPSService kPSService, IMailSender mailSender, IMemoryCache cache, IAuthorization authenticator) {
            this.context = context;
            this.encryptor = encryptor;
            this.kPSService = kPSService;
            this.mailSender = mailSender;
            this.cache = cache;
            this.authenticator = authenticator;
        }

        #region LoginRegister
        private BaseModel Register(string email, string password) {
            var model = new BaseModel();
            try {
                var user = context.Login.AsNoTracking().FirstOrDefault(x => x.Email.Equals(email));
                if(user != null) {
                    model.isError = true;
                    model.errorDescription = "Kullanıcı zaten kayıtlı";
                    return model;
                }
                var cryptodPassword = encryptor.MD5Hash(password);
                var key = GenerateRandomKey();
                mailSender.SendMail(key, email);
                var login = new Login() {
                    Email = email,
                    Password = cryptodPassword,
                    MailKey = key,
                    IsActive = false
                };
                context.Login.Add(login);
                context.SaveChanges();
                return model;
            }
            catch(Exception ex) {
                model.isError = true;
                model.errorDescription = "Beklenmeyen hata";
                return model;
            }
        }

        private BaseModel ConfirmEmail(string email, string mailKey) {
            var model = new BaseModel();
            try {
                var user = context.Login.Single(x => x.Email.Equals(email));
                if(user.MailKey.Equals(mailKey)) {
                    user.IsActive = true;
                    context.SaveChanges();
                    var token = authenticator.Authorize(email, true);
                    model.token = token;
                    return model;
                }
                model.isError = true;
                model.errorDescription = "Mail anahtarı hatalı";
                return model;
            }
            catch(Exception ex) {
                model.isError = true;
                model.errorDescription = "Beklenmeyen hata";
                return model;
            }

        }

        private BaseModel Login(string email, string password) {
            var model = new BaseModel();
            try {
                var login = context.Login.Single(x => x.Email.Equals(email));
                var cryptodPassword = encryptor.MD5Hash(password);
                var isCorrectPassword = login.Password.Equals(cryptodPassword);
                var token = authenticator.Authorize(login.Email, login.IsActive);
                model.token = token;
                if(!isCorrectPassword || login == null) {
                    model.isError = true;
                    model.errorDescription = "Kullanıcı adı ve ya şifre yanlış";
                    return model;
                }
                var isUserActive = login.IsActive;
                if(!isUserActive) {
                    var key = GenerateRandomKey();
                    mailSender.SendMail(key, email);
                    login.MailKey = key;
                    model.isError = true;
                    model.errorName = "NotActive";
                    model.errorDescription = "Kullanıcı aktif değil";
                    return model;
                }
                if(string.IsNullOrEmpty(token)) {
                    model.isError = true;
                    model.errorDescription = "Oturum açılamadı";
                    return model;
                }
                context.SaveChanges();
                return model;
            }
            catch(Exception ex) {
                model.isError = true;
                model.errorDescription = "Beklenmeyen hata";
                return model;
            }
        }
        #endregion

        #region UserInfo
        private BaseModel FillUserInfoKPS(string token, string name, string surname, string tckn, string birthOfDate) {
            var model = new BaseModel();
            try {
                ulong TCKN = Convert.ToUInt64(tckn);
                DateTime birthDate = DateTime.ParseExact(birthOfDate, "MM-dd-yyyy", null);
                var jwtResult = authenticator.ConvertAndRefreshToken(token);
                var email = jwtResult.Email;
                var isActive = jwtResult.IsActive;
                var isValidKPS = kPSService.Validate(name, surname, TCKN, birthDate.Year);
                model.token = jwtResult.Token;
                if(!isValidKPS) {
                    model.isError = true;
                    model.errorDescription = "Kimlik bilgileri uyuşmazlığı";
                    return model;
                }
                var userInfo = context.UserInfo.AsNoTracking().FirstOrDefault(x => x.Tckn == TCKN);
                if(userInfo != null) {
                    model.isError = true;
                    model.errorDescription = "TC'si kayıtlı kullanıcı";
                    return model;
                }
                var userInfoToInsert = new UserInfo() {
                    Email = email,
                    Name = name,
                    Surname = surname,
                    Tckn = TCKN,
                    BirthDate = birthDate
                };
                context.UserInfo.Add(userInfoToInsert);
                context.SaveChanges();
                return model;
            }

            catch(Exception ex) {
                model.isError = true;
                model.errorDescription = "Beklenmeyen hata";
                return model;
            }
        }

        private BaseModel FillUserInfoKPS2(string token, string city, string town, string job, Sex sex) {
            var model = new BaseModel();
            try {
                var jwtResult = authenticator.ConvertAndRefreshToken(token);
                var email = jwtResult.Email;
                var isActive = jwtResult.IsActive;
                model.token = jwtResult.Token;
                var userInfo = context.UserInfo.FirstOrDefault(x => x.Email.Equals(email));
                if(userInfo == null) {
                    model.isError = true;
                    model.errorDescription = "Kullanıcı bulunamadı";
                    return model;
                }
                var userInfoToUpdate = userInfo;
                userInfoToUpdate.City = city;
                userInfoToUpdate.Town = town;
                userInfoToUpdate.Job = job;
                userInfoToUpdate.Sex = sex;
                //city,town seçmeli olacak
                context.Entry(userInfo).CurrentValues.SetValues(userInfoToUpdate);
                context.SaveChanges();
                return model;
            }
            catch(Exception ex) {
                model.isError = true;
                model.errorDescription = "Beklenmeyen hata";
                return model;
            }
        }
        #endregion

        #region Survey
        private SurveyViewModel GetSurveys() {
            var model = new SurveyViewModel();
            try {
                var surveys = context.Survey.Include(x => x.Questions).AsNoTracking().ToList();
                model.surveys = surveys;
            }
            catch(Exception ex) {
                //Mapper kullanırsın burada
                model.isError = true;
                model.errorDescription = "Anketler bulunamadı";

            }
            return model;
        }

        private SurveyViewModel GetSurveyOfUser(string token) {
            var jwtResult = authenticator.ConvertAndRefreshToken(token);
            var allSurveys = GetSurveys();
            var model = new SurveyViewModel();
            model.surveys = new List<Survey>();
            model.token = jwtResult.Token;
            if(allSurveys.isError) {
                return allSurveys;
            }
            var userInfo = context.UserInfo.AsNoTracking().SingleOrDefault(x => x.Email.Equals(jwtResult.Email));
            foreach(var item in allSurveys.surveys) {
                var jsonCondition = item.Condition;
                var condition = JsonConvert.DeserializeObject<Condition>(jsonCondition);
                var isValidSurvey = Helper.ValidSurveyForUser(userInfo, condition);
                if(isValidSurvey) {
                    model.surveys.Add(item);
                }
            }
            return model;
        }

        private WalletViewModel GetWalletOfUser(string token) {
            var jwtResult = authenticator.ConvertAndRefreshToken(token);
            var userCompletedSurveys = GetUserCompletedSurveys(jwtResult.Email);
            var model = new WalletViewModel();
            model.token = jwtResult.Token;
            if(userCompletedSurveys.isError) {
                //Mapper kullanırsın burada
                model.isError = true;
                model.errorDescription = userCompletedSurveys.errorDescription;
            }
            else {
                var totalPrice = userCompletedSurveys.surveys.Sum(x => x.Money);
                model.money = totalPrice;
            }
            return model;
        }

        private SurveyViewModel GetUserCompletedSurveys(string email) {
            var model = new SurveyViewModel();
            try {
                var completedSurveys = context.CompletedSurveys.AsNoTracking().Where(x => x.Email.Equals(email))
                    .Join(context.Survey, z => z.SurveyId, y => y.Id, (z, y) => new { z, y }).Select(x => x.y).ToList();
                model.surveys = completedSurveys;
            }
            catch(Exception ex) {
                //Mapper kullanırsın burada
                model.isError = true;
                model.errorDescription = ex.Message;
            }
            return model;
        }
        #endregion

        #region CityDistrictJob
        private JobViewModel GetJobs() {
            if(cache.TryGetValue(Constant.constantKeyJob, out JobViewModel list))
                return list;

            var model = new JobViewModel();
            try {
                var jobList = context.Job.AsNoTracking().ToList();
                model.Jobs = jobList;
            }
            catch(Exception ex) {
                model.isError = true;
                model.errorDescription = ex.Message;
            }

            if(!model.isError) {
                cache.Set(Constant.constantKeyJob, model, new MemoryCacheEntryOptions {
                    AbsoluteExpiration = DateTime.Now.AddDays(10),
                    Priority = CacheItemPriority.Normal
                });
            }

            return model;
        }
        #endregion

        #region Cache

        private void ClearCache(Constant constant) {
            cache.Remove(constant);
        }

        private void ClearAllCache() {
            var allConstants = Enum.GetValues(typeof(Constant));
            foreach(var constant in allConstants) {
                cache.Remove(constant);
            }
        }

        #endregion

        #region Other
        private string GenerateRandomKey() {
            Random random = new Random();
            int generatedNumber = random.Next(100000, 999999);
            var result = Convert.ToString(generatedNumber);
            return result;
        }
        
        public string MockData() {
            return "MockData";
        }
        #endregion


        #region ILoginRegister
        BaseModel IMainManager.Register(string email, string password) => Register(email, password);
        BaseModel IMainManager.ConfirmEmail(string email, string mailKey) => ConfirmEmail(email, mailKey);

        BaseModel IMainManager.Login(string email, string password) => Login(email, password);
        #endregion

        #region IUserInfo
        BaseModel IMainManager.FillUserInfoKPS(string token, string name, string surname, string TCKN, string birthDate) => FillUserInfoKPS(token, name, surname, TCKN, birthDate);
        //bool IMainManager.FillUserInfoEmail(string email) => FillUserInfoEmail(email);
        BaseModel IMainManager.FillUserInfoKPS2(string email, string city, string town, string job, Sex sex) => FillUserInfoKPS2(email, city, town, job, sex);
        #endregion

        #region ISurvey
        SurveyViewModel IMainManager.GetSurveys() => GetSurveys();
        SurveyViewModel IMainManager.GetSurveyOfUser(string token) => GetSurveyOfUser(token);
        WalletViewModel IMainManager.GetWalletOfUser(string token) => GetWalletOfUser(token);
        #endregion

        #region ICityDistrictJob
        JobViewModel IMainManager.GetJobs() => GetJobs();
        #endregion

        #region ICache
        void IMainManager.ClearCache(Constant constant) => ClearCache(constant);
        void IMainManager.ClearAllCache() => ClearAllCache();

        #endregion

    }
}
