using Microsoft.AspNetCore.Mvc;

namespace Todo.WebApi.Controllers.Identity;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
