using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DiscogsNet.Database
{
    class ArtistFinder
    {
        private MySqlCommand cmdSelectArtist;
        private Dictionary<string, int> artistCache = new Dictionary<string, int>();

        public ArtistFinder(MySqlConnection conn)
        {
            this.cmdSelectArtist = new MySqlCommand("SELECT * FROM `artists` WHERE `name` = @name;", conn);
            this.cmdSelectArtist.Prepare();
            this.cmdSelectArtist.Parameters.Add("name", MySqlDbType.MediumText);

            MySqlCommand selectArtistsCommand = new MySqlCommand("SELECT * FROM `artists`;", conn);
            selectArtistsCommand.CommandTimeout = 60 * 60;
            using (MySqlDataReader reader = selectArtistsCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = Utility.RemoveDiacritics(reader.GetString("name"));
                    this.artistCache[name] = reader.GetInt32("id");
                }
            }
        }

        public int Find(string artistName)
        {
            artistName = Utility.RemoveDiacritics(artistName).ToLower();

            switch (artistName)
            {
                case "various":
                    return (int)SpecialArtist.Various;
                case "unknown artist":
                    return (int)SpecialArtist.Unknown;
                case "no artist":
                    return (int)SpecialArtist.NoArtist;
            }
            if (artistCache.ContainsKey(artistName))
            {
                return artistCache[artistName];
            }
            else
            {
                this.cmdSelectArtist.Parameters["name"].Value = artistName;
                using (var reader = this.cmdSelectArtist.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return this.artistCache[artistName] = reader.GetInt32("id");
                    }
                }
                return this.artistCache[artistName] = (int)SpecialArtist.NotFound;
            }
        }
    }
}
