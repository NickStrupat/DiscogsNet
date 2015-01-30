using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace DiscogsNet.Database
{
    class InsertBuilder
    {
        private MySqlConnection conn;
        private string table;
        private List<KeyValuePair<string, MySqlDbType>> fields;
        private MySqlCommand command;

        public object this[string parameterName]
        {
            get
            {
                return this.command.Parameters[parameterName].Value;
            }
            set
            {
                this.command.Parameters[parameterName].Value = value;
            }
        }

        public InsertBuilder(MySqlConnection conn, string table)
        {
            this.conn = conn;
            this.table = table;
            fields = new List<KeyValuePair<string, MySqlDbType>>();
        }

        public InsertBuilder AddString(string name)
        {
            fields.Add(new KeyValuePair<string, MySqlDbType>(name, MySqlDbType.MediumText));
            return this;
        }

        public InsertBuilder AddInt32(string name)
        {
            fields.Add(new KeyValuePair<string, MySqlDbType>(name, MySqlDbType.Int32));
            return this;
        }

        public InsertBuilder Build()
        {
            string text = "INSERT INTO `" + table + "` (" +
                fields.Select(f => "`" + f.Key + "`").Join(", ") +
                ") VALUES (" +
                fields.Select(f => "@" + f.Key).Join(", ") +
                ");";
            this.command = new MySqlCommand(text, conn);
            this.command.Prepare();
            foreach (var field in fields)
                this.command.Parameters.Add(field.Key, field.Value);
            return this;
        }

        public long Execute()
        {
            this.command.ExecuteNonQuery();
            return this.command.LastInsertedId;
        }
    }

}
