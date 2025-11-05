using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ASP.NETCore5.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ASP.NETCore5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtAuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        public JwtAuthController(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // POST: /api/jwtauth/login2
        [HttpPost("login2")]
        public async Task<IActionResult> Login2([FromBody] LoginView model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Invalid credentials");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials");

            // optional: require email confirmed
            //if (!await _userManager.IsEmailConfirmedAsync(user))
            //{
            //    return Unauthorized("Email not confirmed");
            //}

            // Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // create claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
            .Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)))
            .ToArray();

            // read jwt settings
            var jwtSection = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSection["ExpireMinutes"])),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Optionally store JWT in session (if you want)
            HttpContext.Session.SetString("JWToken", tokenString);
            HttpContext.Session.SetString("UserName", user.UserName);

            return Ok(new
            {
                token = tokenString,
                expires = token.ValidTo,
                username = user.UserName,
                roles = roles
            });
        }

        // Example protected endpoint that requires JWT bearer token
        [HttpGet("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Profile()
        {
            var name = User.Identity.Name;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            return Ok(new { user = name, roles });
        }

        [HttpGet("test")]
        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult TestJwt()
        {
            return Ok(new { message = "JWT is valid!", user = User.Identity.Name });
        }
    }
}
