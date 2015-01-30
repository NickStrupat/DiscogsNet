using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace CharacterCleaner
{
    class Program
    {
        static string IdStart = "<id>";
        static string IdEnd = "</id>";

        static void Main(string[] args)
        {
            using (StreamWriter log = new StreamWriter(@"log.txt"))
            {
                // modify these paths to reflect your files
                Stream inputFile = File.OpenRead(@"D:\Ivo\Discogs\20111110\discogs_20111110_artists.xml.gz");
                using (Stream outputFile = File.Create(@"D:\Ivo\Discogs\20111110\discogs_20111110_artists.fixed.xml.gz"))
                {
                    GZipStream input = new GZipStream(inputFile, CompressionMode.Decompress);
                    using (GZipStream output = new GZipStream(outputFile, CompressionMode.Compress))
                    {

                        StreamReader reader = new StreamReader(input, Encoding.UTF8);
                        using (StreamWriter writer = new StreamWriter(output, Encoding.UTF8))
                        {
                            int itemId = 0;

                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                int i1 = line.IndexOf(IdStart);
                                if (i1 != -1)
                                {
                                    int i2 = line.IndexOf(IdEnd, i1 + IdStart.Length);
                                    if (i2 != -1)
                                    {
                                        itemId = int.Parse(line.Substring(i1 + IdStart.Length, i2 - i1 - IdStart.Length));
                                    }
                                }

                                if (line.Contains("\u000B"))
                                {
                                    line = line.Replace("\u000B", "");
                                    log.WriteLine("Removed 0x0B @ " + itemId + " " + line);
                                }
                                if (line.Contains("\u0010"))
                                {
                                    line = line.Replace("\u0010", "");
                                    log.WriteLine("Removed 0x10 @ " + itemId + " " + line);
                                }
                                if (line.Contains("\u0003"))
                                {
                                    line = line.Replace("\u0003", "");
                                    log.WriteLine("Removed 0x03 @ " + itemId + " " + line);
                                }
                                if (line.Contains("\u0007"))
                                {
                                    line = line.Replace("\u0007", "");
                                    log.WriteLine("Removed 0x07 @ " + itemId + " " + line);
                                }
                                if (line.Contains("\u0018"))
                                {
                                    line = line.Replace("\u0018", "");
                                    log.WriteLine("Removed 0x18 @ " + itemId + " " + line);
                                }
                                if (line.Contains("\u001B"))
                                {
                                    line = line.Replace("\u001B", "");
                                    log.WriteLine("Removed 0x1B @ " + itemId + " " + line);
                                }

                                writer.WriteLine(line);
                            }
                        }
                    }
                }
            }
        }
    }
}
