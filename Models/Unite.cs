using Npgsql;
using System;
using System.Collections.Generic;
using Utils;

namespace Element
{
    public class Unite
    {
        public string Id_article { get; set; }
        public double Quantite { get; set; }
        public string Nom { get; set; }

        public Unite(string id_article, double quantite, string nom)
        {
            Id_article = id_article;
            Quantite = quantite;
            Nom = nom;
        }

        public Unite(string id_article, string nom)
        {
            Id_article = id_article;
            Nom = nom;
        }

        public Unite()
        {
        }

        public static List<Unite> GetUnite(NpgsqlConnection conn, string id_article, string nom)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "id_article", id_article },
                { "nom", nom }
            };

            return DatabaseHelper.Select<Unite>(conn, "unite", parameters);
        }

        public static List<string> AllUnite(NpgsqlConnection conn)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT DISTINCT nom FROM unite";

                List<string> names = new List<string>();

                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        names.Add(reader["nom"].ToString());
                    }
                }

                return names;
            }
        }

        public static void InsertReel(NpgsqlConnection conn ,string Id_mouvement , string Nom , double Quantite ,double etat , int mouvement)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO reel VALUES (@id_mouvement, @nom, @quantite , @etat , @mouvement)";

                cmd.Parameters.AddWithValue("@id_mouvement", Id_mouvement);
                cmd.Parameters.AddWithValue("@nom", Nom);
                cmd.Parameters.AddWithValue("@Quantite", Quantite);
                cmd.Parameters.AddWithValue("@etat", etat);
                cmd.Parameters.AddWithValue("@mouvement",mouvement);

                cmd.ExecuteNonQuery();
            }
        }

        public static void UpdateReel(NpgsqlConnection conn ,string Id_mouvement ,double etat)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE reel SET etat = @etat where id_mouvement = @id_mouvement ";

                cmd.Parameters.AddWithValue("@id_mouvement", Id_mouvement);
                cmd.Parameters.AddWithValue("@etat", etat);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
