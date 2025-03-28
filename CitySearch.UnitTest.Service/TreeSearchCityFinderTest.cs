using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.DatasetNormaliser;
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
            Action action = () => new TreeSearchCityFinder(
                null,
                new Mock<ICityNameNormaliser>().Object,
                new Mock<IDatasetNormaliser>().Object);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenCityNameNormaliserIsMissing()
        {
            Action action = () => new TreeSearchCityFinder(
                new Mock<ICityNameLoader>().Object,
                null,
                new Mock<IDatasetNormaliser>().Object);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenDatasetNormaliserIsNull()
        {
            Action action = () => new TreeSearchCityFinder(
                new Mock<ICityNameLoader>().Object,
                new Mock<ICityNameNormaliser>().Object,
                null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData("LON")]
        [InlineData("WAR")]
        [InlineData("BER")]
        [InlineData("PAR")]
        [InlineData("a")]
        [InlineData("g")]
        [InlineData("k")]
        [InlineData("")]
        public void Search_ShouldReturnNoResults_WhenCityListIsEmpty(string citySearch)
        {
            // arrange
            var (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder) = SetupCityFinder(Array.Empty<string>());

            // act
            var result = finder.Search(citySearch);

            // assert
            result.Should().NotBeNull();
            result.NextCities.Should().HaveCount(0);
            result.NextLetters.Should().HaveCount(0);

            nameLoaderMock.Verify(m => m.Load(), Times.Once);
            nameNormaliserMock.Verify(m => m.Normalise(It.IsAny<string>()), Times.AtMostOnce);
            datasetNormaliserMock.Verify(m => m.Normalise(It.IsAny<IList<string>>()), Times.Once);
        }

        [Theory]
        [InlineData("LONDON,TEHRAN,FAISALABAD,VISHAKHAPATNAM,SHUBRA AL KHAYMAH,E'ZHOU,PORT-AU-PRINCE")]
        [InlineData("XINSHI,COCHABAMBA,BARNAUL,TRIPOLI,JIN'E,HALWAN,ZHUANGYUAN")]
        [InlineData("AKRON,NEW HAVEN,TLAJOMULCO DE ZUNIGA,ESLAMSHAHR,BIKANER,LYON,KASHGAR,CHIMBOTE,DUNHUA,FUYUAN")]
        [InlineData("ZHANGJIAKOU SHI XUANHUA QU,IRAPUATO,WINSTON-SALEM,TARAZ,SZCZECIN,BAICHENG,MINAMISUITA,KISSIMMEE")]
        public void Search_ShouldReturnOneResult_WhenNoSimilarCharactersIsFound(string allCityNames)
        {
            var cities = allCityNames.Split(',');
            var (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder) = SetupCityFinder(cities);

            foreach (var city in cities)
            {
                for (int i = 1; i <= city.Length; i++)
                {
                    var search = city[..i];

                    var result = finder.Search(search);

                    result.Should().NotBeNull();
                    result.NextCities.Should()
                        .NotBeEmpty()
                        .And.AllBe(city);
                    if (search == city)
                    {
                        result.NextLetters.Should().BeEmpty();
                    }
                    else
                    {
                        result.NextLetters.Should()
                            .NotBeEmpty()
                            .And.AllBe(city[i].ToString());
                    }
                }
            }

            nameLoaderMock.Verify(m => m.Load(), Times.Once);
            nameNormaliserMock.Verify(m => m.Normalise(It.IsAny<string>()), Times.AtLeastOnce);
            datasetNormaliserMock.Verify(m => m.Normalise(It.IsAny<IList<string>>()), Times.Once);
        }

        [Theory]
        [InlineData("LONDON,LAS PALMAS,LATUR,LENGSHUIJIANG,LICHENG")]
        [InlineData("BYDGOSZCZ,BILBAO,BREST,BRAHMAPUR,BUON MA THUOT")]
        [InlineData("KOCHI,KUCHING,KURGAN,KAIYUAN,KARLSRUHE")]
        [InlineData("PODOLSK,PEKALONGAN,POPAYAN,PANIPAT,PLANO")]
        public void Search_ShouldReturnAllCities_WhenStartingWithTheSameCharacter(string allCityNames)
        {
            var cities = allCityNames.Split(',');
            var (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder) = SetupCityFinder(cities);

            var search = allCityNames[..1];
            var result = finder.Search(search);

            result.Should().NotBeNull();
            result.NextCities.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(cities);
            result.NextLetters.Should()
                .NotBeEmpty()
                .And.AllSatisfy(letter => cities.Should().ContainMatch($"{search}{letter}*"));
            nameLoaderMock.Verify(m => m.Load(), Times.Once);
            nameNormaliserMock.Verify(m => m.Normalise(It.IsAny<string>()), Times.Once);
            datasetNormaliserMock.Verify(m => m.Normalise(It.IsAny<IList<string>>()), Times.Once);
        }

        [Theory]
        [InlineData("LONDYN", "LONDON,LAS PALMAS,LATUR,LENGSHUIJIANG,LICHENG")]
        [InlineData("SZCZECIN", "BYDGOSZCZ,BILBAO,BREST,BRAHMAPUR,BUON MA THUOT")]
        [InlineData("KANO", "KOCHI,KUCHING,KURGAN,KAIYUAN,KARLSRUHE")]
        [InlineData("PINGLIANG", "PODOLSK,PEKALONGAN,POPAYAN,PANIPAT,PLANO")]
        public void Search_ShouldReturnNoEntries_WhenCityNameIsNotInDataSet(string search, string allCityNames)
        {
            var cities = allCityNames.Split(',');
            var (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder) = SetupCityFinder(cities);

            var result = finder.Search(search);

            result.Should().NotBeNull();
            result.NextCities.Should().BeEmpty();
            result.NextLetters.Should().BeEmpty();

            nameLoaderMock.Verify(m => m.Load(), Times.Once);
            nameNormaliserMock.Verify(m => m.Normalise(It.IsAny<string>()), Times.Once);
            datasetNormaliserMock.Verify(m => m.Normalise(It.IsAny<IList<string>>()), Times.Once);
        }

        [Theory]
        [InlineData("LON", "LONDON,BERLIN,LONDRINA")]
        [InlineData("WAR", "WARANGAL,JOANE,WARSAW,ASTLEY,WARMINSTER")]
        [InlineData("A", "ATKINSON,BYDGOSZCZ,ARTEMISA,BRUSSELS,ASZOD,BRYANT,ASTORIA")]
        public void Search_ShouldReturnNoEntries_WherePrefixIsIncorrect(string search, string allCityNames)
        {
            var cities = allCityNames.Split(',');
            var (nameLoaderMock, nameNormaliserMock, datasetNormaliserMock, finder) = SetupCityFinder(cities);

            var result = finder.Search(search);

            result.Should().NotBeNull();
            result.NextCities.Should()
                .NotBeEmpty()
                .And.AllSatisfy(city => city.Should().StartWithEquivalentOf(search));
            result.NextLetters.Should()
                .NotBeEmpty();

            nameLoaderMock.Verify(m => m.Load(), Times.Once);
            nameNormaliserMock.Verify(m => m.Normalise(It.IsAny<string>()), Times.Once);
            datasetNormaliserMock.Verify(m => m.Normalise(It.IsAny<IList<string>>()), Times.Once);
        }

        [Theory]
        [InlineData("LONDON")]
        [InlineData("TEHRAN")]
        [InlineData("FAISALABAD")]
        [InlineData("VISHAKHAPATNAM")]
        [InlineData("LAIWU")]
        [InlineData("SHUBRA AL KHAYMAH")]
        [InlineData("E'ZHOU")]
        [InlineData("PORT-AU-PRINCE")]
        [InlineData("SRI JAYEWARDENEPURA KOTTE")]
        [InlineData("TA`IZZ")]
        public void GenerateNodeTreeFromCities_AllElementsShouldOnlyHaveOneNode_WhenOnlyOneCityIsProvided(string city)
        {
            var cities = new List<string> {city};

            var node = TreeSearchCityFinder.GenerateNodeTreeFromCities(cities);

            var currentNode = node;
            foreach (var character in city)
            {
                currentNode.Should().NotBeNull();
                currentNode.Start.Should().Be(0);
                currentNode.Length.Should().Be(1);
                currentNode = currentNode.Children[character];
            }
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