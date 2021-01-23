using System;
using System.Linq;
using System.Threading.Tasks;
using identity.Models;
using identity.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identity.Controllers
{
    public class IdentityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public IdentityController(UserManager<IdentityUser> userManager, 
                                IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _userManager = userManager;

        }

        public async Task<IActionResult> Signup()
        {
            var model = new SignupViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
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
                        var confirmationLink = Url.ActionLink("ConfirmEmail", "Identity", new { userId = user.Id, @token = token });

                        await _emailSender.SendEmailAsync("excelcraftman@gmail.com", user.Email, "Please confirm your email address", confirmationLink );
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

            if(user == null)
            {
                return new NotFoundResult();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            
            if(result.Succeeded)
            {
                return RedirectToAction("Signin");
            }

            return new NotFoundResult();
        }


        public async Task<IActionResult> Signin()
        {
            return View();

        }

        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

    }
}