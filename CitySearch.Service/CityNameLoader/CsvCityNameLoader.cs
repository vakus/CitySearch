namespace CitySearch.Service.CityNameLoader;

public class CsvCityNameLoader : ICityNameLoader
{
    private readonly string _cityNameFile;

    public CsvCityNameLoader(string csvFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(csvFileName);

        this._cityNameFile = csvFileName;
    }
    public IList<string> Load()
    {
        return File.ReadAllLines(this._cityNameFile)
            .ToList();
    }
}