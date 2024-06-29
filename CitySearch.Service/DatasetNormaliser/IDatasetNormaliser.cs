namespace CitySearch.Service.DatasetNormaliser;

public interface IDatasetNormaliser
{
    public IList<string> Normalise(IList<string> cities);
}