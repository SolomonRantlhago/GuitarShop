using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuitarShop.Controllers
{
    public class AdminHome : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
