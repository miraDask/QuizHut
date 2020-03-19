namespace QuizHut.Web.Infrastructure.ViewComponents
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Services.Results;
    using QuizHut.Web.ViewModels.Results;

    [ViewComponent(Name="ResultsByGroup")]
    public class GroupWithResulsViewComponent : ViewComponent
    {
        private readonly IResultsService service;

        public GroupWithResulsViewComponent(IResultsService service)
        {
            this.service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string groupName, string eventId)
        {
            var resultsModel = await this.service.GetAllResultsByEventIdAsync<ResultViewModel>(eventId, groupName);
            return this.View(resultsModel);
        }
    }
}
