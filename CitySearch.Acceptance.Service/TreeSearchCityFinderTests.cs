using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.DatasetNormaliser;
using CitySearch.Service.TreeSearchCityFinder;
using FluentAssertions;
using Moq;

namespace CitySearch.Acceptance.Service
{
    public class TreeSearchCityFinderTests
    {
        [Theory]
        [InlineData("BANDUNG,BANGUI,BANGKOK,BANGALORE", "BANG", "BANGUI,BANGKOK,BANGALORE", "UKA")]
        [InlineData("LA PAZ,LA PLATA,LAGOS,LEEDS", "LA", "LA PAZ,LA PLATA,LAGOS", " G")]
        public void Search_ReturnsOnlyExpectedCitiesAndExpectedNextCharacters(string cityDataset, string search, string expectedCities, string expectedCharacters)
        {
            var citiesList = cityDataset.Split(',');
            var expectedCitiesList = expectedCities.Split(',');
            var expectedNextLetters = expectedCharacters.ToCharArray().Select(chr => chr.ToString()).ToArray();
            var (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder) = SetupCityFinder(citiesList);

            var result = finder.Search(search);

            result.Should().NotBeNull();
            result.NextCities.Should()
                .BeEquivalentTo(expectedCitiesList);
            result.NextLetters.Should()
                .BeEquivalentTo(expectedNextLetters);
        }

        [Theory]
        [InlineData("ZARIA,ZHUGHAI,ZIBO", "ZE")]
        public void Search_ReturnsNoMatches_WhenNoCitiesInDatasetStartWithSearch(string cityDataset, string search)
        {
            var citiesList = cityDataset.Split(',');
            var (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder) = SetupCityFinder(citiesList);

            var result = finder.Search(search);

            result.Should().NotBeNull();
            result.NextCities.Should()
                .BeEmpty();
            result.NextLetters.Should()
                .BeEmpty();

        }

        private static (Mock<ICityNameLoader>, Mock<ICityNameNormaliser>, Mock<IDatasetNormaliser>, TreeSearchCityFinder) SetupCityFinder(IList<string> cities)
        {
            var nameLoaderMock = new Mock<ICityNameLoader>();
            nameLoaderMock.Setup(m => m.Load()).Returns(cities);
            var nameNormaliserMock = new Mock<ICityNameNormaliser>();
            nameNormaliserMock.Setup(m => m.Normalise(It.IsAny<string>()))
                .Returns((string city) => city);
            var datasetNormaliserMock = new Mock<IDatasetNormaliser>();
            datasetNormaliserMock.Setup(m => m.Normalise(It.IsAny<IList<string>>()))
                .Returns((IList<string> cities) => cities);
            var finder = new TreeSearchCityFinder(nameLoaderMock.Object, nameNormaliserMock.Object, datasetNormaliserMock.Object);

            return (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder);
        }
    }
}