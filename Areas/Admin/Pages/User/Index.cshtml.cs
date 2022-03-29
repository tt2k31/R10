using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using R10.models;

namespace R10.Admin.user
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        [TempData]
        public string StatusMessage {set; get;}

        public const int Item_per_Page = 20;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }

        public int countPages { set; get; }
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public int totalUser {set; get;}

        public class UserAndRole : AppUser
        {
            public string RoleNames {set; get;}
        }
        // public List<AppUser> users {set; get;}
        public List<UserAndRole> users {set; get;}
        public async Task OnGet()
        {
            // users = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
             var qr = _userManager.Users.OrderBy(u => u.UserName);


            totalUser = await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUser / Item_per_Page);

            if (currentPage <1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }

            var qr1 = (from a in _userManager.Users
                     orderby a.UserName 
                     select a)
                     .Skip((currentPage -1) *10)
                     .Take(Item_per_Page)
                     .Select(u => new UserAndRole(){
                         Id = u.Id,
                         UserName = u.UserName
                     });

            users = await qr1.ToListAsync();

            foreach (var item in users)
            {
                var roles = await _userManager.GetRolesAsync(item);
                item.RoleNames = string.Join(",", roles);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}
