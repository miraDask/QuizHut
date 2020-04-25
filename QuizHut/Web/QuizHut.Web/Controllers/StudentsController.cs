namespace QuizHut.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Common;
    using QuizHut.Data.Common.Enumerations;
    using QuizHut.Data.Models;
    using QuizHut.Services.Events;
    using QuizHut.Services.Results;
    using QuizHut.Web.Infrastructure.Helpers;
    using QuizHut.Web.ViewModels.Events;
    using QuizHut.Web.ViewModels.Quizzes;
    using QuizHut.Web.ViewModels.Results;

    [Authorize]
    public class StudentsController : Controller
    {
        private const int PerPageDefaultValue = 5;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IResultsService resultService;
        private readonly IEventsService eventsService;
        private readonly IDateTimeConverter dateTimeConverter;

        public StudentsController(
            UserManager<ApplicationUser> userManager,
            IResultsService resultService,
            IEventsService eventsService,
            IDateTimeConverter dateTimeConverter)
        {
            this.userManager = userManager;
            this.resultService = resultService;
            this.eventsService = eventsService;
            this.dateTimeConverter = dateTimeConverter;
        }

        public IActionResult Index(string password, string errorText)
        {
            var model = new PasswordInputViewModel();
            if (errorText != null)
            {
                model.Password = password;
                model.Error = errorText;
            }

            return this.View(model);
        }

        public async Task<IActionResult> Results(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var model = new StudentResultsViewModel()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allResultsCount = await this.resultService.GetResultsCountByStudentIdAsync(studentId, searchCriteria, searchText);
            if (allResultsCount > 0)
            {
                var results = await this.resultService.GetPerPageByStudentIdAsync<StudentResultViewModel>(studentId, page, countPerPage, searchCriteria, searchText);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var result in results)
                {
                    result.Date = this.dateTimeConverter.GetDate(result.EventActivationDateAndTime, timeZoneIana);
                }

                model.PagesCount = (int)Math.Ceiling(allResultsCount / (decimal)countPerPage);
                model.Results = results;
            }

            return this.View(model);
        }

        public async Task<IActionResult> StudentActiveEventsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var model = new StudentEventsViewModel<StudentActiveEventViewModel>()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allActiveEventsCount = await this.eventsService.GetEventsCountByStudentIdAndStatusAsync(studentId, Status.Active, searchCriteria, searchText);
            if (allActiveEventsCount > 0)
            {
                var activeEvents = await this.eventsService
               .GetPerPageByStudentIdFilteredByStatusAsync<StudentActiveEventViewModel>(
                    Status.Active, studentId, page, countPerPage, false, searchCriteria, searchText);

                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var activeEvent in activeEvents)
                {
                    activeEvent.Duration = this.dateTimeConverter.GetDurationString(activeEvent.ActivationDateAndTime, activeEvent.DurationOfActivity, timeZoneIana);
                }

                model.PagesCount = (int)Math.Ceiling(allActiveEventsCount / (decimal)countPerPage);
                model.Events = activeEvents;
            }

            return this.View(model);
        }

        public async Task<IActionResult> StudentEndedEventsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var model = new StudentEventsViewModel<StudentEndedEventViewModel>()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allEndedEventsCount = await this.eventsService.GetEventsCountByStudentIdAndStatusAsync(studentId, Status.Ended, searchCriteria, searchText);
            if (allEndedEventsCount > 0)
            {
                var endedEvents = await this.eventsService
               .GetPerPageByStudentIdFilteredByStatusAsync<StudentEndedEventViewModel>(
                    Status.Ended, studentId, page, countPerPage, true, searchCriteria, searchText);
                var scores = await this.resultService.GetAllByStudentIdAsync<ScoreViewModel>(studentId);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];

                foreach (var endenEvent in endedEvents)
                {
                    if (endenEvent.QuizName == null)
                    {
                        endenEvent.QuizName = await this.resultService.GetQuizNameByEventIdAndStudentIdAsync(endenEvent.Id, studentId);
                    }

                    endenEvent.Score = scores.FirstOrDefault(x => x.EventId == endenEvent.Id);
                    endenEvent.Date = this.dateTimeConverter.GetDate(endenEvent.ActivationDateAndTime, timeZoneIana);
                }

                model.PagesCount = (int)Math.Ceiling(allEndedEventsCount / (decimal)countPerPage);
                model.Events = endedEvents;
            }

            return this.View(model);
        }

        public async Task<IActionResult> StudentPendingEventsAll(string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var model = new StudentEventsViewModel<StudentPendingEventViewModel>()
            {
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allPendingEventsCount = await this.eventsService.GetEventsCountByStudentIdAndStatusAsync(studentId, Status.Pending, searchCriteria, searchText);
            if (allPendingEventsCount > 0)
            {
                var pendingEvents = await this.eventsService
               .GetPerPageByStudentIdFilteredByStatusAsync<StudentPendingEventViewModel>(
                    Status.Pending, studentId, page, countPerPage, false, searchCriteria, searchText);

                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var pendingEvent in pendingEvents)
                {
                    pendingEvent.Duration = this.dateTimeConverter.GetDurationString(pendingEvent.ActivationDateAndTime, pendingEvent.DurationOfActivity, timeZoneIana);
                    pendingEvent.Date = this.dateTimeConverter.GetDate(pendingEvent.ActivationDateAndTime, timeZoneIana);
                }

                model.PagesCount = (int)Math.Ceiling(allPendingEventsCount / (decimal)countPerPage);
                model.Events = pendingEvents;
            }

            return this.View(model);
        }
    }
}
