namespace QuizHut.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
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

        public async Task<IActionResult> Results(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var allResultsCount = this.resultService.GetResultsCountByStudentId(studentId);
            int pagesCount = 0;
            var model = new StudentResultsViewModel()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allResultsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allResultsCount / (decimal)countPerPage);
                var results = await this.resultService.GetPerPageByStudentIdAsync<StudentResultViewModel>(studentId, page, countPerPage);
                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var result in results)
                {
                    result.Date = this.dateTimeConverter.GetDate(result.EventActivationDateAndTime, timeZoneIana);
                }

                model.PagesCount = pagesCount;
                model.Results = results;
            }

            return this.View(model);
        }

        public async Task<IActionResult> StudentActiveEventsAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var allActiveEventsCount = this.eventsService.GetEventsCountByStudentIdAndStatus(studentId, Status.Active);
            int pagesCount = 0;
            var model = new StudentEventsViewModel<StudentActiveEventViewModel>()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allActiveEventsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allActiveEventsCount / (decimal)countPerPage);
                var activeEvents = await this.eventsService
               .GetPerPageByStudentIdFilteredByStatusAsync<StudentActiveEventViewModel>(Status.Active, studentId, page, countPerPage, false);

                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var activeEvent in activeEvents)
                {
                    activeEvent.Duration = this.dateTimeConverter.GetDurationString(activeEvent.ActivationDateAndTime, activeEvent.DurationOfActivity, timeZoneIana);
                }

                model.PagesCount = pagesCount;
                model.Events = activeEvents;
            }

            return this.View(model);
        }

        public async Task<IActionResult> StudentEndedEventsAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var allEndedEventsCount = this.eventsService.GetEventsCountByStudentIdAndStatus(studentId, Status.Ended);
            int pagesCount = 0;
            var model = new StudentEventsViewModel<StudentEndedEventViewModel>()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allEndedEventsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allEndedEventsCount / (decimal)countPerPage);
                var endedEvents = await this.eventsService
               .GetPerPageByStudentIdFilteredByStatusAsync<StudentEndedEventViewModel>(Status.Ended, studentId, page, countPerPage, true);
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

                model.PagesCount = pagesCount;
                model.Events = endedEvents;
            }

            return this.View(model);
        }

        public async Task<IActionResult> StudentPendingEventsAll(int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var studentId = this.userManager.GetUserId(this.User);
            var allPendingEventsCount = this.eventsService.GetEventsCountByStudentIdAndStatus(studentId, Status.Pending);
            int pagesCount = 0;
            var model = new StudentEventsViewModel<StudentPendingEventViewModel>()
            {
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allPendingEventsCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allPendingEventsCount / (decimal)countPerPage);
                var pendingEvents = await this.eventsService
               .GetPerPageByStudentIdFilteredByStatusAsync<StudentPendingEventViewModel>(Status.Pending, studentId, page, countPerPage, false);

                var timeZoneIana = this.Request.Cookies[GlobalConstants.Coockies.TimeZoneIana];
                foreach (var pendingEvent in pendingEvents)
                {
                    pendingEvent.Duration = this.dateTimeConverter.GetDurationString(pendingEvent.ActivationDateAndTime, pendingEvent.DurationOfActivity, timeZoneIana);
                    pendingEvent.Date = this.dateTimeConverter.GetDate(pendingEvent.ActivationDateAndTime, timeZoneIana);
                }

                model.PagesCount = pagesCount;
                model.Events = pendingEvents;
            }

            return this.View(model);
        }
    }
}
