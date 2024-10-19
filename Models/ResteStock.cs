using System;
using Npgsql;
using Postgres;
using System.Collections.Generic;

namespace Element
{
    public class ResteStock
    {
        // Attributs
        private EntrerStock entrer_stock;
        private double reste_stock;

        // Propriétés avec getters et setters
        public EntrerStock Entrer_stock
        {
            get { return entrer_stock; }
            set { entrer_stock = value; }
        }

        public double Reste_stock
        {
            get { return reste_stock; }
            set { reste_stock = value; }
        }

        // Constructeur par défaut
        public ResteStock()
        {
        }

        // Constructeur avec paramètres
        public ResteStock(EntrerStock entrer_stock, double reste_stock)
        {
            this.Entrer_stock = entrer_stock;
            this.Reste_stock = reste_stock;
        }

        // Méthode pour insérer des données dans la base de données
        public void insert_reste_stock(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO reste_stock VALUES (@id_entrer,@reste_stock)";

                cmd.Parameters.AddWithValue("@id_entrer", entrer_stock.Id_entrer);
                cmd.Parameters.AddWithValue("@reste_stock", reste_stock);

                cmd.ExecuteNonQuery();
            }
        }

        // Méthode pour obtenir la prochaine valeur d'ID de la séquence
        public static string get_next_id(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT nextval('id_entrer')", conn))
            {
                object result = cmd.ExecuteScalar();
                int sequenceValue = Convert.ToInt32(result);
                return string.Format("ent{0}", sequenceValue);
            }
        }

        public static List<ResteStock> select_reste()
        {
            List<ResteStock> results = new List<ResteStock>();

            using (NpgsqlConnection conn = new Connection().GetConnection())
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    cmd.CommandText = "SELECT * FROM reste_stock where reste_stock > 0";

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ResteStock entry = new ResteStock
                            {
                                Entrer_stock = EntrerStock.select_entrer_stock_by_id(conn,reader["id_entrer"].ToString())[0],
                                Reste_stock = Convert.ToDouble(reader["reste_stock"])
                            };
                            results.Add(entry);
                        }
                    }
                }
            } 

            return results;
        }


        public static double total_reste(List<ResteStock> resteStockList, string categorie, string magasin)
        {
            List<ResteStock> filteredList = resteStockList
                .Where(r => r.Entrer_stock.Categorie.StartsWith(categorie) && r.Entrer_stock.Id_magasin == magasin)
                .ToList();

            double totalResteStock = filteredList.Sum(r => r.Reste_stock);
            return totalResteStock;
        }

        public static void compare(double quantite, double reste)
        {
            if (quantite > reste)
            {
                throw new ArgumentException("La quantité demandée est supérieure au reste en stock.");
            }
        }

    }
}
