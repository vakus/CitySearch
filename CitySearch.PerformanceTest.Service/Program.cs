using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CitySearch;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.TreeSearchCityFinder;

BenchmarkRunner.Run<Benchmarks>();

[MemoryDiagnoser]
public class Benchmarks
{
    private ICityFinder finder;

    [Params("LA", "LOS", "GDA", "LON", "ZA", "J", "F")]
    public string CityPrefix { get; set; }

    [GlobalSetup(Target = nameof(BenchmarkRun))]
    public void Init()
    {
        finder = new TreeSearchCityFinder(new CsvCityNameLoader("CityNames.csv", new UppercaseInvariantCityNameNormaliser()),
            new UppercaseInvariantCityNameNormaliser());
    }

    [Benchmark]
    public void BenchmarkRun()
    {
        finder.Search(CityPrefix);
    }
}