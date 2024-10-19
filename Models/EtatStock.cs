using Npgsql;
using System;
using System.Collections.Generic;
using Utils;

namespace Element
{
    public class EtatStock
    {
        public string Id_mouvement { get; set; }
        public DateTime Date_mouvement { get; set; }
        public string Id_article { get; set; }
        public string Id_magasin { get; set; }
        public double Quantite_initial { get; set; }
        public double Quantite_final { get; set; }
        public double Prix_unitaire { get; set; }
   
        public static List<EtatStock> EtatStockGlobal(string id_magasin, NpgsqlConnection connection)
        {
            List<EtatStock> etatStocks = new List<EtatStock>();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM etat_stock where quantite_final > 0 AND id_magasin= '" + id_magasin + "'";

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EtatStock result = new EtatStock
                        {
                            Id_magasin = reader["ID_magasin"].ToString(),
                            Id_article = reader["id_article"].ToString(),
                            Quantite_initial = Convert.ToDouble(reader["quantite_initial"]),
                            Quantite_final = Convert.ToDouble(reader["quantite_final"]),
                            Prix_unitaire = Convert.ToDouble(reader["pump"])
                        };

                        etatStocks.Add(result);
                    }
                }
            }

            return etatStocks;
        }

        public static double TotalVola(List<EtatStock> liste){
            double total = 0;
            foreach(EtatStock es in liste){
                total += es.Quantite_final * es.Prix_unitaire;
            }
            return total;
        }

        public static List<EtatStock> ResteStock(string magasinId, string articleId, string typeMouvement , NpgsqlConnection connection)
        {
            List<EtatStock> etatStockList = new List<EtatStock>();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;

                string orderClause = Article.TypeMouvement(typeMouvement);

                cmd.CommandText = $"SELECT * FROM etat_stock_details WHERE id_magasin = @magasinId AND id_article = @articleId {orderClause}";

                cmd.Parameters.AddWithValue("@magasinId", magasinId);
                cmd.Parameters.AddWithValue("@articleId", articleId);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        EtatStock etatStock = MapToEtatStock(reader);
                        etatStockList.Add(etatStock);
                    }
                }
            }

            return etatStockList;
        }

        private static EtatStock MapToEtatStock(NpgsqlDataReader reader)
        {
            return new EtatStock
            {
                Id_mouvement = reader["id_mouvement"].ToString(),
                Date_mouvement = Convert.ToDateTime(reader["date_mouvement"]),
                Id_article = reader["id_article"].ToString(),
                Id_magasin = reader["id_magasin"].ToString(),
                Quantite_initial = Convert.ToDouble(reader["quantite_initial"]),
                Quantite_final = Convert.ToDouble(reader["quantite_final"]),
                Prix_unitaire = Convert.ToDouble(reader["prix_unitaire"])
            };
        }

        public static void ChangeDate(NpgsqlConnection conn, DateTime startDate, DateTime endDate)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = "CREATE OR REPLACE VIEW entrer_stock_vue AS " +
                                "SELECT * FROM entrer_stock WHERE date_mouvement < '" + startDate + "' AND date_mouvement > '" + endDate + "'";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE OR REPLACE VIEW sortie_stock_vue AS " +
                                "SELECT * FROM sortie_stock WHERE date_mouvement < '" + startDate + "' AND date_mouvement > '" + endDate + "'";
                cmd.ExecuteNonQuery();
            }
        }


        public static void ToDateNow(NpgsqlConnection conn)
        {
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;

                cmd.CommandText = "CREATE OR REPLACE VIEW entrer_stock_vue AS " +
                                "SELECT * FROM entrer_stock";
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();


                cmd.CommandText = "CREATE OR REPLACE VIEW sortie_stock_vue AS " +
                          "SELECT * FROM sortie_stock";
        
                cmd.ExecuteNonQuery();
            }
        }
    }
}
