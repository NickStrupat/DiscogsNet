using System;
using System.Linq;
using DiscogsNet.Model;
using MySql.Data.MySqlClient;

namespace DiscogsNet.Database
{
    public class MasterReleaseInserter : ReleaseInserter
    {
        private InsertBuilder cmdMaster;

        public MasterReleaseInserter(string connectionString)
            : base(connectionString)
        {
        }

        public MasterReleaseInserter(MySqlConnection conn)
            : base(conn)
        {
        }

        protected override void Init()
        {
            base.Init();

            this.cmdMaster = new InsertBuilder(conn, "masters")
                .AddInt32("id")
                .AddInt32("main_release")
                .AddString("title")
                .AddString("joined_artists")
                .AddInt32("year")
                .AddString("notes")
                .Build();

        }

        public void Insert(MasterRelease master)
        {
            this.cmdMaster["id"] = master.Id;
            this.cmdMaster["main_release"] = master.MainRelease;
            this.cmdMaster["title"] = master.Title ?? "";
            this.cmdMaster["joined_artists"] = master.Aggregate.JoinedArtistsFixed;
            this.cmdMaster["year"] = master.Year;
            this.cmdMaster["notes"] = master.Notes ?? "";
            this.cmdMaster.Execute();
            this.InsertReleaseArtists(master, 0, master.Id);
            if (master.Genres != null)
            {
                this.InsertReleaseGenres(master, 0, master.Id);
            }
            if (master.Images != null)
            {
                this.InsertReleaseImages(master, 0, master.Id);
            }
            if (master.Styles != null)
            {
                this.InsertReleaseStyles(master, 0, master.Id);
            }
            if (master.Tracklist != null)
            {
                this.InsertReleaseTracks(master, 0, master.Id);
            }
        }
    }
}
