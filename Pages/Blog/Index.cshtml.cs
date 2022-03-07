using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using R10.models;

namespace R10_web_EF.Pages_Blog
{
    public class IndexModel : PageModel
    {
        private readonly R10.models.MyBlogContext _context;

        public IndexModel(R10.models.MyBlogContext context)
        {
            _context = context;
        }

        public const int Item_per_Page = 20;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }

        public int countPages { set; get; }

        public IList<Article> Article { get; set; }

        public async Task OnGetAsync(string SearchString)
        {

            int totalArticle = await _context.articles.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticle / Item_per_Page);

            if (currentPage <1)
            {
                currentPage = 1;
            }
            if (currentPage > countPages)
            {
                currentPage = countPages;
            }


            // Article = await _context.articles.ToListAsync();

            var qr = (from a in _context.articles
                     orderby a.Created descending
                     select a)
                     .Skip((currentPage -1) *10)
                     .Take(Item_per_Page);

            if (!string.IsNullOrEmpty(SearchString))
            {
                Article = await qr.Where(a => a.Title.Contains(SearchString)).ToListAsync();
            }
            else
            {
                Article = await qr.ToListAsync();
            }
        }
    }
}
