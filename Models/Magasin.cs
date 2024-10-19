using System;
using System.Collections.Generic;
using Npgsql;
using Utils;

namespace Element
{
    public class Magasin
    {
        public string id_magasin;
        public string nom;

        public string Id_magasin { get; set; }
        public string Nom { get; set; }

        public Magasin()
        {
        }

        public Magasin(string idmagasin, string nom)
        {
            this.Id_magasin = idmagasin;
            this.Nom = nom;
        }

        public static List<Magasin> AllMagasin(NpgsqlConnection conn)
        {
            return DatabaseHelper.Select<Magasin>(conn, "magasin");
        }
    }
}