using System;
using Utils;
using Npgsql;

namespace Element
{
    public class Brouillard : Mouvement
    {
        private double etat;

        public double Etat
        {
            get { return etat; }
            set { etat = value; }
        }

        public Brouillard()
        {
        }

        public Brouillard(string id_mouvement, string id_article, DateTime date_mouvement, string id_magasin, double quantite, double etat)
            : base(id_mouvement, "", id_article, date_mouvement, id_magasin, quantite)
        {
            this.Etat = etat;
        }

        public Brouillard(string id_mouvement, string id_article, string date_mouvement, string id_magasin, string quantite, string etat)
            : this(id_mouvement, id_article, Convert.ToDateTime(date_mouvement), id_magasin, Convert.ToDouble(quantite), Convert.ToDouble(etat))
        {
        }


        public void InsertBrouillard(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO brouillard VALUES (@id_mouvement, @id_article, to_timestamp(@date_mouvement, 'YYYY-MM-DD HH24:MI:SS'), @id_magasin, @quantite, @etat)";

                cmd.Parameters.AddWithValue("@id_mouvement", this.Id_mouvement);
                cmd.Parameters.AddWithValue("@id_article", this.Id_article);
                cmd.Parameters.AddWithValue("@date_mouvement", this.Date_mouvement.ToString("yyyy-MM-dd HH:mm:ss")); // Convert to PostgreSQL-compatible format
                cmd.Parameters.AddWithValue("@id_magasin", this.Id_magasin);
                cmd.Parameters.AddWithValue("@quantite", this.Quantite);
                cmd.Parameters.AddWithValue("@etat", this.Etat);

                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateEtat(NpgsqlConnection conn, string id_mouvement, double newEtat)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE brouillard SET etat = @newEtat WHERE id_mouvement = @id_mouvement";

                cmd.Parameters.AddWithValue("@newEtat", newEtat);
                cmd.Parameters.AddWithValue("@id_mouvement", id_mouvement);

                cmd.ExecuteNonQuery();
            }
        }

        public static List<Brouillard> GetBrouillard(double etat, NpgsqlConnection connection)
        {
            List<Brouillard> brouillards = new List<Brouillard>();

            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                cmd.CommandText = "SELECT * FROM brouillard WHERE etat = @etat";
                cmd.Parameters.AddWithValue("@etat", etat);

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Brouillard brouillard = MapToBrouillard(reader);
                        brouillards.Add(brouillard);
                    }
                }
            }

            return brouillards;
        }

        public void UpdateEtat(NpgsqlConnection conn, double newEtat)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE brouillard SET etat = @newEtat WHERE id_mouvement = @id_mouvement";

                cmd.Parameters.AddWithValue("@newEtat", newEtat);
                cmd.Parameters.AddWithValue("@id_mouvement", this.Id_mouvement);

                cmd.ExecuteNonQuery();

                // Update the current object with the new etat
                this.Etat = newEtat;
            }
        }

        private static Brouillard MapToBrouillard(NpgsqlDataReader reader)
        {
            return new Brouillard
            {
                Id_mouvement = reader["id_mouvement"].ToString(),
                Id_article = reader["id_article"].ToString(),
                Date_mouvement = Convert.ToDateTime(reader["date_mouvement"]),
                Id_magasin = reader["id_magasin"].ToString(),
                Quantite = Convert.ToDouble(reader["quantite"]),
                Etat = Convert.ToDouble(reader["etat"])
            };
        }
    }
}