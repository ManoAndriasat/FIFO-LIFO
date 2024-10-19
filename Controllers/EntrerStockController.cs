using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Postgres;
using Element;

public class EntrerStockController : Controller
{
    private NpgsqlConnection connection;

    public EntrerStockController()
    {
        connection = new Connection().GetConnection();
    }

    public IActionResult InsertionEntrer(string error)
    {
        if (!string.IsNullOrEmpty(error)){
            ViewBag.Error = error;
        }

        ViewBag.ListArticle = Article.AllArticle(connection);
        ViewBag.ListMagasin = Magasin.AllMagasin(connection);
        return View();
    }

    [HttpPost]
    public IActionResult Insert(IFormCollection formCollection)
    {
         using (var transaction = connection.BeginTransaction())
        {
            try
            {
                string id_Entrer = EntrerStock.get_next_id(connection);
                string categorie = formCollection["categorie"];
                string id_Magasin = formCollection["id_magasin"];
                DateTime date_entrer = Convert.ToDateTime(formCollection["date_entrer"]);
                double quantite = Convert.ToDouble(formCollection["quantite"]);
                double prix_unitaire = Convert.ToDouble(formCollection["prix_unitaire"]);

                EntrerStock objet = new EntrerStock(id_Entrer, categorie, id_Magasin, date_entrer, quantite, prix_unitaire);
                ResteStock reste = new ResteStock(objet, quantite);
                objet.insert_entrer_stock(connection);
                reste.insert_reste_stock(connection);
                transaction.Commit();

            }catch (Exception ex){
                transaction.Rollback();
                return RedirectToAction("InsertionEntrer", "EntrerStock", new { error = ex.Message });
            }

        }

        return RedirectToAction("InsertionEntrer", "EntrerStock");
    }
}
