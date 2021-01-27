using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiSecurityController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public ApiSecurityController(SignInManager<IdentityUser> signInManager,
                                    UserManager<IdentityUser> userManager,
                                    IConfiguration configuration)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
        }


        [AllowAnonymous]
        [Route("Auth")]
        [HttpPost]
        public async Task<IActionResult> TokenAuth(SigninViewModel model)
        {
            var issuer = _configuration["Tokens:Issuer"];
            var audience = _configuration["Tokens:Audience"];
            var Key = _configuration["Tokens:Key"];

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync
                (
                    model.UserName,model.Password,false,false
                    
                );
                

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.UserName);
                    if (user != null)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, user.Id)
                        };

                        var KeyByte = Encoding.UTF8.GetBytes(Key);
                        var theKey = new SymmetricSecurityKey(KeyByte);
                        var credential = new SigningCredentials(theKey, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(issuer,audience,claims,null,expires:DateTime.Now.AddMinutes(30), credential);

                        return Ok(new {token = new JwtSecurityTokenHandler().WriteToken(token)});
                    }
                }
            }

            return BadRequest();

        }
    }
}