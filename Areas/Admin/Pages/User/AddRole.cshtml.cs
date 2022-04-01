// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using R10.models;

namespace R10.Admin.user
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _context;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            MyBlogContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 
   
        // [BindProperty]
        // public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>


        public AppUser user { set; get; }

        [BindProperty]
        [Display(Name = "Các role cho user")]
        public string[] RoleNames { set; get; }

        public SelectList allRoles { set; get; }

        public List<IdentityRoleClaim<string>> claimInRole {set; get;}
        public List<IdentityUserClaim<string>> claimInUserRole {set; get;}

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("ko thay");
            }
            user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"KO co Tai khoan");
            }

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            await GetClaim(id);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound("ko thay");
            }
            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound($"KO co Tai khoan");
            }

            await GetClaim(id);

            //Rolesname
            var OldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            var deleRole = OldRoleNames.Where(r => !RoleNames.Contains(r));
            var addRole = RoleNames.Where(r => !OldRoleNames.Contains(r));

            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            var resultDelete = await _userManager.RemoveFromRolesAsync(user, deleRole);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(er =>
                {
                    ModelState.AddModelError(string.Empty, er.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, addRole);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(er =>
                {
                    ModelState.AddModelError(string.Empty, er.Description);
                });
                return Page();
            }

            
            StatusMessage = $"Vừa cập nhật thành công {user.UserName}.";

            return RedirectToPage("./Index");
        }


        async Task GetClaim(string id)
        {
            var listRole = from r in _context.Roles
                            join ur in _context.UserRoles on r.Id equals ur.RoleId
                            where ur.UserId == id
                            select r;

            var _claimInrole = from c in _context.RoleClaims
                                join r in _context.Roles on c.RoleId equals r.Id
                                select c;

            claimInRole = await _claimInrole.ToListAsync();

            claimInUserRole = await (from u in _context.UserClaims
                                        where u.UserId == id select u).ToListAsync();

        }
    }
}
