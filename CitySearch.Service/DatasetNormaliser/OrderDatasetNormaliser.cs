namespace CitySearch.Service.DatasetNormaliser;

public class OrderDatasetNormaliser : IDatasetNormaliser
{
    public IList<string> Normalise(IList<string> cities)
    {
        return cities.OrderBy(c => c).ToList();
    }
}