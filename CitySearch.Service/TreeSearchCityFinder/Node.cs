﻿namespace CitySearch.Service.TreeSearchCityFinder
{
    public class Node
    {
        public int Start { get; init; }
        public int Length { get; set; }
        public IDictionary<char, Node> Children { get; } = new Dictionary<char, Node>();
    }
}
