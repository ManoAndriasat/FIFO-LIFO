using System;
using System.Collections.Generic;
using Npgsql;

namespace Element
{
    public class Mouvement
    {
        private string id_mouvement;
        private string id_entrer;
        private string id_article;
        private DateTime date_mouvement;
        private string id_magasin;
        private double quantite;

        public string Id_mouvement
        {
            get { return id_mouvement; }
            set { id_mouvement = value; }
        }

        public string Id_entrer
        {
            get { return id_entrer; }
            set { id_entrer = value; }
        }

        public string Id_article
        {
            get { return id_article; }
            set { id_article = value; }
        }

        public DateTime Date_mouvement
        {
            get { return date_mouvement; }
            set { date_mouvement = value; }
        }

        public string Id_magasin
        {
            get { return id_magasin; }
            set { id_magasin = value; }
        }

        public double Quantite
        {
            get { return quantite; }
            set {
                if (value < 0){
                    throw new ArgumentException("La quantité ne peut pas être négative.");
                }
                quantite = value;
            }
        }

        public Mouvement()
        {
        }

        public Mouvement(string id_mouvement, string id_entrer, string id_article, DateTime date_mouvement, string id_magasin, double quantite)
        {
            this.Id_mouvement = id_mouvement;
            this.Id_entrer = id_entrer;
            this.Id_article = id_article;
            this.Date_mouvement = date_mouvement;
            this.Id_magasin = id_magasin;
            this.Quantite = quantite;

        }

        public Mouvement(string id_mouvement, string id_entrer, string id_article, string date_mouvement, string id_magasin, string quantite)
            : this(id_mouvement, id_entrer, id_article, Convert.ToDateTime(date_mouvement), id_magasin, Convert.ToDouble(quantite))
        {}


        public void InsertMouvement(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO sortie_stock VALUES (@id_mouvement, @id_entrer, @id_magasin, @id_article,  @date_mouvement , @quantite)";

                cmd.Parameters.AddWithValue("@id_mouvement", this.Id_mouvement);
                cmd.Parameters.AddWithValue("@id_entrer", this.Id_entrer);
                cmd.Parameters.AddWithValue("@id_article", this.Id_article);
                cmd.Parameters.AddWithValue("@date_mouvement", this.Date_mouvement);
                cmd.Parameters.AddWithValue("@id_magasin", this.Id_magasin);
                cmd.Parameters.AddWithValue("@quantite", this.Quantite);

                cmd.ExecuteNonQuery();
            }
        }


        public void InsertEntrer(NpgsqlConnection conn , double prixUnitaire)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO entrer_stock VALUES (@id_mouvement, @id_magasin, @id_article,  @date_mouvement , @quantite, "+ prixUnitaire +")";

                cmd.Parameters.AddWithValue("@id_mouvement", this.Id_mouvement);
                cmd.Parameters.AddWithValue("@id_article", this.Id_article);
                cmd.Parameters.AddWithValue("@date_mouvement", this.Date_mouvement);
                cmd.Parameters.AddWithValue("@id_magasin", this.Id_magasin);
                cmd.Parameters.AddWithValue("@quantite", this.Quantite);

                cmd.ExecuteNonQuery();
            }
        }

        public static void CompareDate(DateTime one , DateTime two){
            if(one<two){
                throw new ArgumentException("Tsy mety le date Anterieur");
            }
        }

        public static Mouvement DernierMouvement(string articleId, string magasinId, NpgsqlConnection connection)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM sortie_stock " +
                                    "WHERE id_article = @articleId AND id_magasin = @magasinId " +
                                    "ORDER BY date_mouvement DESC LIMIT 1";

                cmd.Parameters.AddWithValue("@articleId", articleId);
                cmd.Parameters.AddWithValue("@magasinId", magasinId);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapToMouvement(reader);
                    }
                }
            }
            return null;
        }

        public Mouvement[] Decomposer(List<EtatStock> ResteStock, NpgsqlConnection connection)
        {
            double sortie = this.Quantite; 
            List<Mouvement> mouvements = new List<Mouvement>();
            foreach (EtatStock es in ResteStock)
            {
                double reste = es.Quantite_final;
                if(reste - sortie >= 0){
                    Mouvement mvt = new Mouvement(this.Id_mouvement, es.Id_mouvement, this.Id_article, this.Date_mouvement, this.Id_magasin, sortie);
                    mouvements.Add(mvt);
                    return mouvements.ToArray();
                }
                else{
                    Mouvement mvt = new Mouvement(this.Id_mouvement, es.Id_mouvement, this.Id_article, this.Date_mouvement, this.Id_magasin, reste);
                    mouvements.Add(mvt);
                    sortie = sortie - reste;
                }
            }
            return mouvements.ToArray();
        }

        private static Mouvement MapToMouvement(NpgsqlDataReader reader)
        {
            return new Mouvement
            {
                Id_mouvement = reader["id_mouvement"].ToString(),
                Id_entrer = reader["id_entrer"].ToString(),
                Id_article = reader["id_article"].ToString(),
                Date_mouvement = Convert.ToDateTime(reader["date_mouvement"]),
                Id_magasin = reader["id_magasin"].ToString(),
                Quantite = Convert.ToDouble(reader["quantite"])
            };
        }
    }
}
