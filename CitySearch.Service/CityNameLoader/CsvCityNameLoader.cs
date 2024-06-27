using CitySearch.Service.CityNameNormaliser;

namespace CitySearch.Service.CityNameLoader;

public class CsvCityNameLoader : ICityNameLoader
{
    private const string CityNameFile = "CityNames.csv";

    private readonly ICityNameNormaliser _cityNameNormaliser;

    public CsvCityNameLoader(ICityNameNormaliser cityNameNormaliser)
    {
        ArgumentNullException.ThrowIfNull(cityNameNormaliser);

        _cityNameNormaliser = cityNameNormaliser;
    }
    public IList<string> Load()
    {
        return File.ReadAllLines(CityNameFile).Select(_cityNameNormaliser.Normalise).ToList();
    }
}