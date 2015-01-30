using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DiscogsNet.Database
{
    class GenreFinder
    {
        private Dictionary<string, long> genreCache = new Dictionary<string, long>();
        private MySqlCommand cmdSelectGenre;
        private InsertBuilder cmdInsertGenre;

        public GenreFinder(MySqlConnection conn)
        {
            cmdSelectGenre = new MySqlCommand("SELECT * FROM `genres` WHERE `genre` = @genre;", conn);
            cmdSelectGenre.Prepare();
            cmdSelectGenre.Parameters.Add("genre", MySqlDbType.MediumText);

            cmdInsertGenre = new InsertBuilder(conn, "genres")
                .AddString("genre")
                .Build();
        }

        public long Find(string name)
        {
            if (genreCache.ContainsKey(name))
            {
                return genreCache[name];
            }

            cmdSelectGenre.Parameters["genre"].Value = name;
            using (var reader = cmdSelectGenre.ExecuteReader())
            {
                if (reader.Read())
                {
                    return genreCache[name] = reader.GetInt64("id");
                }
            }

            cmdInsertGenre["genre"] = name;
            return genreCache[name] = cmdInsertGenre.Execute();
        }
    }
}
