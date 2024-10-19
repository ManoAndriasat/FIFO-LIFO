using System;
using Npgsql;

namespace Element
{
    public class EntrerStock
    {
        // Attributs
        private string id_entrer;
        private string categorie;
        private string id_magasin;
        private DateTime date_entrer;
        private double quantite;
        private double prix_unitaire;

        public string Id_entrer
        {
            get { return id_entrer; }
            set { id_entrer = value; }
        }

        public string Categorie
        {
            get { return categorie; }
            set { categorie = value; }
        }

        public string Id_magasin
        {
            get { return id_magasin; }
            set { id_magasin = value; }
        }

        public DateTime Date_entrer
        {
            get { return date_entrer; }
            set { date_entrer = value; }
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
        public EntrerStock()
        {
        }

        // Constructeur avec paramètres
        public EntrerStock(string id_entrer, string categorie, string id_magasin, DateTime date_entrer, double quantite, double prix_unitaire)
        {
            this.Id_entrer = id_entrer;
            this.categorie = categorie;
            this.Id_magasin = id_magasin;
            this.Date_entrer = date_entrer;
            this.Quantite = quantite;
            this.Prix_unitaire = prix_unitaire;
        }

        public void insert_entrer_stock(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO entrer_stock (id_entrer, categorie, id_magasin, date_entrer, quantite, prix_unitaire) " +
                                "VALUES (@id_entrer, @categorie, @id_magasin, @date_entrer, @quantite, @prix_unitaire)";

                cmd.Parameters.AddWithValue("@id_entrer", this.id_entrer);
                cmd.Parameters.AddWithValue("@categorie", this.categorie);
                cmd.Parameters.AddWithValue("@id_magasin", this.id_magasin);
                cmd.Parameters.AddWithValue("@date_entrer", this.date_entrer);
                cmd.Parameters.AddWithValue("@quantite", this.quantite);
                cmd.Parameters.AddWithValue("@prix_unitaire", this.prix_unitaire);
                cmd.ExecuteNonQuery();
            }
        }

        public static string get_next_id(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT nextval('id_entrer')", conn))
            {
                object result = cmd.ExecuteScalar();
                int sequenceValue = Convert.ToInt32(result);
                return string.Format("ent{0}", sequenceValue);
            }
        }

        public static List<EntrerStock> select_entrer_stock_by_id(NpgsqlConnection conn, string id_entrer)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id_entrer", id_entrer },
            };
            return Generalisation.Select<EntrerStock>(conn, "entrer_stock", parameters);
        }
    }
}
