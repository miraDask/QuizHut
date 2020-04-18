namespace QuizHut.Web.Infrastructure.Components
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Web.ViewModels.Events;

    [ViewComponent(Name = "Results")]
    public class FilteredResults : ViewComponent
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEventsService eventService;

        public FilteredResults(UserManager<ApplicationUser> userManager, IEventsService eventService)
        {
            this.userManager = userManager;
            this.eventService = eventService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Status status, ClaimsPrincipal principal)
        {
            var userId = this.userManager.GetUserId(principal);
            var eventsModel = await this.eventService.GetAllByCreatorIdAsync<EventSimpleViewModel>(userId);
            eventsModel = eventsModel.Where(x => x.Status == status).ToList();
            return this.View(eventsModel);
        }
    }
}
