﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExampleServer.Controllers
{
    public class HomeController : Controller
    {
        private ILogger logger;

        public HomeController(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public IActionResult Index()
        {
            logger.LogInformation("In Index of the HomeController", null);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [Authorize]
        public IActionResult Configuration([FromServices] IConfiguration service)
        {
            ViewBag.Configuration = service.AsEnumerable();
            return View();
        }
    }
}
