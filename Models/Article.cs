
using System;
using System.Collections.Generic;
using Npgsql;
using Utils;

namespace Element
{
    public class Article
    {
        private string id_article;
        private string categorie;
        private string nom;
        private string type_mouvement;
        private string unite;

        public string Id_article
        {
            get { return id_article; }
            set { this.id_article = value; }
        }

        public string Categorie
        {
            get { return categorie; }
            set { this.categorie = value; }
        }

        public string Nom
        {
            get { return nom; }
            set { this.nom = value; }
        }

        public string Type_mouvement
        {
            get { return type_mouvement; }
            set { this.type_mouvement = value.ToUpper(); }
        }

        public string Unite
        {
            get { return unite; }
            set { this.unite = value; }
        }

        public Article()
        {
        }

        public Article(string idArticle, string categorie, string nom, string type_mouvement, string unite)
        {
            id_article = idArticle;
            this.categorie = categorie;
            this.nom = nom;
            this.type_mouvement = type_mouvement;
            this.unite = unite;
        }

        public static List<Article> AllArticle(NpgsqlConnection conn)
        {
            return DatabaseHelper.Select<Article>(conn, "article");
        }

        public static List<Article> GetArticleByID(NpgsqlConnection conn , string id_article)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id_article", id_article },
            };
            return DatabaseHelper.Select<Article>(conn, "article",parameters);
        }

        public static string TypeMouvement(string TypeMouvement)
        {
            if (TypeMouvement== "LIFO"){
                return "ORDER BY date_mouvement DESC";
            }
            else if(TypeMouvement== "FIFO"){
                return "ORDER BY date_mouvement ASC";
            }else{
                return "";
            }
        }

        public static double TotalReste(List<EtatStock> es)
        {
            double totalStockDetails = es.Sum(r => r.Quantite_final);
            return totalStockDetails;
        }

        public static void Compare(double quantite, double reste)
        {
            if (quantite > reste)
            {
                throw new ArgumentException("La quantité demandée est supérieure au reste en stock.");
            }
        }

        public static string get_next_id(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT nextval('id_article')", conn))
            {
                object result = cmd.ExecuteScalar();
                int sequenceValue = Convert.ToInt32(result);
                return string.Format("ent{0}", sequenceValue);
            }
        }
    }
}