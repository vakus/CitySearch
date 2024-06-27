using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.TreeSearchCityFinder;
using FluentAssertions;
using Moq;

namespace CitySearch.UnitTest.Service
{
    public class TreeSearchCityFinderTest
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenCityNameLoaderIsMissing()
        {
            Action action = () => new TreeSearchCityFinder(null, new Mock<ICityNameNormaliser>().Object);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCityNameNormaliserIsMissing()
        {
            Action action = () => new TreeSearchCityFinder(new Mock<ICityNameLoader>().Object, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}