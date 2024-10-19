using System;
using System.Collections.Generic;
using Npgsql;

namespace Element
{
    public class SortieStock
    {
        // Attributs
        private string id_sortie;
        private string categorie;
        private string id_magasin;
        private DateTime date_sortie;
        private double quantite;
        private double prix_unitaire;

        // Propriétés avec getters et setters
        public string Id_sortie
        {
            get { return id_sortie; }
            set { this.id_sortie = value; }
        }

        public string Categorie
        {
            get { return categorie; }
            set { this.categorie = value; }
        }

        public string Id_magasin
        {
            get { return id_magasin; }
            set { this.id_magasin = value; }
        }

        public DateTime Date_sortie
        {
            get { return date_sortie; }
            set { this.date_sortie = value; }
        }

        public double Quantite
        {
            get { return quantite; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("La quantité ne peut pas être négative.");
                }
                quantite = value;
            }
        }

        public double Prix_unitaire
        {
            get { return prix_unitaire; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Le prix unitaire ne peut pas être négatif.");
                }
                prix_unitaire = value;
            }
        }

        // Constructeur par défaut
        public SortieStock()
        {
        }

        // Constructeur avec paramètres
        public SortieStock(string id_sortie, string categorie, string id_magasin, DateTime date_sortie, double quantite, double prix_unitaire)
        {
            this.Id_sortie = id_sortie;
            this.Categorie = categorie;
            this.Id_magasin = id_magasin;
            this.Date_sortie = date_sortie;
            this.Quantite = quantite;
            this.Prix_unitaire = prix_unitaire;
        }

        public void insert_sortie_stock(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO sortie_stock (id_sortie, categorie, id_magasin, date_sortie, quantite, prix_unitaire) " +
                                "VALUES (@id_sortie, @categorie, @id_magasin, @date_sortie, @quantite, @prix_unitaire)";

                cmd.Parameters.AddWithValue("@id_sortie", this.id_sortie);
                cmd.Parameters.AddWithValue("@categorie", this.categorie);
                cmd.Parameters.AddWithValue("@id_magasin", this.id_magasin);
                cmd.Parameters.AddWithValue("@date_sortie", this.date_sortie);
                cmd.Parameters.AddWithValue("@quantite", this.quantite);
                cmd.Parameters.AddWithValue("@prix_unitaire", this.prix_unitaire);

                cmd.ExecuteNonQuery();
            }
        }

        public static string etat(string etat)
        {
            if (etat == "LIFO")
            {
                return "ORDER BY date_sortie DESC";
            }
            else
            {
                return "ORDER BY date_sortie ASC";
            }
        }

              
        public static string get_next_id(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT nextval('id_sortie')", conn))
            {
                object result = cmd.ExecuteScalar();
                int sequenceValue = Convert.ToInt32(result);
                return string.Format("sortie{0}", sequenceValue);
            }
        }

        public static List<SortieStock> select_sortie_stock_by_id(NpgsqlConnection conn, string id_sortie)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id_sortie", id_sortie },
            };
            return Generalisation.Select<SortieStock>(conn, "sortie_stock", parameters);
        }

    }
}
