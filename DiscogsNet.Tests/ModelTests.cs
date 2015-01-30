using DiscogsNet.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiscogsNet.Tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void TestNameFixing()
        {
            ArtistAggregate.NameFixingLevel = NameFixingLevel.All;
            Assert.AreEqual("The Test Artist", ArtistAggregate.FixName("Test Artist, The (3)"));
            Assert.AreEqual("The Test Artist", ArtistAggregate.FixName("Test Artist, The"));

            ArtistAggregate.NameFixingLevel = NameFixingLevel.FixThe;
            Assert.AreEqual("Test Artist, The (3)", ArtistAggregate.FixName("Test Artist, The (3)"));
            Assert.AreEqual("The Test Artist", ArtistAggregate.FixName("Test Artist, The"));

            ArtistAggregate.NameFixingLevel = NameFixingLevel.RemoveNumbers;
            Assert.AreEqual("Test Artist, The", ArtistAggregate.FixName("Test Artist, The (3)"));
            Assert.AreEqual("Test Artist, The", ArtistAggregate.FixName("Test Artist, The"));

            ArtistAggregate.NameFixingLevel = NameFixingLevel.All;

            Artist artist = new Artist();
            artist.Name = "Test Artist, The (3)";
            Assert.AreEqual("The Test Artist", artist.Aggregate.NameFixed);
        }

        [TestMethod]
        public void TestReleaseYearGuesser()
        {
            Assert.AreEqual(2006, ReleaseYearGuesser.Guess("2006"));
            Assert.AreEqual(2006, ReleaseYearGuesser.Guess("2006-00"));
            Assert.AreEqual(2006, ReleaseYearGuesser.Guess("2006-00-00"));
            Assert.AreEqual(0, ReleaseYearGuesser.Guess("00-00-00"));
            Assert.AreEqual(0, ReleaseYearGuesser.Guess(""));
            Assert.AreEqual(0, ReleaseYearGuesser.Guess(null));
        }
    }
}
