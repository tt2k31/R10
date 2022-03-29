using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using R10.models;

namespace R10.Admin.role
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> RoleManager, MyBlogContext context) : base(RoleManager, context)
        {
        }

        public List<IdentityRole> roles { set; get; }
        public async Task OnGet()
        {
            roles = await _RoleManager.Roles.OrderBy(c => c.Name).ToListAsync();
        }

        public void OnPost() => RedirectToPage();
    }
}
