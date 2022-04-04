using Microsoft.AspNetCore.Authorization;

namespace R10.Security.Requirements
{
    public class GenZRequirement : IAuthorizationRequirement
    {
        public GenZRequirement(int fromYear = 1997, int toYear = 2012)
        {
            this.fromYear = fromYear;
            this.toYear = toYear;
        }

        public int fromYear {set; get;}
        public int toYear {set; get;}
        
    }
}