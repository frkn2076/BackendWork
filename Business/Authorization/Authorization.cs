using Business.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Business.Authorization {
    public class Authorization : IAuthorization {

        private readonly AppSettings appSettings;
        
        public Authorization(IOptions<AppSettings> appSettings) {
            this.appSettings = appSettings.Value;
        }

        private string Authorize(string email, bool isActive) {
            //3 part of token => Header (Başlık), Payload (Yük), Signature (İmza)
            //Header => email ve isActive kombinasyionu :)
            try {
                var header = string.Concat(email, "+", isActive.ToString());
                // Authentication(Yetkilendirme) başarılı ise JWT token üretilir.
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, header)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var resultToken = tokenHandler.WriteToken(token);
                return resultToken;
            }
            catch(Exception ex) {
                return "Oturum açılamadı";
            }
        }

        private TokenResponseModel ConvertAndRefreshToken(string token) {
            var handler = new JwtSecurityTokenHandler();
            token = token.Replace("Bearer ", "");
            var jsonToken = handler.ReadToken(token);
            var tokenCollection = handler.ReadToken(token) as JwtSecurityToken;
            var header = tokenCollection.Claims.FirstOrDefault().Value;
            var headerCollection = header.Split("+");
            var email = headerCollection[0];
            var isActive = headerCollection[1] == true.ToString();
            var refreshedToken = Authorize(email, isActive);
            var result = new TokenResponseModel(email, isActive, refreshedToken);
            return result;
        }

        string IAuthorization.Authorize(string email, bool isActive) => Authorize(email, isActive);
        TokenResponseModel IAuthorization.ConvertAndRefreshToken(string token) => ConvertAndRefreshToken(token);
    }
}
