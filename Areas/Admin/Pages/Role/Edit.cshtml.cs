using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R10.models;

namespace R10.Admin.role
{
    [Authorize(Roles = "Admin")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> RoleManager, MyBlogContext context) : base(RoleManager, context)
        {
        }

        public class InputModel
        {
            [Display(Name = "Name")]
            [Required(ErrorMessage = "{0} phải nhập")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tối thiểu {2} - {1} kí tự")]
            public string Name { set; get; }
        }

        [BindProperty]
        public InputModel Input { set; get; }
        public IdentityRole role {set; get;}
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null)
            {
                return NotFound("ko tim thay");
            }

            role = await _RoleManager.FindByIdAsync(roleid);
            if (role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };
                return Page();
            }
             return NotFound("ko tim thay");

        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null)
            {
                return NotFound("ko tim thay");
            }

            role = await _RoleManager.FindByIdAsync(roleid);

            if (role == null) return NotFound("ko tim thay");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            role.Name = Input.Name;

            var result = await _RoleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = "Ban vừa sửa thành công";
                return RedirectToPage("./Index");
            } 
            else
            {
                result.Errors.ToList().ForEach( err => {
                    ModelState.AddModelError(string.Empty, err.Description);
                });
            }

            return Page();
        }
    }
}