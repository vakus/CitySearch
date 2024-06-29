using CitySearch.Service.CityNameNormaliser;

namespace CitySearch.Service.CityNameLoader;

public class CsvCityNameLoader : ICityNameLoader
{
    private readonly string _cityNameFile;

    private readonly ICityNameNormaliser _cityNameNormaliser;

    public CsvCityNameLoader(string csvFileName, ICityNameNormaliser cityNameNormaliser)
    {
        ArgumentNullException.ThrowIfNull(cityNameNormaliser);
        ArgumentException.ThrowIfNullOrWhiteSpace(csvFileName);

        this._cityNameNormaliser = cityNameNormaliser;
        this._cityNameFile = csvFileName;
    }
    public IList<string> Load()
    {
        return File.ReadAllLines(this._cityNameFile)
            .Select(this._cityNameNormaliser.Normalise)
            .ToList();
    }
}