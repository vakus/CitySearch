﻿using System.Collections.Immutable;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;

namespace CitySearch.Service.TreeSearchCityFinder;

public sealed class TreeSearchCityFinder : ICityFinder
{
    private readonly IList<string> _cities;
    private readonly Node _root;
    private readonly ICityNameNormaliser _cityNameNormaliser;
    private readonly int _longestCityName;

    public TreeSearchCityFinder(ICityNameLoader cityNameLoader, ICityNameNormaliser cityNameNormaliser)
    {
        ArgumentNullException.ThrowIfNull(cityNameNormaliser);
        ArgumentNullException.ThrowIfNull(cityNameLoader);
        
        this._cityNameNormaliser = cityNameNormaliser;
        this._cities = cityNameLoader.Load()
            .OrderBy(name => name)
            .ToImmutableList();

        this._root = new Node
        {
            Start = 0,
            Length = this._cities.Count,
        };
        this.GenerateNodeTreeFromCities();
        this._longestCityName = _cities.Max(city => city.Length);
    }

    public ICityResult Search(string searchString)
    {
        if (searchString.Length > _longestCityName)
        {
            return TreeSearchCityResult.Empty;
        }

        searchString = _cityNameNormaliser.Normalise(searchString);

        Node? resultNode = this.FindNodeForName(searchString);

        if (resultNode is null)
        {
            return TreeSearchCityResult.Empty;
        }

        var cityList = new List<string>(resultNode.Length);
        var cityEnd = resultNode.Start + resultNode.Length;
        for (var x = resultNode.Start; x < cityEnd; x++)
        {
            cityList.Add(_cities[x]);
        }

        var letterList = new List<string>(resultNode.Children.Count);
        foreach (var key in resultNode.Children.Keys)
        {
            letterList.Add(key.ToString());
        }

        return new TreeSearchCityResult()
        {
            NextCities = cityList,
            NextLetters = letterList,
        };
    }

    private Node? FindNodeForName(string cityName)
    {
        var currentNode = _root;

        for (int i = 0; i < cityName.Length; i++)
        {
            if (!currentNode.Children.TryGetValue(cityName[i], out currentNode))
            {
                return null;
            }
        }

        return currentNode;
    }

    private void GenerateNodeTreeFromCities()
    {
        for (var index = 0; index < _cities.Count; index++)
        {
            var city = _cities[index].AsSpan();
            var currentNode = _root;
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
    }
}