using System;
using System.Linq;
using DiscogsNet.Database;
using DiscogsNet.FileReading;
using DiscogsNet.Model;
using MySql.Data.MySqlClient;

namespace DatabaseManagerCli
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run(args);
        }

        private Config config;
        private DatabaseHelper helper;
        private ProgressReporter progressReporter;

        private void Run(string[] args)
        {
            try
            {
                config = new Config("Config.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            helper = new DatabaseHelper(config);
            progressReporter = new ProgressReporter();

            if (args.Length == 0)
            {
                Console.WriteLine("Commands:");
                Console.WriteLine("  drop-db");
                Console.WriteLine("  create-db");
                Console.WriteLine("  read-labels");
                Console.WriteLine("  read-artists");
                Console.WriteLine("  read-releases");
                Console.WriteLine("  read-masters");
                Console.WriteLine("  import-labels");
                Console.WriteLine("  import-artists");
                Console.WriteLine("  import-releases");
                Console.WriteLine("  import-masters");
            }
            else
            {
                foreach (string command in args)
                {
                    ExecuteCommand(command);
                }
            }
        }

        private void ExecuteCommand(string command)
        {
            switch (command)
            {
                case "drop-db":
                    helper.DropDatabase();
                    break;
                case "create-db":
                    helper.CreateDatabase();
                    helper.SelectDatabase();
                    helper.CreateTables();
                    break;
                case "import-labels":
                    this.ImportLabels();
                    break;
                case "import-artists":
                    this.ImportArtists();
                    break;
                case "import-releases":
                    this.ImportReleases();
                    break;
                case "import-releases-fts":
                    this.ImportReleasesFts();
                    break;
                case "import-masters":
                    this.ImportMasters();
                    break;
                case "read-labels":
                    this.ReadLabels();
                    break;
                case "read-artists":
                    this.ReadArtists();
                    break;
                case "read-masters":
                    this.ReadMasters();
                    break;
                case "read-releases":
                    this.ReadReleases();
                    break;
                default:
                    Console.WriteLine(command + ": unknown command");
                    break;
            }
        }

        private void ReadLabels()
        {
            this.helper.SelectDatabase();

            LabelReader2 labelReader = new LabelReader2(this.config.LabelsXmlPath);
            foreach (Label label in labelReader.Enumerate())
            {
                this.progressReporter.Report("Labels", labelReader.EstimatedProgress);
            }
            this.progressReporter.Report("Labels", -1);
        }

        private void ReadArtists()
        {
            ArtistReader2 artistReader = new ArtistReader2(this.config.ArtistsXmlPath);
            foreach (Artist artist in artistReader.Enumerate())
            {
                this.progressReporter.Report("Artists", artistReader.EstimatedProgress);
            }
            this.progressReporter.Report("Artists", -1);
        }

        private void ReadMasters()
        {
            MasterReleaseReader2 masterReader = new MasterReleaseReader2(this.config.MasterReleasesXmlPath);
            foreach (MasterRelease master in masterReader.Enumerate())
            {
                this.progressReporter.Report("Masters", masterReader.EstimatedProgress);
            }
            this.progressReporter.Report("Masters", -1);
        }

        private void ReadReleases()
        {
            ReleaseReader2 releaseReader = new ReleaseReader2(this.config.ReleasesXmlPath);
            foreach (Release release in releaseReader.Enumerate())
            {
                this.progressReporter.Report("Releases", releaseReader.EstimatedProgress);
            }
            this.progressReporter.Report("Releases", -1);
        }

        private void ImportLabels()
        {
            this.helper.SelectDatabase();

            LabelReader2 labelReader = new LabelReader2(this.config.LabelsXmlPath);
            LabelInserter labelInserter = new LabelInserter(this.config.Conn);
            foreach (Label label in labelReader.Enumerate())
            {
                try
                {
                    labelInserter.Insert(label);
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error importing label " + label.Id + ": " + ex.Message);
                }

                this.progressReporter.Report("Labels", labelReader.EstimatedProgress);
            }
            this.progressReporter.Report("Labels", -1);
        }

        private void ImportArtists()
        {
            this.helper.SelectDatabase();

            ArtistReader2 artistReader = new ArtistReader2(this.config.ArtistsXmlPath);
            ArtistInserter artistInserter = new ArtistInserter(this.config.Conn);
            foreach (Artist artist in artistReader.Enumerate())
            {
                try
                {
                    artistInserter.Insert(artist);
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error importing artist " + artist.Id + ": " + ex.Message);
                }

                this.progressReporter.Report("Artists", artistReader.EstimatedProgress);
            }
            this.progressReporter.Report("Artists", -1);
        }

        private void ImportReleases()
        {
            this.helper.SelectDatabase();

            Console.WriteLine("Creating index on artists...");
            this.helper.TryCreatePreReleasesImportIndexes();

            Console.WriteLine("Importing data...");
            ReleaseReader2 releaseReader = new ReleaseReader2(this.config.ReleasesXmlPath);
            ReleaseInserter releaseInserter = new ReleaseInserter(this.config.Conn);
            foreach (Release release in releaseReader.Enumerate())
            {
                try
                {
                    releaseInserter.Insert(release);
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error importing release " + release.Id + ": " + ex.Message);
                }

                this.progressReporter.Report("Releases", releaseReader.EstimatedProgress);
            }
            this.progressReporter.Report("Releases", -1);
        }

        private void ImportReleasesFts()
        {
            this.helper.SelectDatabase();

            Console.WriteLine("Importing data...");
            ReleaseReader2 releaseReader = new ReleaseReader2(this.config.ReleasesXmlPath);
            ReleaseInserter releaseInserter = new ReleaseInserter(this.config.Conn);
            foreach (Release release in releaseReader.Enumerate())
            {
                try
                {
                    releaseInserter.InsertFts(release);
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("Error importing release " + release.Id + ": " + ex.Message);
                }

                this.progressReporter.Report("Releases", releaseReader.EstimatedProgress);
            }
            this.progressReporter.Report("Releases", -1);
        }

        private void ImportMasters()
        {
            this.helper.SelectDatabase();

            Console.WriteLine("Creating index on artists...");
            this.helper.TryCreatePreReleasesImportIndexes();

            MasterReleaseReader2 masterReleaseReader = new MasterReleaseReader2(this.config.MasterReleasesXmlPath);
            MasterReleaseInserter masterInserter = new MasterReleaseInserter(this.config.Conn);
            foreach (MasterRelease master in masterReleaseReader.Enumerate())
            {
                masterInserter.Insert(master);

                this.progressReporter.Report("Masters", masterReleaseReader.EstimatedProgress);
            }
            this.progressReporter.Report("Masters", -1);
        }
    }
}
