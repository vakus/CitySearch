using CitySearch;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.DatasetNormaliser;
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

static ServiceProvider CreateServices()
{
    var serviceCollection = new ServiceCollection();
    
    serviceCollection.AddSingleton<ICityNameNormaliser, UppercaseInvariantCityNameNormaliser>();
    serviceCollection.AddSingleton<ICityNameLoader>(services =>
    {
        var filename = "CityNames.csv";
        var nameNormaliser = services.GetRequiredService<ICityNameNormaliser>();
        return new CsvCityNameLoader(filename, nameNormaliser);
    });
    serviceCollection.AddSingleton<NameDatasetNormaliser>();
    serviceCollection.AddSingleton<DuplicateDatasetNormaliser>();
    serviceCollection.AddSingleton<OrderDatasetNormaliser>();
    serviceCollection.AddSingleton<IDatasetNormaliser, AggregateDatasetNormaliser>(services =>
    {
        var nameDatasetNormaliser = services.GetRequiredService<NameDatasetNormaliser>();
        var duplicateDatasetNormaliser = services.GetRequiredService<DuplicateDatasetNormaliser>();
        var orderDatasetNormaliser = services.GetRequiredService<OrderDatasetNormaliser>();
        return new AggregateDatasetNormaliser(
            nameDatasetNormaliser,
            duplicateDatasetNormaliser,
            orderDatasetNormaliser);
    });
    serviceCollection.AddSingleton<ICityFinder, TreeSearchCityFinder>();
    return serviceCollection.BuildServiceProvider();
}