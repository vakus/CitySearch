using CitySearch;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.TreeSearchCityFinder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;



var serviceProvider = CreateServices();
var cityFinder = serviceProvider.GetRequiredService<ICityFinder>();
Console.WriteLine("Setup complete.");

while (true)
{
    Console.Write("Enter city name start to search [blank to exit]: ");
    var cityName = Console.ReadLine();
    if (string.IsNullOrEmpty(cityName))
    {
        break;
    }

    Console.WriteLine(JsonConvert.SerializeObject(cityFinder.Search(cityName)));
}

ServiceProvider CreateServices()
{
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddSingleton<ICityNameLoader, CsvCityNameLoader>();
    serviceCollection.AddSingleton<ICityNameNormaliser, UppercaseInvariantCityNameNormaliser>();
    serviceCollection.AddSingleton<ICityFinder, TreeSearchCityFinder>();
    return serviceCollection.BuildServiceProvider();
}