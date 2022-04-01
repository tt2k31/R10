using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R10.models;

namespace R10.Admin.user
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EditUserRoleClaimModel(
                MyBlogContext context,
                UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { set; get; }
        [BindProperty]
        public InputModel Input { set; get; }
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

        public AppUser user { set; get; }
        public IdentityUserClaim<string> userclaim {set; get;}
        public NotFoundObjectResult OnGet() => NotFound("ko truy cập");

        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("ko tim thay");

            return Page();
        }
        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("ko tim thay");

            if (!ModelState.IsValid) return Page();

            var claims = _context.UserClaims.Where(c => c.UserId == userid);

            if (claims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "User Đã có đặc tính này");
                return Page();
            }

            var result = await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(err =>{
                    ModelState.AddModelError(string.Empty, err.Description);
                });
            }
            StatusMessage = "Thêm thành công đặc tính";
            return RedirectToPage("./AddRole", new{ id = user.Id});
        }

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("ko tim thay");
            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if (user == null) return NotFound("ko tim thay");

            Input = new InputModel()
            {
                ClaimType = userclaim.ClaimType,
                ClaimValue = userclaim.ClaimValue
            };

            return Page();
        }
        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("ko tim thay");
            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if (user == null) return NotFound("ko tim thay");

            if(!ModelState.IsValid) return Page();


            if(_context.UserClaims.Any(c => c.UserId == user.Id && 
                                         c.ClaimType == Input.ClaimType && 
                                         c.ClaimValue == Input.ClaimValue &&
                                         c.Id != userclaim.Id))
            {
                ModelState.AddModelError(string.Empty, "User Đã có đặc tính này");
                return Page();
            }

            userclaim.ClaimType = Input.ClaimType;
            userclaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = "Cập nhật thành công đặc tính";
            return RedirectToPage("./AddRole", new{ id = user.Id});
        }

         public async Task<IActionResult> OnPostDeleteClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("ko tim thay");
            userclaim = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();

            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if (user == null) return NotFound("ko tim thay");

            await _userManager.RemoveClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));

            StatusMessage = "Xoa thành công đặc tính";
            return RedirectToPage("./AddRole", new{ id = user.Id});
        }
    }
}
