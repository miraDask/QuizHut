using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizHut.Common;
using QuizHut.Web.ViewModels.Quizzes;

namespace QuizHut.Web.Controllers
{
    public class StudentsController : Controller
    {

        public IActionResult Index(string password, string errorText)
        {
            var model = new PasswordInputViewModel();
            if (password != null)
            {
                model.Error = GlobalConstants.ErrorMessages.EmptyPasswordField;
            }

            model.Password = password;
            model.Error = errorText;

            return this.View(model);
        }
    }
}