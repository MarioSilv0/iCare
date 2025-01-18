using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace backend.Controllers
{
    public class PublicUserController : Controller
    {
        private readonly UserManager<User> _userManager;

        public PublicUserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<PublicUser>> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var publicUser = new PublicUser
            {
                Name = user.Name,
                Email = user.Email,
                Birthdate = user.Birthdate,
                Height = user.Height,
                Weight = user.Weight
            };

            return publicUser;
        }

        [HttpPut]
        public async Task<ActionResult<PublicUser>> Edit(string id, [FromBody] PublicUser model)
        {
            if (model == null || id != model.Id)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = string.IsNullOrEmpty(model.Name) ? user.Name : model.Name;
            user.Email = string.IsNullOrEmpty(model.Email) ? user.Email : model.Email;
            user.Birthdate = model.Birthdate;
            user.Height = model.Height <= 0 || model.Height > 3 ? user.Height : model.Height;
            user.Weight = model.Weight <= 0 || model.Weight > 700 ? user.Weight : model.Weight;

            await _userManager.UpdateAsync(user);

            PublicUser pu = new PublicUser
            {
                Name = user.Name,
                Email = user.Email,
                Birthdate = user.Birthdate,
                Height = user.Height,
                Weight = user.Weight
            };

            return pu;
        }
    }

}


