using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R10.models;

namespace R10.Admin.role
{
    [Authorize(Roles = "Admin")]
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> RoleManager, MyBlogContext context) : base(RoleManager, context)
        {
        }

        public class InputModel
        {
            [Display(Name = "ClaimType")]
            [Required(ErrorMessage = "{0} phải nhập")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tối thiểu {2} - {1} kí tự")]
            public string ClaimType { set; get; }

            [Display(Name = "ClaimValue")]
            [Required(ErrorMessage = "{0} phải nhập")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "Tối thiểu {2} - {1} kí tự")]
            public string ClaimValue { set; get; }
        }

        [BindProperty]
        public InputModel Input { set; get; }
        public IdentityRole role {set; get;}
        public async Task<IActionResult> OnGet(string roleid)
        {
            role = await _RoleManager.FindByIdAsync(roleid);
            if (role ==null)
            {
                return NotFound("ko tim thay");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            role = await _RoleManager.FindByIdAsync(roleid);
            if (role ==null)
            {
                return NotFound("ko tim thay");
            }

            if ((await _RoleManager.GetClaimsAsync(role)).Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim Đã có");
                return Page();
            }
            
            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result = await _RoleManager.AddClaimAsync(role, newClaim);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(err =>{
                    ModelState.AddModelError(string.Empty, "Có lỗi");
                });
                return Page();
            }
            StatusMessage = "Thêm Claim thành công";

            return RedirectToPage("./Edit",new {roleid = role.Id});
        }
    }
}
