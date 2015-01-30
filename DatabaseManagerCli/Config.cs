using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;

namespace DatabaseManagerCli
{
    class Config
    {
        public MySqlConnection Conn { get; private set; }

        public string Host { get; private set; }
        public string User { get; private set; }
        public string Pass { get; private set; }
        public string Db { get; private set; }

        public string LabelsXmlPath { get; set; }
        public string ArtistsXmlPath { get; set; }
        public string ReleasesXmlPath { get; set; }
        public string MasterReleasesXmlPath { get; set; }

        public Config(string file)
        {
            this.Parse(file);

            this.Reconnect();
        }

        public void Reconnect()
        {
            this.Conn = new MySqlConnection("host=" + this.Host + ";user=" + this.User + ";password=" + this.Pass + ";charset=utf8");
            this.Conn.Open();
        }

        private void Parse(string file)
        {
            int lineNumber = 0;
            foreach (string _line in File.ReadAllLines(file, Encoding.UTF8))
            {
                ++lineNumber;

                string line = _line.Trim();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] parts = line.Split('=').Select(p => p.Trim()).ToArray();
                if (parts.Length != 2)
                {
                    throw new FormatException(line + Environment.NewLine + "Lines have to be empty or have exactly one '='.");
                }

                string name = parts[0];
                string value = parts[1];

                PropertyInfo property = this.GetType().GetProperty(name);
                if (property == null || property.PropertyType != typeof(string))
                {
                    throw new FormatException(name + ": invalid property");
                }

                property.SetValue(this, value, null);
            }
        }
    }
}
