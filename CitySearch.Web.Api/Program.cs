using System.Reflection;
using CitySearch;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.DatasetNormaliser;
using CitySearch.Service.TreeSearchCityFinder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ICityNameNormaliser, UppercaseInvariantCityNameNormaliser>();
builder.Services.AddSingleton<ICityNameLoader>(services =>
{
    var filename = builder.Configuration.GetConnectionString("CsvCityNameFile");
    ArgumentException.ThrowIfNullOrWhiteSpace(filename);
    return new CsvCityNameLoader(filename);
});

builder.Services.AddSingleton<NameDatasetNormaliser>();
builder.Services.AddSingleton<DuplicateDatasetNormaliser>();
builder.Services.AddSingleton<IDatasetNormaliser, AggregateDatasetNormaliser>(services =>
{
    var nameDatasetNormaliser = services.GetRequiredService<NameDatasetNormaliser>();
    var duplicateDatasetNormaliser = services.GetRequiredService<DuplicateDatasetNormaliser>();
    return new AggregateDatasetNormaliser(
        nameDatasetNormaliser,
        duplicateDatasetNormaliser);
});
builder.Services.AddSingleton<ICityFinder, TreeSearchCityFinder>();

builder.Services.AddControllers()
    .AddControllersAsServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "City Search API",
        Description = "Simple service to provide predictive city name completion",
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseAuthorization();

app.MapControllers();

app.Run();


// make implicit Program class for test projects to be able to access it
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public partial class Program
{
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member