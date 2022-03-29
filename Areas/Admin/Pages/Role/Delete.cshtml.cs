using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R10.models;

namespace R10.Admin.role
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> RoleManager, MyBlogContext context) : base(RoleManager, context)
        {
        }



        [BindProperty]
        
        public IdentityRole role { set; get; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null)
            {
                return NotFound("ko tim thay");
            }

            role = await _RoleManager.FindByIdAsync(roleid);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
            {
                return NotFound("ko tim thay");
            }

            role = await _RoleManager.FindByIdAsync(roleid);

            if (role == null) return NotFound("ko tim thay");

            var result = await _RoleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = "Ban vừa xóa thành công";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(err =>
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                });
            }

            return Page();
        }
    }
}
