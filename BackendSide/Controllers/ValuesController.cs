using BackendSide.ExceptionHandling;
using BackendSide.RequestModel;
using Business;
using Business.Job;
using Business.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace BackendSide.Controllers {

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase {
        private readonly IMainManager mainManager;
        private readonly IScheduledJob scheduledJob;
        private readonly ILogger<ValuesController> logger;
        public ValuesController(IMainManager mainManager, IScheduledJob scheduledJob, ILogger<ValuesController> logger) {
            this.mainManager = mainManager;
            this.scheduledJob = scheduledJob;
            this.logger = logger;
        }

        [ExceptionHandler]
        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult<BaseModel> Register(LoginRequestModel model) {
            try {
                var result = mainManager.Register(model.email, model.password);
                return Ok(result);
            }
            catch(Exception ex) {
                logger.LogError(ex.Message);
                return BadRequest((BaseModel)ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("confirmMail")]
        public ActionResult<BaseModel> ConfirmMail(ConfirmMailRequestModel model) {
            var result = mainManager.ConfirmEmail(model.email, model.key);
            return result;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<BaseModel> Login(LoginRequestModel model) {
            var result = mainManager.Login(model.email, model.password);
            return result;
        }

        [HttpPost("userInfo1")]
        public ActionResult<BaseModel> UserInfo1(UserInfoRequestModel1 model) {
            var token = Request.Headers["Authorization"];
            var result = mainManager.FillUserInfoKPS(token, model.name, model.surname, model.tckn, model.birthDate);
            return result;
        }

        [HttpPost("userInfo2")]
        public ActionResult<BaseModel> UserInfo2(UserInfoRequestModel2 model) {
            var token = Request.Headers["Authorization"];
            var result = mainManager.FillUserInfoKPS2(token, model.city, model.town, model.job, model.sex);
            return result;
        }

        [HttpGet("surveys")]
        public ActionResult<SurveyViewModel> Surveys() {
            SurveyViewModel result = mainManager.GetSurveys();
            return result;
        }

        [HttpGet("userSurveys")]
        public ActionResult<SurveyViewModel> SurveysOfUser() {
            var token = Request.Headers["Authorization"];
            var result = mainManager.GetSurveyOfUser(token);
            return result;
        }

        [HttpPost("wallet")]
        public ActionResult<WalletViewModel> Wallet([FromBody]string email) {
            WalletViewModel result = mainManager.GetWalletOfUser(email);
            return result;
        }

        [AllowAnonymous]
        [HttpGet("cities")]
        public ActionResult<SurveyViewModel> GetCities() {
            return null;
        }

        [AllowAnonymous]
        [HttpGet("jobs")]
        public ActionResult<JobViewModel> GetJobs() {

            var result = mainManager.GetJobs();
            return result;
        }

        [AllowAnonymous]
        [HttpGet("clearCache")]
        public ActionResult<bool> ClearJobCache() {
            //cache.Remove(constantKeyJob);
            return true;
        }


        //Daha sonra kaldırılacak.
        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<string> Get() {
            return mainManager.MockData();
        }

    }
}
