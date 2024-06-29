namespace CitySearch.Service.DatasetNormaliser;

public sealed class DuplicateDatasetNormaliser : IDatasetNormaliser
{
    public IList<string> Normalise(IList<string> cities)
    {
        return cities.Distinct().ToList();
    }
}