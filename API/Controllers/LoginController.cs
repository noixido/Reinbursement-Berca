using API.Repositories;
using API.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowOrigin")]
    public class LoginController : Controller
    {
        private readonly LoginRepository _repository;
        private readonly IConfiguration _configuration;
        public LoginController(LoginRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVM)
        {
            try
            {
                var login = _repository.Login(loginVM);
                if (!login)
                {
                    return BadRequest(new
                    {
                        status = StatusCodes.Status400BadRequest,
                        message = "Login Failed!",
                        data = (object) false,
                    });
                }

                var payload = _repository.GetPayload(loginVM.Email);
                payload.Email = loginVM.Email;

                var claims = new List<Claim>
                {
                    new Claim("id_account", payload.Id_Account),
                    new Claim("email", payload.Email),
                    new Claim("name", payload.Name),
                    new Claim("role", payload.Roles),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:API"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: signIn
                );
                var tokenResult = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    status = StatusCodes.Status200OK,
                    message = "Login Success!",
                    token = (object)tokenResult,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = StatusCodes.Status400BadRequest,
                    message = ex.Message,
                });
            }
        }
    }
}
