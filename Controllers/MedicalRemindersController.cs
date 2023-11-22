using Microsoft.AspNetCore.Mvc;

namespace App_Xamarin_Firebase.Controllers
{
    public class MedicalReminders : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
