namespace CitySearch.Service.DatasetNormaliser;

public class AggregateDatasetNormaliser : IDatasetNormaliser
{
    private readonly IList<IDatasetNormaliser> _normalisers;

    public AggregateDatasetNormaliser(params IDatasetNormaliser[] normalisers)
    {
        this._normalisers = normalisers;
    }

    public IList<string> Normalise(IList<string> cities)
    {
        foreach (var normaliser in this._normalisers)
        {
            cities = normaliser.Normalise(cities);
        }

        return cities;
    }
}