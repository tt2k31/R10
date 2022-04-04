using Microsoft.AspNetCore.Authorization;

namespace R10.Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
        public ArticleUpdateRequirement(int year = 2021, int month = 06, int day = 01)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public int Year {set; get;}
        public int Month {set; get;}
        public int Day {set; get;}
    }
}