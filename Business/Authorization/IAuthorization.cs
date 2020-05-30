using Business.Model;

namespace Business.Authorization {
    public interface IAuthorization {
        string Authorize(string email, bool isActive);
        TokenResponseModel ConvertAndRefreshToken(string token);
    }
}