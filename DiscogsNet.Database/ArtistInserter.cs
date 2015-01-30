using System;
using DiscogsNet.Model;
using MySql.Data.MySqlClient;

namespace DiscogsNet.Database
{
    public class ArtistInserter
    {
        private MySqlConnection conn;
        private InsertBuilder cmdArtist, cmdAlias, cmdNameVariation, cmdUrl, cmdImage, cmdGroup, cmdMember;

        public ArtistInserter(MySqlConnection conn)
        {
            this.conn = conn;
            Init();
        }

        public ArtistInserter(string connectionString)
        {
            conn = new MySqlConnection(connectionString);
            conn.Open();
            Init();
        }

        private void Init()
        {
            cmdArtist = new InsertBuilder(this.conn, "artists")
                .AddString("name")
                .AddString("realname")
                .AddString("profile")
                .Build();

            cmdAlias = new InsertBuilder(this.conn, "artists_aliases")
                .AddInt32("artist")
                .AddInt32("number")
                .AddString("alias")
                .Build();

            cmdNameVariation = new InsertBuilder(this.conn, "artists_namevariations")
                .AddInt32("artist")
                .AddInt32("number")
                .AddString("namevariation")
                .Build();

            cmdUrl = new InsertBuilder(this.conn, "artists_urls")
                .AddInt32("artist")
                .AddInt32("number")
                .AddString("url")
                .Build();

            cmdImage = new InsertBuilder(this.conn, "artists_images")
                .AddInt32("artist")
                .AddInt32("number")
                .AddString("type")
                .AddInt32("width")
                .AddInt32("height")
                .AddString("uri")
                .AddString("uri150")
                .Build();

            cmdGroup = new InsertBuilder(this.conn, "artists_groups")
                .AddInt32("artist")
                .AddInt32("number")
                .AddString("group")
                .Build();

            cmdMember = new InsertBuilder(this.conn, "artists_members")
                .AddInt32("artist")
                .AddInt32("number")
                .AddString("member")
                .Build();
        }

        private void InsertAliases(Artist artist, long artistId)
        {
            int number = 0;
            foreach (var alias in artist.Aliases)
            {
                cmdAlias["artist"] = artistId;
                cmdAlias["number"] = number++;
                cmdAlias["alias"] = alias.Name;
                cmdAlias.Execute();
            }
        }

        private void InsertNameVariations(Artist artist, long artistId)
        {
            int number = 0;
            foreach (var nameVariation in artist.NameVariations)
            {
                cmdNameVariation["artist"] = artistId;
                cmdNameVariation["number"] = number++;
                cmdNameVariation["namevariation"] = nameVariation;
                cmdNameVariation.Execute();
            }
        }

        private void InsertUrls(Artist artist, long artistId)
        {
            int number = 0;
            foreach (var url in artist.Urls)
            {
                cmdUrl["artist"] = artistId;
                cmdUrl["number"] = number++;
                cmdUrl["url"] = url;
                cmdUrl.Execute();
            }
        }

        private void InsertImages(Artist artist, long artistId)
        {
            int number = 0;
            foreach (var image in artist.Images)
            {
                cmdImage["artist"] = artistId;
                cmdImage["number"] = number++;
                cmdImage["type"] = image.Type.ToString();
                cmdImage["width"] = image.Width;
                cmdImage["height"] = image.Height;
                cmdImage["uri"] = image.Uri ?? "";
                cmdImage["uri150"] = image.Uri150 ?? "";
                cmdImage.Execute();
            }
        }

        private void InsertGroups(Artist artist, long artistId)
        {
            int number = 0;
            foreach (var group in artist.Groups)
            {
                cmdGroup["artist"] = artistId;
                cmdGroup["number"] = number++;
                cmdGroup["group"] = group;
                cmdGroup.Execute();
            }
        }

        private void InsertMembers(Artist artist, long artistId)
        {
            int number = 0;
            foreach (var member in artist.Members)
            {
                cmdMember["artist"] = artistId;
                cmdMember["number"] = number++;
                cmdMember["member"] = member;
                cmdMember.Execute();
            }
        }

        public void Insert(Artist artist)
        {
            try
            {
                cmdArtist["name"] = artist.Name;
                cmdArtist["realname"] = artist.RealName ?? "";
                cmdArtist["profile"] = artist.Profile ?? "";
                long artistId = cmdArtist.Execute();

                if (artist.Aliases != null)
                    InsertAliases(artist, artistId);
                if (artist.Groups != null)
                    InsertGroups(artist, artistId);
                if (artist.Images != null)
                    InsertImages(artist, artistId);
                if (artist.Members != null)
                    InsertMembers(artist, artistId);
                if (artist.NameVariations != null)
                    InsertNameVariations(artist, artistId);
                if (artist.Urls != null)
                    InsertUrls(artist, artistId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting artist: " + ex.Message);
            }
        }
    }
}
