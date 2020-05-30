using Business.Model.Other;
using DataAccess.Entity;
using System;
using System.Linq;

namespace Business {
    internal static class Helper {
        internal static bool ValidSurveyForUser(UserInfo userInfo, Condition condition) {
            bool result = true;
            var age = Helper.GetDifferenceAsYear(DateTime.Now, userInfo.BirthDate);
            var city = userInfo.City;
            var job = userInfo.Job;
            if(condition.AgeMin != null) {
                var isValidMinAge = condition.AgeMin <= age;
                result &= isValidMinAge;
            }
            if(condition.AgeMax != null) {
                var isValidMaxAge = condition.AgeMax >= age;
                result &= isValidMaxAge;
            }
            if(condition.City != null) {
                var isValidCity = condition.City.Exists(x => x.Equals(city));
                result &= isValidCity;
            }
            if(condition.Job != null) {
                var isValidJob = condition.Job.Exists(x => x.Equals(job));
                result &= isValidJob;
            }
            return result;
        }
        public static int GetDifferenceAsYear(DateTime maxDate, DateTime minDate) {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = maxDate - minDate;
            int years = (zeroTime + span).Year - 1;
            return years;
        }
    }
}
