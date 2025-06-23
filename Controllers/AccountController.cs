using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            this.config = config;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromForm] RegisterDTO user)
        {
            try
            {

                AppUser newUser = new AppUser()
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    UserName = user.Email
                };

                var result = await userManager.CreateAsync(newUser, user.Password);

                if (result.Succeeded)
                {
                    //add role to user
                    if (!await roleManager.RoleExistsAsync("Reader"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("Reader"));
                    }

                    await userManager.AddToRoleAsync(newUser, "Reader");
                    return Ok($"The user [{user.FullName}] is registered successfully");
                }
                else
                {
                    return StatusCode(500, result.Errors);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login([FromForm]LoginDTO login)
        {
            try
            {
                var user = await userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == login.Email.ToLower());
                if (user == null)
                {
                    return Unauthorized("Invaild User Email");
                }
                var signuser = await signInManager.CheckPasswordSignInAsync(user, login.Password, false);
                if (!signuser.Succeeded)
                {
                    return Unauthorized("Invaild User Email or Password");

                }
                return Ok(new { Email = login.Email, Token =await GenerateToken(user) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("assign_admin")]
        public async Task<ActionResult> AssginRole(string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
                return BadRequest("Invalid email format.");

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("No user has this email");
            }
            var role = await userManager.GetRolesAsync(user);

            if (role.Contains("Admin"))
            {
                return BadRequest("You are already admin");
            }
            if (role.Contains("Reader"))
            {
                await userManager.RemoveFromRoleAsync(user, "Reader");
            }
            await userManager.AddToRoleAsync(user, "Admin");
            return Ok($"{user.Email} is admin now");
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
                return BadRequest("Invalid email format.");

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized("No user found with this email.");

            await signInManager.SignOutAsync();
            return Ok($"You {email} logged out");
        }
        
        private async Task<string> GenerateToken(AppUser user)
        {
            var Claims = new List<Claim> {

                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName,user.FullName)
            };
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles) { 
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["JWT:skey"]));
            var signCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDicriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signCred,
                Issuer = config["JWT:iss"],
                Audience = config["JWT:aud"],
                Expires = DateTime.Now.AddDays(1),
                Subject = new ClaimsIdentity(Claims)
            };

            var TokenHandler = new JwtSecurityTokenHandler();
            var token = TokenHandler.CreateToken(tokenDicriptor);
            return TokenHandler.WriteToken(token);
        }

    }
}