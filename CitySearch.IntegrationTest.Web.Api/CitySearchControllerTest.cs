using CitySearch.Service.TreeSearchCityFinder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace CitySearch.IntegrationTest.Web.Api
{
    public class CitySearchControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public CitySearchControllerTest(WebApplicationFactory<Program> application)
        {
            this._client = application.CreateClient();
        }

        [Theory]
        [InlineData("lon", "LONDON")]
        [InlineData("lit", "LITTLE ROCK")]
        [InlineData("ok", "OKAZAKI")]
        [InlineData("ku", "KURE")]
        [InlineData("kelo", "KELOWNA")]
        [InlineData("TERR", "TERREBONNE")]
        [InlineData("LAG", "LAGHOUAT")]
        [InlineData("GD", "GDANSK")]
        [InlineData("SONGKHLA", "SONGKHLA")]
        [InlineData("PRUSZKOW", "PRUSZKOW")]
        [InlineData("Royal", "ROYAL TUNBRIDGE WELLS")]
        public async Task Search_ReturnsValidCities_WhenPrefixProvided(string search, string expected)
        {
            var response = await this._client.GetAsync($"/api/CitySearch/{search}");
            response.Should().BeSuccessful();

            var result = await response.Content.ReadAsStringAsync();
            var cityResult = JsonConvert.DeserializeObject<TreeSearchCityResult>(result);
            cityResult.Should().NotBeNull();
            cityResult.NextCities.Should().NotBeNull()
                .And.NotBeEmpty()
                .And.Contain(expected)
                .And.AllSatisfy(city => city.Should().StartWithEquivalentOf($"{search}"));

            if (search.Length != expected.Length)
            {
                cityResult.NextLetters.Should().NotBeNull()
                    .And.NotBeEmpty()
                    .And.Contain(expected[search.Length].ToString());
            }
        }

        [Fact]
        public async Task Search_ReturnsClientError_WhenPrefixIsSkipped()
        {
            var response = await this._client.GetAsync($"/api/CitySearch/");
            response.Should().HaveClientError("City prefix must be supplied.");
        }
    }
}