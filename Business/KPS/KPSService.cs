using KPSServiceTC;
using System;
using System.Globalization;

namespace Business.KPS {
    public class KPSService : IKPSService {
        #region KPSService
        private /*async Task<bool>*/ bool Validate(string name, string surname, ulong TCKN, int birthOfYear) {

            try {
                bool isValidate = false;
                var cultureInfo = new CultureInfo("tr-TR");
                var tcknNo = Convert.ToInt64(TCKN);
                string nameUpper = name.ToUpper(cultureInfo);
                string surnameUpper = surname.ToUpper(cultureInfo);
                using(KPSPublicSoapClient kpsService = new KPSPublicSoapClient(new KPSPublicSoapClient.EndpointConfiguration())) {
                    var tcKimlikDogrulaResponse = /*await*/ kpsService.TCKimlikNoDogrulaAsync(tcknNo, nameUpper, surnameUpper, birthOfYear);
                    isValidate = tcKimlikDogrulaResponse.Result.Body.TCKimlikNoDogrulaResult;
                    /// 
                    /// Await olayına bak asenkron doldurmuyor.
                    ///
                }
                return isValidate;
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region IKPSService

        bool IKPSService.Validate(string name, string surname, ulong TCKN, int birthOfYear) => Validate(name, surname, TCKN, birthOfYear);

        #endregion

    }
}
