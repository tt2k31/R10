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

        // public List<IdentityRole> roles { set; get; }

        public class RoleModel : IdentityRole
        {
            public string[] Claims {set; get;}
        }

        public List<RoleModel> roles { set; get; }

        public async Task OnGet()
        {
            // roles = await _RoleManager.Roles.OrderBy(c => c.Name).ToListAsync();

            //lấy các claim trong role

            var r = await _RoleManager.Roles.OrderBy(c => c.Name).ToListAsync();
            roles = new List<RoleModel>();
            
            foreach (var _r in r)
            {
                var claims = await _RoleManager.GetClaimsAsync(_r);
                var claimsString = claims.Select(c => c.Type + "=" + c.Value);

                var rm = new RoleModel() {
                    Id = _r.Id,
                    Name = _r.Name,
                    Claims = claimsString.ToArray()
                };

                roles.Add(rm);
            }

        }

        public void OnPost() => RedirectToPage();
    }
}
