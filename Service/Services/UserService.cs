using Service.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Service.Services {
    public class UserService {
        private readonly AuthorizeSettings authorizeSettings;
        public UserService(AuthorizeSettings authorizeSettings) {
            this.authorizeSettings = authorizeSettings;
        }

        public string GetUserToken(int? id) {
            if (id == null) throw ValidationException.UserIdNotSpecified();
            Dictionary<int, UserModel> users = new Dictionary<int, UserModel>() {
                { 0, new UserModel() { Role = Role.Admin.ToString() } },
                { 1, new UserModel() { Role = Role.Guest.ToString() } }
            };
            if (!users.ContainsKey(id.Value)) throw ValidationException.UserNotFound();
            var user = users[id.Value];

            var identity = GetIdentity(user, id.Value);
            var jwt = new JwtSecurityToken(
                    issuer: AuthorizeSettings.Issuer,
                    audience: AuthorizeSettings.Audience,
                    notBefore: DateTime.Now,
                    claims: identity.Claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authorizeSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private ClaimsIdentity GetIdentity(UserModel user, int id) {
            var claims = new List<Claim>() {
                new Claim(ClaimsIdentity.DefaultNameClaimType, id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
