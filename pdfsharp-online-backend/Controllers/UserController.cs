
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using pdfsharp_online_backend.Domain;
using pdfsharp_online_backend.Domain.DTO;
using pdfsharp_online_backend.Helpers;
using System.Security.Cryptography;

namespace pdfsharp_online_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public UserController(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [Authorize]
        [HttpGet]
        [Route("info/{upn}")]
        public async Task<ActionResult<User>> GetUserByUPN(string upn)
        {
            return Ok(await _context.Users.AsNoTracking().Include(u => u.Role).SingleOrDefaultAsync(u => u.UPN.ToLower() == upn.ToLower()));
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userObj = await _context.Users.FirstOrDefaultAsync(u => u.UPN == user.UPN);

            if (userObj == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }

            if (!PasswordHasher.VerifyPassword(user.Password, userObj.Password))
            {
                return BadRequest(new { Message = "Password is incorrect" });
            }

            userObj.Token = CreateJwt(userObj);

            var newAccessToken = userObj.Token;
            var newRefreshToken = CreateRefreshToken();
            userObj.RefreshToken = newRefreshToken;
            userObj.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _context.SaveChangesAsync();

            return Ok(new TokenApiDTO()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser([FromBody] User user)
        {
            user.RoleId = user.Role.Id;
            user.Role = null;

            if (user == null)
            {
                return BadRequest();
            }

            if (user.RoleId == 0)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                return BadRequest(new { Message = "Password is empty" });
            }

            if (await CheckUPNExistAsync(user.UPN))
            {
                return BadRequest(new { Message = "User already exists" });
            }

            user.Password = PasswordHasher.HashPassword(user.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        private Task<bool> CheckUPNExistAsync(string? upn)
            => _context.Users.AnyAsync(x => x.UPN == upn);

        private string CreateJwt(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysceret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Upn,$"{user.UPN}"),
                new Claim(ClaimTypes.Name,$"{user.DisplayName}"),
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _context.Users.Any(a => a.RefreshToken == refreshToken);

            if (tokenInUser)
            {
                return CreateRefreshToken();
            }

            return refreshToken;

        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryverysceret.....");

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("This is Invalid Token");
            }

            return principal;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiDTO tokenApiDTO)
        {
            if (tokenApiDTO is null)
                return BadRequest("Invalid Client Request");
            string accessToken = tokenApiDTO.AccessToken;
            string refreshToken = tokenApiDTO.RefreshToken;
            var principal = GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.DisplayName == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();

            return Ok(new TokenApiDTO { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        [HttpPost("send-reset-email/{upn}")]
        public async Task<IActionResult> SendEmail(string upn)
        {
            var user = await _context.Users.FirstOrDefaultAsync(a => a.UPN == upn);

            if(user is null)
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "email doesnt exists"
                });

            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);

            string from = _configuration["EmailSettings:From"];
            var emailModel = new EmailModel(upn, "Reset Password!!", EmailBody.EmailStringBody(upn, emailToken));

            _emailService.SendEmail(emailModel);
            _context.Entry(upn).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent!"
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var newToken = resetPasswordDTO.EmailToken.Replace(" ", "+");
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(a => a.UPN == resetPasswordDTO.Email);

            if (user is null)
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "email doesnt exists"
                });

            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetPasswordExpiry;
            if(tokenCode != resetPasswordDTO.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Ivalid Reset link"
                });
            }

            user.Password = PasswordHasher.HashPassword(resetPasswordDTO.NewPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Password Has been changed successfully"
            });
        }
    }
}
