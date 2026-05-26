using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudyPlanner.Models;
using StudyPlanner.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudyPlanner.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<User> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password
            );

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "Пользователь зарегистрирован"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(
                model.Username
            );

            if (user == null)
                return Unauthorized();

            var validPassword =
                await _userManager.CheckPasswordAsync(
                    user,
                    model.Password
                );

            if (!validPassword)
                return Unauthorized();

            var code = new Random()
                .Next(100000, 999999)
                .ToString();

            user.TwoFactorCode = code;
            user.TwoFactorCodeExpiry =
                DateTime.UtcNow.AddMinutes(5);

            await _userManager.UpdateAsync(user);

            Console.WriteLine(
                $"2FA code for {user.UserName}: {code}"
            );

            return Ok(new
            {
                requires2FA = true
            });
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> Verify2FA(
            [FromBody] Verify2FARequest model
        )
        {
            var user = await _userManager.FindByNameAsync(
                model.Username
            );

            if (user == null)
                return Unauthorized();

            if (
                user.TwoFactorCode != model.Code ||
                user.TwoFactorCodeExpiry <
                DateTime.UtcNow
            )
                return Unauthorized();

            user.TwoFactorCode = null;
            user.TwoFactorCodeExpiry = null;

            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            SetRefreshTokenCookie(refreshToken);

            return Ok(new
            {
                accessToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken =
                Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var user = _userManager.Users.FirstOrDefault(
                u => u.RefreshToken == refreshToken
            );

            if (user == null)
                return Unauthorized();

            if (
                user.RefreshTokenExpiryTime <=
                DateTime.UtcNow
            )
                return Unauthorized();

            var newAccessToken =
                GenerateJwtToken(user);

            var newRefreshToken =
                GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            SetRefreshTokenCookie(newRefreshToken);

            return Ok(new
            {
                accessToken = newAccessToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");

            return Ok();
        }

        private void SetRefreshTokenCookie(
            string refreshToken
        )
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                cookieOptions
            );
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),
                new Claim(
                    ClaimTypes.Name,
                    user.UserName
                )
            };

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _configuration["Jwt:Key"]
                    )
                );

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );

            var token = new JwtSecurityToken(
                issuer:
                    _configuration["Jwt:Issuer"],
                audience:
                    _configuration["Jwt:Audience"],
                claims: claims,
                expires:
                    DateTime.UtcNow.AddMinutes(15),
                signingCredentials:
                    credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng =
                RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(
                randomNumber
            );
        }
    }

    public class Verify2FARequest
    {
        public string Username { get; set; }
        public string Code { get; set; }
    }
}