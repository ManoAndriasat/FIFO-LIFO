using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Postgres;
using Element;
using System;

public class SortieStockController : Controller
{
    private readonly NpgsqlConnection connection;

    public SortieStockController()
    {
        connection = new Connection().GetConnection();
    }

    public IActionResult InsertionSortie(string error)
    {
        if (!string.IsNullOrEmpty(error))
        {
            ViewBag.Error = error;
        }

        ViewBag.ListArticle = Article.AllArticle(connection);
        ViewBag.ListMagasin = Magasin.AllMagasin(connection);
        return View();
    }

    [HttpPost]
    public IActionResult Insert(IFormCollection formCollection)
    {
        List<ResteStock> resteStockList = ResteStock.select_reste();
        
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                string id_Sortie = SortieStock.get_next_id(connection);
                string categorie = formCollection["categorie"];
                string id_Magasin = formCollection["id_magasin"];
                DateTime date_Sortie = Convert.ToDateTime(formCollection["date_Sortie"]);
                double quantite = Convert.ToDouble(formCollection["quantite"]);
                double prix_unitaire = Convert.ToDouble(formCollection["prix_unitaire"]);
                string receveur = formCollection["receveur"];

                SortieStock objet = new SortieStock(id_Sortie, categorie, id_Magasin, date_Sortie, quantite, prix_unitaire);
                MouvementStock mouvement = new MouvementStock(objet, receveur);
                
                double reste = ResteStock.total_reste(resteStockList, categorie, id_Magasin);
                ResteStock.compare(quantite, reste);

                objet.insert_sortie_stock(connection);
                mouvement.insert_mouvement_stock(connection);

                if (receveur != "client")
                {
                    string id_Entrer = EntrerStock.get_next_id(connection);
                    EntrerStock entrerStock = new EntrerStock(id_Entrer, categorie, receveur, date_Sortie, quantite, prix_unitaire);
                    entrerStock.insert_entrer_stock(connection);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return RedirectToAction("InsertionSortie", "SortieStock", new { error = ex.Message });
            }
        }
        return RedirectToAction("InsertionSortie", "SortieStock");
    }
}
