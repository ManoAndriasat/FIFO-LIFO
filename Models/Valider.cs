using System;
using System.Collections.Generic;
using Npgsql;

namespace Element
{
    public class Valider : Mouvement
    {
        private DateTime date_validation;

        public DateTime Date_validation
        {
            get { return date_validation; }
            set { date_validation = value; }
        }

        public Valider()
        {
        }

        public Valider(string id_mouvement, string id_article, DateTime date_mouvement, string id_magasin, double quantite, DateTime date_validation)
            : base(id_mouvement, "-", id_article, date_mouvement, id_magasin, quantite)
        {
            this.Date_validation = date_validation;
        }

        public Valider(string id_mouvement, string id_article, string date_mouvement, string id_magasin, string quantite, string date_validation)
            : this(id_mouvement, id_article, Convert.ToDateTime(date_mouvement), id_magasin, Convert.ToDouble(quantite), Convert.ToDateTime(date_validation))
        {
        }

        public void Insert(NpgsqlConnection connection)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO valider (id_mouvement, id_article, date_mouvement, id_magasin, quantite, date_validation ) VALUES (@id_mouvement, @id_article, @date_mouvement, @id_magasin, @quantite, @date_validation)";

                cmd.Parameters.AddWithValue("@id_mouvement", this.Id_mouvement);
                cmd.Parameters.AddWithValue("@id_article", this.Id_article);
                cmd.Parameters.AddWithValue("@date_mouvement", this.Date_mouvement);
                cmd.Parameters.AddWithValue("@id_magasin", this.Id_magasin);
                cmd.Parameters.AddWithValue("@quantite", this.Quantite);
                cmd.Parameters.AddWithValue("@date_validation", this.Date_validation);

                cmd.ExecuteNonQuery();
            }
        }

    }
}