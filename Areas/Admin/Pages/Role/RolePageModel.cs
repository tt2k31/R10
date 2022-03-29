using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using R10.models;

namespace R10.Admin.role
{
    public class RolePageModel : PageModel
    {
        public readonly RoleManager<IdentityRole> _RoleManager;
        public readonly MyBlogContext _context;
        [TempData]
        public string StatusMessage {set; get;}
        public RolePageModel(RoleManager<IdentityRole> RoleManager, MyBlogContext context)
        {
            _RoleManager = RoleManager;
            _context = context;
        }
    }
}