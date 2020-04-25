namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Events;
    using QuizHut.Services.Groups;
    using QuizHut.Services.Results;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Groups;
    using QuizHut.Web.ViewModels.Results;

    public class ResultsController : AdministrationController
    {
        private const int PerPageDefaultValue = 5;
        private readonly IGroupsService groupsService;
        private readonly IEventsService eventService;
        private readonly IResultsService resultsService;

        public ResultsController(IGroupsService groupsService, IEventsService eventService, IResultsService resultsService)
        {
            this.groupsService = groupsService;
            this.eventService = eventService;
            this.resultsService = resultsService;
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public IActionResult Index()
        {
            return this.View();
        }

        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        public async Task<IActionResult> EventResultsDetails(string eventId, string groupId, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var eventModel = await this.eventService.GetEventModelByIdAsync<EventWithGroupViewModel>(eventId);
            eventModel.PagesCount = 0;
            eventModel.CurrentPage = page;
            eventModel.Groups = await this.groupsService.GetAllByEventIdAsync<GroupSimpleViewModel>(eventId);
            if (eventModel.Groups.ToList().Any())
            {
                eventModel.Group = groupId != null ? await this.groupsService.GetGroupModelAsync<GroupWithEventResultsViewModel>(groupId)
                    : await this.groupsService.GetEventsFirstGroupAsync<GroupWithEventResultsViewModel>(eventId);
                groupId ??= eventModel.Group.Id;
                var resultsCount = await this.resultsService.GetAllResultsByEventAndGroupCountAsync(eventId, groupId);
                if (resultsCount > 0)
                {
                    var results = await this.resultsService.GetAllResultsByEventAndGroupPerPageAsync<ResultViewModel>(eventId, groupId, page, countPerPage);
                    eventModel.Group.Results = results;
                    eventModel.PagesCount = (int)Math.Ceiling(resultsCount / (decimal)countPerPage);
                }
            }

            return this.View(eventModel);
        }

        [SetDashboardRequestToTrueInViewDataActionFilterAttribute]
        [HttpPost]
        public IActionResult EventResultsDetails(string eventId, string groupId)
        {
            return this.RedirectToAction("EventResultsDetails", new { eventId, groupId });
        }
    }
}
