using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Utils;
using Element;
using System;

public class ValidationController : Controller
{
    private readonly NpgsqlConnection connection;

    public ValidationController()
    {
        connection = new Connection().GetConnection();
    }

    public IActionResult Refuser(string id_mouvement)
    {
        Brouillard.UpdateEtat(connection, id_mouvement, 10);
        return RedirectToAction("listeBrouillard", "Brouillard");
    }

    [HttpPost]
    public IActionResult Valider(string id_mouvement, DateTime date_validation, string id_article, string id_magasin, string date_mouvement, double quantite)
    {
        using (var transaction = connection.BeginTransaction())
        {
            try
            {
                Article article = Article.GetArticleByID(connection, id_article)[0];
                DateTime DateMouvement = Convert.ToDateTime(date_mouvement);

                // controle date mouvement
                Mouvement dernier = Mouvement.DernierMouvement(id_article,id_magasin,connection);
                if(dernier != null) { Mouvement.CompareDate(DateMouvement , dernier.Date_mouvement); }

                //controle date validation
                Mouvement.CompareDate(date_validation , DateMouvement);

                //controle quantite restante
                List<EtatStock> resteStock = EtatStock.ResteStock(id_magasin, id_article, article.Type_mouvement, connection);
                Article.Compare(quantite,Article.TotalReste(resteStock));

                Valider valider = new Valider(id_mouvement, id_article, DateMouvement, id_magasin, quantite, date_validation);
                Mouvement[] mouvements = valider.Decomposer(resteStock, connection);

                Unite.UpdateReel(connection,id_mouvement,10);

                Brouillard.UpdateEtat(connection, id_mouvement, 20);
                valider.Insert(connection);
                foreach (Mouvement mouvement in mouvements){mouvement.InsertMouvement(connection);}

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return RedirectToAction("listeBrouillard", "Brouillard", new { error = ex.Message });       
            }
        }

        return RedirectToAction("listeBrouillard", "Brouillard");
    }
}
