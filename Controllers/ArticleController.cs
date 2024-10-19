using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Postgres;
using Element;
using Npgsql;

public class ArticleController : Controller
{
    private NpgsqlConnection connection;

    public ArticleController()
    {
        connection = new Connection().GetConnection();
    }

    public IActionResult InsertionArticle()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Insert(IFormCollection formCollection)
    {
        return RedirectToAction("InsertionArticle", "Article");
    }
    
}
