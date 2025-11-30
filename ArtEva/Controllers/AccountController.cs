using ArteEva.Data;
using ArteEva.Models;
using ArteEva.Models.DTOs;
using ArtEva.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ArtEva.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public AccountController(UserManager<User> userManager) 
        {
            this.userManager = userManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationViewModel regReq)
        {
            if(ModelState.IsValid)
            {
                User user = new User();
                user.Email = regReq.Email;
                user.UserName= regReq.UserName;
                user.PhoneNumber = regReq.PhoneNumber;
                IdentityResult result =
                    await userManager.CreateAsync(user,regReq.Password);
                if (result.Succeeded)
                {

                }

            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public IActionResult Login()
        {
            if (ModelState.IsValid)
            {

            }
            return BadRequest(ModelState);
        }
    }
}
