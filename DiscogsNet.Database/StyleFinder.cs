using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DiscogsNet.Database
{
    class StyleFinder
    {
        private Dictionary<string, long> styleCache = new Dictionary<string, long>();
        private MySqlCommand cmdSelectStyle;
        private InsertBuilder cmdInsertStyle;

        public StyleFinder(MySqlConnection conn)
        {
            cmdSelectStyle = new MySqlCommand("SELECT * FROM `styles` WHERE `style` = @style;", conn);
            cmdSelectStyle.Prepare();
            cmdSelectStyle.Parameters.Add("style", MySqlDbType.MediumText);

            cmdInsertStyle = new InsertBuilder(conn, "styles")
                .AddString("style")
                .Build();
        }

        public long Find(string name)
        {
            if (styleCache.ContainsKey(name))
            {
                return styleCache[name];
            }

            cmdSelectStyle.Parameters["style"].Value = name;
            using (var reader = cmdSelectStyle.ExecuteReader())
            {
                if (reader.Read())
                {
                    return styleCache[name] = reader.GetInt64("id");
                }
            }

            cmdInsertStyle["style"] = name;
            return styleCache[name] = cmdInsertStyle.Execute();
        }
    }
}
