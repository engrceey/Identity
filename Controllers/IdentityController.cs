using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using identity.Models;
using identity.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace identity.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityController(UserManager<IdentityUser> userManager,
                                IEmailSender emailSender,
                                SignInManager<IdentityUser> signInManager,
                                RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _userManager = userManager;

        }

        public IActionResult Signup()
        {
            var model = new SignupViewModel() { Role = "Member" };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (!(await _roleManager.RoleExistsAsync(model.Role)))
                {
                    var role = new IdentityRole { Name = model.Role };
                    var roleResult = await _roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        var errors = roleResult.Errors.Select(s => s.Description);
                        ModelState.AddModelError("Role", string.Join(",", errors));
                        return View(model);
                    }
                }


                if ((await _userManager.FindByEmailAsync(model.Email)) == null)
                {
                    var user = new IdentityUser
                    {
                        Email = model.Email,
                        UserName = model.Email
                    };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    user = await _userManager.FindByEmailAsync(model.Email);

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    if (result.Succeeded)
                    {
                        var claim = new Claim("Department", model.Department);
                        await _userManager.AddClaimAsync(user, claim);
                        var confirmationLink = Url.ActionLink("ConfirmEmail", "Identity", new { userId = user.Id, @token = token });
                        await _userManager.AddToRoleAsync(user, model.Role);
                        await _emailSender.SendEmailAsync("excelcraftman@gmail.com", user.Email, "Please confirm your email address", confirmationLink);
                        return RedirectToAction("Signin");
                    }
                    ModelState.AddModelError("Signup", string.Join("", result.Errors.Select(x => x.Description)));
                    return View(model);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new NotFoundResult();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return RedirectToAction("Signin");
            }

            return new NotFoundResult();
        }


        public IActionResult Signin()
        {
            return View(new SigninViewModel());

        }

        [HttpPost]
        public async Task<IActionResult> Signin(SigninViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.UserName);

                    if (await _userManager.IsInRoleAsync(user, "Member"))
                    {
                        return RedirectToAction("Member", "Home");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("Login", "Cannot login");
            }
            return View(model);
        }

        public async Task<IActionResult> Signout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Signin");
        }

        [Authorize]
        public async Task<IActionResult> MFASetup()
        {
            // Gets the current loggin user
            var user = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            var token = await _userManager.GetAuthenticatorKeyAsync(user);
            var model = new MFAViewModel
            {
                Token = token,

            };
            
            return View(model);
        }

        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MFASetup(MFAViewModel model)
        {
           if(ModelState.IsValid)
           {
               var user = await _userManager.GetUserAsync(User);
               var result = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, model.Code);
               if(result)
               {
                   await _userManager.SetTwoFactorEnabledAsync(user, true);
               }
               else
               {
                   ModelState.AddModelError("Verify", "Authentication cannot be validated");
               }
           } 
           return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }


}