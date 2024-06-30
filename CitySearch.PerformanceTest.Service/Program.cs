using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.DatasetNormaliser;
using CitySearch.Service.TreeSearchCityFinder;

BenchmarkRunner.Run<Benchmarks>();

[MemoryDiagnoser]
public class Benchmarks
{
    private TreeSearchCityFinder _finder;

    [Params("LA", "LOS", "GDA", "LON", "ZA", "J", "F")]
    public string CityPrefix { get; set; }

    [GlobalSetup(Target = nameof(BenchmarkRun))]
    public void Init()
    {
        var nameNormaliser = new UppercaseInvariantCityNameNormaliser();
        this._finder = new TreeSearchCityFinder(
            new CsvCityNameLoader("CityNames.csv"),
            nameNormaliser,
            new AggregateDatasetNormaliser(
                new NameDatasetNormaliser(nameNormaliser),
                new DuplicateDatasetNormaliser()));
    }

    [Benchmark]
    public void BenchmarkRun()
    {
        this._finder.Search(this.CityPrefix);
    }
}