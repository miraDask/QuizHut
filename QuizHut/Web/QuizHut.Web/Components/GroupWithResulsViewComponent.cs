namespace QuizHut.Web.ViewComponents
{
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Events;
    using QuizHut.Web.ViewModels.Results;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    [ViewComponent(Name="ResultsByGroup")]
    public class GroupWithResulsViewComponent : ViewComponent
    {
        private readonly IEventsService eventService;

        public GroupWithResulsViewComponent(IEventsService eventService)
        {
            this.eventService = eventService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string groupName, string eventId)
        {
            var resultsModel = await this.eventService.GetAllResultsByEventIdAsync<ResultViewModel>(eventId, groupName);
            return this.View(resultsModel);
        }
    }
}
