using Business.enums;
using Business.Model;
using DataAccess.enums;
using System;

namespace Business {
    public interface IMainManager {

        #region ILoginRegister
        BaseModel Register(string email, string password);
        BaseModel ConfirmEmail(string email, string mailKey);
        BaseModel Login(string email, string password);
        #endregion

        #region IUserInfo
        BaseModel FillUserInfoKPS(string token, string name, string surname, string TCKN, string birthDate);
        //bool FillUserInfoEmail(string email);
        BaseModel FillUserInfoKPS2(string token, string city, string town, string job, Sex sex);
        #endregion

        #region ISurvey
        SurveyViewModel GetSurveys();
        SurveyViewModel GetSurveyOfUser(string email);
        WalletViewModel GetWalletOfUser(string email);

        #endregion

        #region ICityDistrictJob
        JobViewModel GetJobs();

        #endregion

        #region ICache
        void ClearCache(Constant constant);
        void ClearAllCache();

        #endregion

        string MockData();
    }
}