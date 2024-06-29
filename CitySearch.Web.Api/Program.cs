using System.Reflection;
using CitySearch;
using CitySearch.Service.CityNameLoader;
using CitySearch.Service.CityNameNormaliser;
using CitySearch.Service.DatasetNormaliser;
using CitySearch.Service.TreeSearchCityFinder;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<ICityNameNormaliser, UppercaseInvariantCityNameNormaliser>();
builder.Services.AddSingleton<ICityNameLoader>(services =>
{
    var filename = builder.Configuration.GetConnectionString("CsvCityNameFile");;
    var nameNormaliser = services.GetRequiredService<ICityNameNormaliser>();
    return new CsvCityNameLoader(filename, nameNormaliser);
});

builder.Services.AddSingleton<NameDatasetNormaliser>();
builder.Services.AddSingleton<DuplicateDatasetNormaliser>();
builder.Services.AddSingleton<OrderDatasetNormaliser>();
builder.Services.AddSingleton<IDatasetNormaliser, AggregateDatasetNormaliser>(services =>
{
    var nameDatasetNormaliser = services.GetRequiredService<NameDatasetNormaliser>();
    var duplicateDatasetNormaliser = services.GetRequiredService<DuplicateDatasetNormaliser>();
    var orderDatasetNormaliser = services.GetRequiredService<OrderDatasetNormaliser>();
    return new AggregateDatasetNormaliser(
        nameDatasetNormaliser,
        duplicateDatasetNormaliser,
        orderDatasetNormaliser);
});
builder.Services.AddSingleton<ICityFinder, TreeSearchCityFinder>();

builder.Services.AddControllers()
    .AddControllersAsServices();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Configure the HTTP request pipeline.
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
