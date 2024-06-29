using CitySearch.Service.CityNameNormaliser;

namespace CitySearch.Service.DatasetNormaliser;

public sealed class NameDatasetNormaliser : IDatasetNormaliser
{
    private readonly ICityNameNormaliser _cityNameNormaliser;

    public NameDatasetNormaliser(ICityNameNormaliser cityNameNormaliser)
    {
        this._cityNameNormaliser = cityNameNormaliser;
    }

    public IList<string> Normalise(IList<string> cities)
    {
        return cities.Select(c => this._cityNameNormaliser.Normalise(c)).ToList();
    }
}