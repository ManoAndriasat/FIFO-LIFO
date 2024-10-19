using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Utils;
using Element;
using System;

public class BrouillardController : Controller
{
    private readonly NpgsqlConnection connection;

    public BrouillardController()
    {
        connection = new Connection().GetConnection();
    }

    public IActionResult Index(string error)
    {
        if(!string.IsNullOrEmpty(error)){
            ViewBag.Error = error;
        }
        ViewBag.ListeUnite = Unite.AllUnite(connection);
        ViewBag.ListArticle = Article.AllArticle(connection);
        ViewBag.ListMagasin = Magasin.AllMagasin(connection);
        return View();
    }

    public IActionResult ListeBrouillard(string error)
    {
        if(!string.IsNullOrEmpty(error)){
            ViewBag.Error = error;
        }
        ViewBag.ListeBrouillard = Brouillard.GetBrouillard(0,connection);
        return View("ListeBrouillard");
    }

    [HttpPost]
    public IActionResult Insert(string id_magasin, DateTime date_mouvement, double quantite, string id_article,string unite)
    {
        try
        {
            Unite ValeurUnite = Unite.GetUnite(connection, id_article,unite)[0];
            if(ValeurUnite == null)
            {
                throw new ArgumentException("tsy mifanaraka unite sy article");
            }
            string id_mouvement = DatabaseHelper.GetNextId(connection,"id_sortie","SRT");
            Brouillard br = new Brouillard(id_mouvement,id_article,date_mouvement,id_magasin,quantite*ValeurUnite.Quantite,0);
            //insert reel
            Unite.InsertReel(connection,id_mouvement , unite ,quantite , 0,2);

            br.InsertBrouillard(connection);

        }catch (Exception ex)
        {
            return RedirectToAction("Index", "Brouillard", new { error = ex.Message });       
        }

        return RedirectToAction("Index", "Brouillard");
    }
}
