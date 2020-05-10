namespace Service.Models {
    public class AuthorizeSettings {
        public const string Issuer = "MyIdentityServer";
        public const string Audience = "MyIdentityClient";
        public string JWT_Secret { get; set; }
        public string Client_URL { get; set; }
    }
}
