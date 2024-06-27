namespace CitySearch.Service.TreeSearchCityFinder
{
    public sealed class TreeSearchCityResult : ICityResult
    {
        public ICollection<string> NextLetters { get; set; } = Array.Empty<string>();
        public ICollection<string> NextCities { get; set; } =  Array.Empty<string>();

        internal static readonly TreeSearchCityResult Empty = new();
    }
}