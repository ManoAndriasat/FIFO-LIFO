using System;
using System.Collections.Generic;
using Npgsql;

namespace Element
{
    public class MouvementStock
    {
        private SortieStock sortie_stock;
        private string receveur;

        public SortieStock Sortie_stock
        {
            get { return sortie_stock; }
            set { sortie_stock = value; }
        }

        public string Receveur
        {
            get { return receveur; }
            set
            {
                if (value == sortie_stock.Id_magasin)
                {
                    throw new ArgumentException("receveur ne peut pas avoir la même valeur que id_magasin.");
                }
                receveur = value;
            }
        }
        
        public MouvementStock(SortieStock sortie_stock,string receveur)
        {
            this.Sortie_stock = sortie_stock;
            this.Receveur = receveur;
        }


        public void insert_mouvement_stock(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO mouvement_stock VALUES (@id_sortie, @receveur)";

                cmd.Parameters.AddWithValue("@id_sortie", this.sortie_stock.Id_sortie);
                cmd.Parameters.AddWithValue("@receveur", this.receveur);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<MouvementStock> get_list_mouvement_stock(NpgsqlConnection conn){

            List<MouvementStock> mouvementStocks = new List<MouvementStock>();

            using (NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM mouvement_stock", conn))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SortieStock sortieStock = SortieStock.select_sortie_stock_by_id(conn, reader["id_sortie"].ToString())[0];
                        string receveur = reader["receveur"].ToString();
                        MouvementStock mouvementStock = new MouvementStock(sortieStock, receveur);
                        mouvementStocks.Add(mouvementStock);
                    }
                }
            }

            return mouvementStocks;
        }
    }
}
