namespace Business.Model {
    public class TokenResponseModel {
        internal TokenResponseModel(string email, bool isActive, string token) {
            Email = email;
            IsActive = isActive;
            Token = token;
        }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string Token { get; set; }
    }
}
