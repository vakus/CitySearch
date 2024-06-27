namespace CitySearch.Service.CityNameNormaliser;

public class UppercaseInvariantCityNameNormaliser : ICityNameNormaliser
{
    public string Normalise(string cityName)
    {
        return cityName.ToUpperInvariant();
    }
}