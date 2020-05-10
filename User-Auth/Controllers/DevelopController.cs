using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Service.Models;
using Service.Services;

namespace User_Auth.Controllers {
    [ApiController]
    [Route("api/")]
    public class DevelopController : ControllerBase {
        private readonly UserService userService;
        public DevelopController(IOptions<AuthorizeSettings> authorizeSettings) {
            userService = new UserService(authorizeSettings.Value);
        }

        [HttpGet("GetToken/{id}")]
        public IActionResult GetToken(int id) {
            try {
                return Ok(new { token = userService.GetUserToken(id) });
            }
            catch (ValidationException ex) {
                return ValidationException.GetResponse(ex.Type, new { Error = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        [HttpGet("Filter")]
        public IActionResult FilterTest() => Ok();

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Guest")]
        [HttpGet("GetRole")]
        public IActionResult GuestTest() {
            return Ok(new { Role = "Guest" });
        }
    }
}