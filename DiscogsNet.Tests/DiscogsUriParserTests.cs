using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiscogsNet.Tests
{
    [TestClass]
    public class DiscogsUriParserTests
    {
        [TestMethod]
        public void TestParseReleaseIdFromUri()
        {
            Dictionary<string, int> expectedResults = new Dictionary<string, int>()
            {
                {"http://www.discogs.com/Parov-Stelar-Coco/release/1987396", 1987396},
                {"http://www.discogs.com/Parov-Stelar-Coco/release/1987396?testGetArguments=value", 1987396},
                {"http://discogs.com/Parov-Stelar-Coco/release/1987396", 1987396},
                {"http://discogs.com/Parov-Stelar-Coco/release/1987396?testGetArguments=value", 1987396},
                {"http://discogscom/Parov-Stelar-Coco/release/1987396", 0},
                {"http://discogs/Parov-Stelar-Coco/release/1987396", 0},
                {"http://discogs/Parov-Stelar-Coco/releas/1987396", 0},
                {"http://discogs/Parov-Stelar-Coco/release1987396", 0},

                {"http://www.discogs.com/release/1987396", 1987396},
                {"http://discogs.com/release/1987396", 1987396},
                {"http://discogscom/release/1987396", 0},
                {"http:/discogs.com/release1987396", 0},
                {"http://discogs.com/release/", 0},
                {"http://discogs.comrelease/1987396", 0},
            };

            foreach (KeyValuePair<string, int> expectedResult in expectedResults)
            {
                Assert.AreEqual(expectedResult.Value, DiscogsUriParser.ParseReleaseIdFromUri(expectedResult.Key));
            }
        }

        [TestMethod]
        public void TestParseArtistNameFromUri()
        {
            Dictionary<string, string> expectedResults = new Dictionary<string, string>()
            {
                {"http://www.discogs.com/artist/Parov+Stelar", "Parov Stelar"},
                {"http://www.discogs.com/artist/Parov+Stelar?testGetArguments=value", "Parov Stelar"},
                {"http://www.discogs.com/artist/Parov Stelar", "Parov Stelar"},
                {"http://discogs.com/artist/Parov Stelar", "Parov Stelar"},
                {"http://discogs.com/artist/Parov Stelar?testGetArguments=value", "Parov Stelar"},
                {"http://discogs.com/artistParov Stelar", null},
                {"http://discogs.com/Parov Stelar", null},
                {"http://www.iscogs.com/Parov Stelar", null},
                {"www.discogs.com/Parov Stelar", null},
                {"http://www.discogscom/artist/Parov+Stelar", null},
            };

            foreach (KeyValuePair<string, string> expectedResult in expectedResults)
            {
                Assert.AreEqual(expectedResult.Value, DiscogsUriParser.ParseArtistNameFromUri(expectedResult.Key));
            }
        }
    }
}
