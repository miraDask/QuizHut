namespace QuizHut.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using QuizHut.Data.Models;
    using QuizHut.Services.Users;
    using QuizHut.Web.Infrastructure.Filters;
    using QuizHut.Web.ViewModels.Students;

    public class StudentsController : AdministrationController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUsersService service;

        public StudentsController(UserManager<ApplicationUser> userManager, IUsersService service)
        {
            this.userManager = userManager;
            this.service = service;
        }

        public async Task<IActionResult> AllStudentsAddedByTeacher(string invalidEmail)
        {
            var userId = this.userManager.GetUserId(this.User);
            var students = await this.service.GetAllByUserIdAsync<StudentViewModel>(userId);
            var model = new AllStudentsAddedByTeacherViewModel()
            {
                Students = students,
                NewStudent = new UserInputViewModel(),
            };

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
            await this.service.DeleteAsync(id, userId);
            return this.RedirectToAction("AllStudentsAddedByTeacher");
        }
    }
}
