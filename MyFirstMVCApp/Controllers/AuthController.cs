using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFirstMVCApp.Dto;
using MyFirstMVCApp.Models;
using System.Security.Claims;
using System.Text;

namespace MyFirstMVCApp.Controllers
{
    public class AuthController(AppDbContext _context) : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult LoginToRegister()
        {
            return RedirectToAction("Registration");
        }

        public IActionResult RegisterToLogin()
        {
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> RegisterUser(UserDto dto)
        {
            if (dto == null)
            {
                ViewBag.Message = "Please provide email and password.";
                return View("Login");
            }
            if (dto.Email == null || dto.Password == null)
            {
                ViewBag.Message = "Email and password are required.";
                return View("Login");
            }

            var data = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if(data != null)
            {
                ViewBag.Message = "Email already exists.";
                return View("Registration");
            }

            _context.Users.Add(new User
            {
                Username = dto.Username,
                Password = dto.Password,
                Email = dto.Email
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Login");

        }

        public IActionResult LoginUser(UserDto dto)
        {
            if(dto == null)
            {
                ViewBag.Message = "Please provide email and password.";
                return View("Login");
            }
            if (dto.Email == null || dto.Password == null)
            {
                ViewBag.Message = "Email and password are required.";
                return View("Login");
            }

            var isExist = _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if(isExist == null)
            {
                ViewBag.Message = "Email does not exist.";
                return View("Login");
            }

            if(isExist.Result.Password != dto.Password)
            {
                ViewBag.Message = "Incorrect password.";
                return View("Login");
            }

            var token = GenerateJwtToken(dto);

            Response.Cookies.Append("jwt_Token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return RedirectToAction("Index", "Dashboard");

        }

        private string GenerateJwtToken(UserDto  dto)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("d2f1d58034bf9137e6e399385843aa23d7fa70970794d0db4f09ff19d249ae5b4b890341");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, dto.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_Token");
            return RedirectToAction("Login");
        }
    }
}
