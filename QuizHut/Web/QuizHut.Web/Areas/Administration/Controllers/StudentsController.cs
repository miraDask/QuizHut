namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Users;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Students;

    public class StudentsController : AdministrationController
    {
        private const int PerPageDefaultValue = 5;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService service;

        public StudentsController(UserManager<ApplicationUser> userManager, IUsersService service)
        {
            this.userManager = userManager;
            this.service = service;
        }

        [ClearDashboardRequestInSessionActionFilterAttribute]
        public async Task<IActionResult> AllStudentsAddedByTeacher(string invalidEmail, string searchText, string searchCriteria, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);

            var model = new AllStudentsAddedByTeacherViewModel()
            {
                NewStudent = new StudentInputViewModel(),
                CurrentPage = page,
                PagesCount = 0,
                SearchType = searchCriteria,
                SearchString = searchText,
            };

            var allStudentsAddedByTeacherCount = await this.service.GetAllStudentsCountAsync(userId, searchCriteria, searchText);
            if (allStudentsAddedByTeacherCount > 0)
            {
                var students = await this.service.GetAllStudentsPerPageAsync<StudentViewModel>(page, countPerPage, userId, searchCriteria, searchText);
                model.Students = students;
                model.PagesCount = (int)Math.Ceiling(allStudentsAddedByTeacherCount / (decimal)countPerPage);
            }

            if (invalidEmail != null)
            {
                model.NewStudent.IsNotAdded = true;
                model.NewStudent.Email = invalidEmail;
            }

            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilterAttribute]
        public async Task<IActionResult> AllStudentsAddedByTeacher(AllStudentsAddedByTeacherViewModel model)
        {
            var userId = this.userManager.GetUserId(this.User);
            var partisipantIsAdded = await this.service.AddStudentAsync(model.NewStudent.Email, userId);

            if (!partisipantIsAdded)
            {
                return this.RedirectToAction("AllStudentsAddedByTeacher", new { invalidEmail = model.NewStudent.Email });
            }

            return this.RedirectToAction("AllStudentsAddedByTeacher");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var userId = this.userManager.GetUserId(this.User);
            await this.service.DeleteFromTeacherListAsync(id, userId);
            return this.RedirectToAction("AllStudentsAddedByTeacher");
        }
    }
}
