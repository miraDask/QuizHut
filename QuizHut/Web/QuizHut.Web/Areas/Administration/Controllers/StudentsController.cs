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
        public async Task<IActionResult> AllStudentsAddedByTeacher(string invalidEmail, int page = 1, int countPerPage = PerPageDefaultValue)
        {
            var userId = this.userManager.GetUserId(this.User);
            var allStudentsAddedByTeacherCount = this.service.GetAllStudentsCount(userId);
            int pagesCount = 0;

            var model = new AllStudentsAddedByTeacherViewModel()
            {
                NewStudent = new StudentInputViewModel(),
                CurrentPage = page,
                PagesCount = pagesCount,
            };

            if (allStudentsAddedByTeacherCount > 0)
            {
                pagesCount = (int)Math.Ceiling(allStudentsAddedByTeacherCount / (decimal)countPerPage);
                var students = await this.service.GetAllPerPage<StudentViewModel>(page, countPerPage, userId);
                model.Students = students;
                model.PagesCount = pagesCount;
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
