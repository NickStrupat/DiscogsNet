using DiscogsNet.Model;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace DiscogsNet.Database
{
    public class ReleaseInserter
    {
        protected MySqlConnection conn;
        private InsertBuilder cmdRelease, cmdReleaseArtist, cmdReleaseFormat, cmdReleaseFormatDescription, cmdReleaseGenre, cmdReleaseImage, cmdReleaseLabel, cmdReleaseIdentifier, cmdReleaseStyle, cmdReleaseTrack, cmdReleaseTrackArtist, cmdReleaseTrackExtraArtist;
        private InsertBuilder cmdReleaseFts;
        private ArtistFinder artistFinder;
        private GenreFinder genreFinder;
        private StyleFinder styleFinder;

        public ReleaseInserter(string connectionString)
        {
            conn = new MySqlConnection(connectionString);
            conn.Open();
            Init();
        }
        public ReleaseInserter(MySqlConnection conn)
        {
            this.conn = conn;
            Init();
        }

        protected virtual void Init()
        {
            this.genreFinder = new GenreFinder(this.conn);
            this.styleFinder = new StyleFinder(this.conn);

            this.cmdRelease = new InsertBuilder(conn, "releases")
                .AddInt32("id")
                .AddString("master_id")
                .AddString("status")
                .AddString("title")
                .AddString("joined_artists")
                .AddString("country")
                .AddString("releasedate")
                .AddString("notes")
                .Build();

            this.cmdReleaseArtist = new InsertBuilder(conn, "releases_artists")
                .AddInt32("release")
                .AddInt32("master")
                .AddInt32("number")
                .AddInt32("artist")
                .AddString("namevariation")
                .AddString("join")
                .Build();

            this.cmdReleaseFormat = new InsertBuilder(conn, "releases_formats")
                .AddInt32("release")
                .AddInt32("number")
                .AddString("name")
                .AddInt32("quantity")
                .Build();

            this.cmdReleaseFormatDescription = new InsertBuilder(conn, "releases_formats_descriptions")
                .AddInt32("release_format")
                .AddInt32("number")
                .AddString("description")
                .Build();

            this.cmdReleaseGenre = new InsertBuilder(conn, "releases_genres")
                .AddInt32("release")
                .AddInt32("master")
                .AddInt32("number")
                .AddInt32("genre")
                .Build();

            this.cmdReleaseImage = new InsertBuilder(conn, "releases_images")
                .AddInt32("release")
                .AddInt32("master")
                .AddInt32("number")
                .AddString("type")
                .AddInt32("width")
                .AddInt32("height")
                .AddString("uri")
                .AddString("uri150")
                .Build();

            this.cmdReleaseLabel = new InsertBuilder(conn, "releases_labels")
                .AddInt32("release")
                .AddInt32("number")
                .AddString("catno")
                .AddString("name")
                .Build();

            this.cmdReleaseIdentifier = new InsertBuilder(conn, "releases_identifiers")
                .AddInt32("release")
                .AddInt32("number")
                .AddString("type")
                .AddString("value")
                .AddString("description")
                .Build();

            this.cmdReleaseStyle = new InsertBuilder(conn, "releases_styles")
                .AddInt32("release")
                .AddInt32("master")
                .AddInt32("number")
                .AddInt32("style")
                .Build();

            this.cmdReleaseTrack = new InsertBuilder(conn, "releases_tracks")
                .AddInt32("release")
                .AddInt32("master")
                .AddInt32("number")
                .AddString("position")
                .AddString("title")
                .AddInt32("duration")
                .Build();

            this.cmdReleaseTrackArtist = new InsertBuilder(conn, "releases_tracks_artists")
                .AddInt32("track")
                .AddInt32("number")
                .AddString("artist")
                .AddString("namevariation")
                .AddString("join")
                .Build();

            this.cmdReleaseTrackExtraArtist = new InsertBuilder(conn, "releases_tracks_extraartists")
                .AddInt32("track")
                .AddInt32("number")
                .AddString("name")
                .AddString("namevariation")
                .AddString("role")
                .Build();

            this.cmdReleaseFts = new InsertBuilder(conn, "releases_fts")
                .AddInt32("id")
                .AddString("fts")
                .Build();

            Console.WriteLine("Creating ArtistFinder...");
            this.artistFinder = new ArtistFinder(this.conn);
        }

        protected void InsertReleaseArtists(ReleaseBase release, int releaseId, int masterId)
        {
            if (release.Artists == null)
            {
                return;
            }

            int number = 0;
            foreach (var artist in release.Artists)
            {
                cmdReleaseArtist["release"] = releaseId;
                cmdReleaseArtist["master"] = masterId;

                cmdReleaseArtist["number"] = number++;
                cmdReleaseArtist["artist"] = this.artistFinder.Find(artist.Name);
                cmdReleaseArtist["namevariation"] = artist.NameVariation ?? "";
                cmdReleaseArtist["join"] = artist.Join ?? "";
                cmdReleaseArtist.Execute();
            }
        }

        private void InsertReleaseFormats(Release release, int releaseId)
        {
            int number = 0;
            foreach (var format in release.Formats)
            {
                cmdReleaseFormat["release"] = releaseId;
                cmdReleaseFormat["number"] = number++;
                cmdReleaseFormat["name"] = format.Name;
                cmdReleaseFormat["quantity"] = format.Quantity;
                long formatId = cmdReleaseFormat.Execute();
                if (format.Descriptions != null)
                {
                    InsertReleaseFormatDescriptions(format, formatId);
                }
            }
        }

        private void InsertReleaseFormatDescriptions(ReleaseFormat format, long formatId)
        {
            int number = 0;
            foreach (var desc in format.Descriptions)
            {
                cmdReleaseFormatDescription["release_format"] = formatId;
                cmdReleaseFormatDescription["number"] = number++;
                cmdReleaseFormatDescription["description"] = desc;
                cmdReleaseFormatDescription.Execute();
            }
        }

        protected void InsertReleaseGenres(ReleaseBase release, int releaseId, int masterId)
        {
            int number = 0;
            foreach (var genre in release.Genres)
            {
                cmdReleaseGenre["release"] = releaseId;
                cmdReleaseGenre["master"] = masterId;
                cmdReleaseGenre["number"] = number++;
                cmdReleaseGenre["genre"] = this.genreFinder.Find(genre);
                cmdReleaseGenre.Execute();
            }
        }

        protected void InsertReleaseImages(ReleaseBase release, int releaseId, int masterId)
        {
            int number = 0;
            foreach (var image in release.Images)
            {
                cmdReleaseImage["release"] = releaseId;
                cmdReleaseImage["master"] = masterId;

                cmdReleaseImage["number"] = number++;
                cmdReleaseImage["type"] = image.Type;
                cmdReleaseImage["width"] = image.Width;
                cmdReleaseImage["height"] = image.Width;
                cmdReleaseImage["uri"] = image.Uri;
                cmdReleaseImage["uri150"] = image.Uri150;
                cmdReleaseImage.Execute();
            }
        }

        private void InsertReleaseLabels(Release release, int releaseId)
        {
            int number = 0;
            foreach (var label in release.Labels)
            {
                cmdReleaseLabel["release"] = releaseId;
                cmdReleaseLabel["number"] = number++;
                cmdReleaseLabel["catno"] = label.CatalogNumber ?? "";
                cmdReleaseLabel["name"] = label.Name;
                cmdReleaseLabel.Execute();
            }
        }

        private void InsertReleaseIdentifiers(Release release, int releaseId)
        {
            int number = 0;
            foreach (var identifier in release.Identifiers)
            {
                cmdReleaseIdentifier["release"] = releaseId;
                cmdReleaseIdentifier["number"] = number++;
                cmdReleaseIdentifier["type"] = identifier.Type.ToString();
                cmdReleaseIdentifier["value"] = identifier.Value ?? "";
                cmdReleaseIdentifier["description"] = identifier.Description ?? "";
                cmdReleaseIdentifier.Execute();
            }
        }

        protected void InsertReleaseStyles(ReleaseBase release, int releaseId, int masterId)
        {
            int number = 0;
            foreach (var style in release.Styles)
            {
                cmdReleaseStyle["release"] = releaseId;
                cmdReleaseStyle["master"] = masterId;

                cmdReleaseStyle["number"] = number++;
                cmdReleaseStyle["style"] = this.styleFinder.Find(style);
                cmdReleaseStyle.Execute();
            }
        }

        protected void InsertReleaseTracks(ReleaseBase release, int releaseId, int masterId)
        {
            int number = 0;
            foreach (var track in release.Tracklist)
            {
                cmdReleaseTrack["release"] = releaseId;
                cmdReleaseTrack["master"] = masterId;

                cmdReleaseTrack["number"] = number++;
                cmdReleaseTrack["position"] = track.Position;
                cmdReleaseTrack["title"] = track.Title;
                cmdReleaseTrack["duration"] = (int)track.Aggregate.Duration.TotalSeconds;
                long trackId = cmdReleaseTrack.Execute();
                if (track.Artists != null)
                {
                    InsertReleaseTrackArtists(track, trackId);
                }
                if (track.ExtraArtists != null)
                {
                    InsertReleaseTrackExtraArtists(track, trackId);
                }
            }
        }

        private void InsertReleaseTrackArtists(Track track, long trackId)
        {
            int number = 0;
            foreach (var artist in track.Artists)
            {
                cmdReleaseTrackArtist["track"] = trackId;
                cmdReleaseTrackArtist["number"] = number++;
                cmdReleaseTrackArtist["artist"] = this.artistFinder.Find(artist.Name);
                cmdReleaseTrackArtist["namevariation"] = artist.NameVariation ?? "";
                cmdReleaseTrackArtist["join"] = artist.Join ?? "";
                cmdReleaseTrackArtist.Execute();
            }
        }

        private void InsertReleaseTrackExtraArtists(Track track, long trackId)
        {
            int number = 0;
            foreach (var extraArtist in track.ExtraArtists)
            {
                cmdReleaseTrackExtraArtist["track"] = trackId;
                cmdReleaseTrackExtraArtist["number"] = number++;
                cmdReleaseTrackExtraArtist["name"] = extraArtist.Name;
                cmdReleaseTrackExtraArtist["namevariation"] = extraArtist.NameVariation ?? "";
                cmdReleaseTrackExtraArtist["role"] = extraArtist.Role ?? "";
                cmdReleaseTrackExtraArtist.Execute();
            }
        }

        public void Insert(Release release)
        {
            cmdRelease["id"] = release.Id;
            cmdRelease["master_id"] = release.MasterId;
            cmdRelease["status"] = release.Status.ToString();
            cmdRelease["title"] = release.Title;
            cmdRelease["joined_artists"] = release.Aggregate.JoinedArtistsFixed;
            cmdRelease["country"] = release.Country ?? "";
            cmdRelease["releasedate"] = release.ReleaseDate ?? "";
            cmdRelease["notes"] = release.Notes ?? "";
            cmdRelease.Execute();
            this.InsertReleaseArtists(release, release.Id, 0);
            this.InsertReleaseFormats(release, release.Id);
            if (release.Genres != null)
            {
                this.InsertReleaseGenres(release, release.Id, 0);
            }
            if (release.Images != null)
            {
                this.InsertReleaseImages(release, release.Id, 0);
            }
            if (release.Labels != null)
            {
                this.InsertReleaseLabels(release, release.Id);
            }
            if (release.Identifiers != null)
            {
                this.InsertReleaseIdentifiers(release, release.Id);
            }
            if (release.Styles != null)
            {
                this.InsertReleaseStyles(release, release.Id, 0);
            }
            this.InsertReleaseTracks(release, release.Id, 0);
        }

        public void InsertFts(Release release)
        {
            cmdReleaseFts["id"] = release.Id;
            StringBuilder fts = new StringBuilder();
            fts.Append(release.Aggregate.JoinedArtistsFixed);
            fts.Append(" ");
            fts.Append(release.Title);
            foreach (ReleaseLabel label in release.Labels)
            {
                fts.Append(" ");
                fts.Append(label.Name);
                fts.Append(" ");
                fts.Append(label.CatalogNumber);
            }
            cmdReleaseFts["fts"] = fts.ToString();
            cmdReleaseFts.Execute();
        }
    }
}
