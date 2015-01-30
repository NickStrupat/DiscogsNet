using System;
using System.IO;
using System.Linq;

namespace DatabaseManagerCli
{
    class DatabaseHelper
    {
        private Config config;

        public DatabaseHelper(Config config)
        {
            this.config = config;
        }

        public void DropDatabase()
        {
            try
            {
                var command = this.config.Conn.CreateCommand();
                command.CommandText = "DROP DATABASE `" + this.config.Db + "`;";
                command.ExecuteNonQuery();
            }
            catch
            {
            }
        }

        public void CreateDatabase()
        {
            try
            {
                var command = this.config.Conn.CreateCommand();
                command.CommandText = "CREATE DATABASE `" + this.config.Db + "` COLLATE `utf8_unicode_ci`;";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating DB: " + ex.Message);
            }
        }

        public void CreateTables()
        {
            string schema = File.ReadAllText("DatabaseSchema.txt");
            var command = this.config.Conn.CreateCommand();
            command.CommandText = schema;
            command.ExecuteNonQuery();

            Console.WriteLine("Tables created.");
        }

        internal void SelectDatabase()
        {
            var command = this.config.Conn.CreateCommand();
            command.CommandText = "USE `" + this.config.Db + "`;";
            command.ExecuteNonQuery();
        }

        internal void TryCreatePreReleasesImportIndexes()
        {
            try
            {
                var command = this.config.Conn.CreateCommand();
                command.CommandTimeout = 15 * 60 * 60; // 15 minutes
                command.CommandText = "CREATE INDEX `name_index` ON `artists` (name(11));";
                command.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                if (ex.Message != "Duplicate key name 'name_index'")
                {
                    throw ex;
                }
            }
        }
    }
}
