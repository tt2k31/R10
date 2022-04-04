using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using R10.models;

namespace R10.Security.Requirements
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;
        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            _logger.LogInformation(" context.resource " + context.Resource?.GetType().Name);
            var requirements = context.PendingRequirements.ToList();
            foreach (var r in requirements)
            {
                //có thể xử lí nhiều requirement
                // if (r is Requirement)
                // {
                //     //code xử lí
                //     //kết thúc
                //     // context.Succeed(r);
                // }
                if (r is GenZRequirement)
                {
                    //code xử lí
                    if (IsGenZ(context.User, (GenZRequirement)r))
                    {
                        context.Succeed(r);
                    }

                }
                if (r is ArticleUpdateRequirement)
                {
                    bool canupdate = CanUpdateArticle(context.User, context.Resource, (ArticleUpdateRequirement)r);
                    if (canupdate) context.Succeed(r);
                }
            }

            return Task.CompletedTask;
        }

        private bool CanUpdateArticle(ClaimsPrincipal user, object resource, ArticleUpdateRequirement r)
        {
            if (user.IsInRole("Admin"))
            {
                _logger.LogInformation("da cap nhat");
                return true;
            }
            var article = resource as Article;
            var dateCreated = article.Created;
            var dateCanUpdate = new DateTime(r.Year, r.Month, r.Day);
            if (dateCreated < dateCanUpdate)
            {
                _logger.LogInformation("qua han cap nhat");
                return false;
            }

            return true;

        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement r)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll();
            var appUser = appUserTask.Result;
            if (appUser.BirthDate == null)
            {
                _logger.LogInformation($"{appUser.UserName} ko co ngay sinh");
                return false;
            };

            int year = appUser.BirthDate.Value.Year;

            var result = (year >= r.fromYear && year <= r.toYear);

            if (result)
            {
                _logger.LogInformation("Thoa man");
                _logger.LogInformation($"{r.toYear}");
            }
            else
            {
                _logger.LogInformation("KO hoa man");
                _logger.LogInformation($"{r.toYear}");
            }
            return result;
        }
    }
}