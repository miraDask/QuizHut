using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace QuizHut.Web.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Profile()
        {
            //TODO
            return View();
        }

        public IActionResult Logout()
        {
            //TODO
            return View();
        }
    }
}
