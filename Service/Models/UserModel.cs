namespace Service.Models {
    public class UserModel {
        public string Role { get; set; }
        public string Token { get; set; }
    }

    public enum Role { Admin, Guest }
}
