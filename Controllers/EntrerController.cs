using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Utils;
using Element;
using System;

public class EntrerController : Controller
{
    private readonly NpgsqlConnection connection;

    public EntrerController()
    {
        connection = new Connection().GetConnection();
    }

    public IActionResult Index(string error)
    {
        ViewBag.ListeUnite = Unite.AllUnite(connection);
        ViewBag.ListArticle = Article.AllArticle(connection);
        ViewBag.ListMagasin = Magasin.AllMagasin(connection);
        return View();
    }

    
    [HttpPost]
    public IActionResult Insert(string id_magasin, DateTime date_mouvement, double quantite, string id_article,double prix_unitaire,string unite)
    {
        Unite ValeurUnite = Unite.GetUnite(connection, id_article,unite)[0];
        string id_mouvement = DatabaseHelper.GetNextId(connection,"id_entrer","ENT");
        Unite.InsertReel(connection,id_mouvement , unite , quantite , 10 ,1);
        
        Mouvement entre = new Mouvement(id_mouvement,"",id_article,date_mouvement,id_magasin,quantite*ValeurUnite.Quantite);
        entre.InsertEntrer(connection , prix_unitaire/ValeurUnite.Quantite);
        return RedirectToAction("Index", "Entrer");
    }

}
