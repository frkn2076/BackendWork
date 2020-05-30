namespace Business.Model {
    public class BaseModel {
        public bool isError { get; set; }
        public string errorName { get; set; }
        public string errorDescription { get; set; }
        public string token { get; set; }

        public static implicit operator BaseModel(string message) {
            var model = new BaseModel();
            model.isError = true;
            model.errorName = "Hata!";
            model.errorDescription = message;
            return model;
        }
    }
    
}
