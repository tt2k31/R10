using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R10.models;

namespace R10.Admin.role
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> RoleManager, MyBlogContext context) : base(RoleManager, context)
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
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var NewRole = new IdentityRole(Input.Name);
            var result = await _RoleManager.CreateAsync(NewRole);
            if (result.Succeeded)
            {
                StatusMessage = "Ban vừa tạo thành công";
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
