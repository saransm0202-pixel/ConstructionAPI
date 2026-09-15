using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SSConstructions.Repository.Interface;
using SSConstructions.Repository.Models;
using SSConstructions.Repository.Utility;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SSConstructions.Controllers.API
{
    [ApiController]
    [EnableCors("addCors")]
    [Route("api/[controller]")]
    public class LoginAPIController : Controller
    {
        private readonly CustomLogger _logger;
        private readonly ILoginRepo _loginRepo;
        private readonly IUsers _usersRepo;
        private readonly IOptions<JWTSettings> _jwtSettings;
        private static readonly ConcurrentDictionary<string, (string Otp, DateTime Expiry)> _otpStore = new();
        public LoginAPIController(CustomLogger logger, ILoginRepo loginRepo, IUsers usersRepo, IOptions<JWTSettings> jwtSettings)
        {
            _logger = logger;
            _loginRepo = loginRepo;
            _usersRepo = usersRepo;
            _jwtSettings = jwtSettings;
        }
        [Route("LoginSignupUsers")]
        [HttpPost()]
        public async Task<IActionResult> LoginSignupUsers([FromBody] UserModel userModel)
        {
            try
            {
                _logger.Log("UserAPIController.LoginSignupUsers Started.");
                var expireTime = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenDays);
                var userResult = await _loginRepo.LoginUser(userModel);
                if (userResult == null || userResult.Id <= 0)
                {
                    return Ok(new
                    {
                        user = userResult,
                        accessToken = string.Empty,
                        refreshToken = string.Empty
                    });
                }
                var refeshToken = GenerateRefreshToken();
                _loginRepo.updateTokenInfo(userResult.Id, refeshToken, expireTime, userModel.AccountId);
                userResult.expiresAt = expireTime;
                return Ok(new
                {
                    user = userResult,
                    accessToken = GenerateAccessToken(userResult),
                    refreshToken = refeshToken
                });
                _logger.Log("UserAPIController.LoginSignupUsers Completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return BadRequest(ex.Message);
            }
        }
        [Route("SendOTP")]
        [HttpPost()]
        public IActionResult SendOtp([FromBody] EmailRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            // Validate email format
            if (!new EmailAddressAttribute().IsValid(request.Email))
                return BadRequest("Invalid email format.");

            // Generate 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var user = _usersRepo.GetUserByEmail(request.AccountId, request.Email).Result;
            if(user == null)
            {
                return Ok(new { Success = false, Message = "UnAuthorized User Access, Contact Administrator." });
            }
            // Store OTP with 3-minute expiry
            _otpStore[request.Email] = (otp, DateTime.UtcNow.AddMinutes(3));
            var accountName = _loginRepo.GetAccountName(request.AccountId).Result;
            string subject = $"Your {accountName} OTP Code";
            string body = $@"
                        <!DOCTYPE html>
                        <html>
                        <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                          <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; background-color: #ffffff; border-radius: 10px; overflow: hidden; margin-top: 40px; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>
                            <tr>
                              <td style='background-color: #4CAF50; padding: 20px; text-align: center; color: #ffffff;'>
                                <h1 style='margin: 0; font-size: 24px;'>{accountName}</h1>
                                <p style='margin: 5px 0 0; font-size: 14px;'>Secure Verification Code</p>
                              </td>
                            </tr>
                            <tr>
                              <td style='padding: 30px; text-align: center;'>
                                <h2 style='color: #333;'>Your One-Time Password (OTP)</h2>
                                <p style='font-size: 16px; color: #555;'>Use the code below to verify your email address. This code is valid for the next <strong>3 minutes</strong>.</p>
                                <div style='margin: 25px auto; display: inline-block; background: #f1f1f1; border-radius: 8px; padding: 15px 30px;'>
                                  <h2 style='color: #4CAF50; font-size: 32px; margin: 0; letter-spacing: 4px;'>{otp}</h2>
                                </div>
                                <p style='font-size: 15px; color: #777; margin-top: 25px;'>If you didn’t request this verification, please ignore this message.</p>
                              </td>
                            </tr>
                            <tr>
                              <td style='background-color: #f9f9f9; text-align: center; padding: 20px;'>
                                <p style='font-size: 14px; color: #999; margin: 0;'>Thanks,<br/>The <strong>{accountName} Team</strong></p>
                              </td>
                            </tr>
                          </table>
                        </body>
                        </html>";

            try
            {
                Email.SendEmail(request.Email, subject, body);
                return Ok(new { Success = true, Message = "OTP sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to send email. Please try again later.");
            }
        }

        [Route("VerifyOTP")]
        [HttpPost()]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
                return BadRequest("Email and OTP are required.");

            if (!_otpStore.TryGetValue(request.Email, out var otpInfo))
                return BadRequest("No OTP found for this email.");

            // Check expiry
            if (DateTime.UtcNow > otpInfo.Expiry)
            {
                _otpStore.TryRemove(request.Email, out _);
                return BadRequest("OTP expired. Please request a new one.");
            }

            // Check OTP match
            if (otpInfo.Otp != request.Otp)
                return BadRequest("Invalid OTP.");

            // OTP is valid → remove it
            _otpStore.TryRemove(request.Email, out _);

            return Ok(new { Success = true, Message = "OTP verified successfully." });
        }
        public string GenerateAccessToken(UserModel user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Value.Key));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Value.Issuer,
                audience: _jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.AccessTokenMinutes),
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
