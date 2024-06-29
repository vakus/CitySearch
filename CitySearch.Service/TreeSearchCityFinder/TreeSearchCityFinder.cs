using System.Collections.Immutable;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.DatasetNormaliser;

namespace CitySearch.Service.TreeSearchCityFinder;

public sealed class TreeSearchCityFinder : ICityFinder
{
    private readonly IList<string> _cities;
    private readonly Node _root;
    private readonly ICityNameNormaliser _cityNameNormaliser;
    private readonly int _longestCityName;

    public TreeSearchCityFinder(
        ICityNameLoader cityNameLoader,
        ICityNameNormaliser cityNameNormaliser,
        IDatasetNormaliser datasetNormaliser)
    {
        ArgumentNullException.ThrowIfNull(cityNameNormaliser);
        ArgumentNullException.ThrowIfNull(cityNameLoader);
        ArgumentNullException.ThrowIfNull(datasetNormaliser);
        
        this._cityNameNormaliser = cityNameNormaliser;
        this._cities = datasetNormaliser.Normalise(cityNameLoader.Load())
            .ToImmutableList();

        this._root = GenerateNodeTreeFromCities(this._cities);
        if (this._cities.Any())
        {
            this._longestCityName = this._cities.Max(city => city.Length);
        }
        
    }

    public ICityResult Search(string searchString)
    {
        return TreeSearchCityResult.Empty;
    }

    private Node? FindNodeForName(string cityName)
    {
        var currentNode = this._root;

        for (int i = 0; i < cityName.Length; i++)
        {
            if (!currentNode.Children.TryGetValue(cityName[i], out currentNode))
            {
                return null;
            }
        }

        return currentNode;
    }

    internal static Node GenerateNodeTreeFromCities(IList<string> cities)
    {
        var root = new Node
        {
            Start = 0,
            Length = cities.Count,
        };

        for (var index = 0; index < cities.Count; index++)
        {
            var city = cities[index].AsSpan();
            var currentNode = root;
            foreach (var character in city)
            {
                if (!currentNode.Children.TryGetValue(character, out var node))
                {
                    node = new Node
                    {
                        Start = index
                    };
                    currentNode.Children[character] = node;
                }

                currentNode = node;
                currentNode.Length++;
            }
        }

        return root;
    }
}