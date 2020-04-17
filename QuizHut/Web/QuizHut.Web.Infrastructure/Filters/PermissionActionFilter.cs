namespace QuizHut.Web.Infrastructure.Filters
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Distributed;
    using QuizHut.Common;
    using QuizHut.Data.Models;
    using QuizHut.Services.Quizzes;

    public class PermissionActionFilter : IAsyncActionFilter
    {
        private readonly IQuizzesService quizzesService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDistributedCache distributedCache;

        public PermissionActionFilter(
            IQuizzesService quizzesService,
            UserManager<ApplicationUser> userManager,
            IDistributedCache distributedCache)
        {
            this.quizzesService = quizzesService;
            this.userManager = userManager;
            this.distributedCache = distributedCache;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string id;
            var password = context.ActionArguments
                .Where(x => x.Value != null && x.Key.ToString() == "password")
                .Select(x => x.Value.ToString()).FirstOrDefault();

            if (context.ActionArguments.TryGetValue("id", out object value))
            {
                id = value.ToString();
            }
            else
            {
                id = await this.quizzesService.GetQuizIdByPasswordAsync(password);
            }

            var user = await this.userManager.GetUserAsync(context.HttpContext.User);
            var roles = await this.userManager.GetRolesAsync(user);
            var isQuizTaken = await this.distributedCache.GetStringAsync(user.Id) != null;
            var permissionResult = await this.quizzesService.HasUserPermition(user.Id, id, isQuizTaken);
            var userHasPermitionToTakeTheQuiz = permissionResult[0];
            var isCreator = permissionResult[1];

            if (!userHasPermitionToTakeTheQuiz)
            {
                var controller = roles.Count > 0 ? "Home" : "Students";
                var routObject = new
                {
                    password,
                    area = roles.Count > 0 ? GlobalConstants.Administration : string.Empty,
                    errorText = GlobalConstants.ErrorMessages.PermissionDenied,
                };

                context.Result = new RedirectToActionResult("Index", controller, routObject);
            }

            if (userHasPermitionToTakeTheQuiz)
            {
                await next();
            }
        }
    }
}
