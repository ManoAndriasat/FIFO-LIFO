// ErrorController.cs

using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    public IActionResult Error(string Error)
    {
        ViewBag.error = Error;
        return View();
    }
}
