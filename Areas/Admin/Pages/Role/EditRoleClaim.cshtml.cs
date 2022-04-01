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
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> RoleManager, MyBlogContext context) : base(RoleManager, context)
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
        public IdentityRole role { set; get; }
        public IdentityRoleClaim<string> claim { set; get; }
        public async Task<IActionResult> OnGet(int? claimid)
        {
            if (claimid == null) return NotFound("ko tim thay");

            claim = _context.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("ko tim thay");

            role = await _RoleManager.FindByIdAsync(claim.RoleId);
            if (role == null) return NotFound("ko tim thay");

            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (claimid == null) return NotFound("ko tim thay");

            claim = _context.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("ko tim thay");

            role = await _RoleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("ko tim thay");

            if (!ModelState.IsValid)
            {
                return Page();
            }


            if (_context.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType
                                        && c.ClaimValue == Input.ClaimValue && c.Id != claim.Id))
            {
                ModelState.AddModelError(string.Empty, "Claim Đã có");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();
            
            StatusMessage = "Cập nhật Claim thành công";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null) return NotFound("ko tim thay");

            claim = _context.RoleClaims.Where(rc => rc.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("ko tim thay");

            role = await _RoleManager.FindByIdAsync(claim.RoleId);

            if (role == null) return NotFound("ko tim thay");

            var result = await _RoleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));

            
            
            StatusMessage = "Xóa Claim thành công";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
