# CitySearch
[![CI/CD](https://github.com/vakus/CitySearch/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/vakus/CitySearch/actions/workflows/dotnet.yml)

CitySearch is an algorithm provided to quickly search through thousands of city names, aimed to provide autocompletion for city names.
Additionally to the Algorithm provided, this project also provides two sample usages:
- [CitySearch.Web.Api](CitySearch.Web.Api) - An WebAPI, this can be used to integrate with existing services.
- [CitySearch.Console](CitySearch.Console) - Development Console, this can be used by developers to manually test and debug issues within the application.

The project also contains a number of automatic tests
- [CitySearch.UnitTest.Service](CitySearch.UnitTest.Service) project contains UnitTests covering the search algorithm.
- [CitySearch.PerformanceTest.Service](CitySearch.PerformanceTest.Service) project contains small set of performance tests.
- [CitySearch.Acceptance.Service](CitySearch.Acceptance.Service) project contains outlined acceptance tests.
- [CitySearch.IntegrationTest.Web.Api](CitySearch.IntegrationTest.Web.Api) project contains set of integration tests against provided WebAPI.

Additional projects contained within this repository:
- [CitySearch.Contract](CitySearch.Contract) - Third party interfaces which must be implemented for integration purposes.
- [CitySearch.Service](CitySearch.Service) - The main algorithm implementation.

# Continuous Integration and Continuous Deployment (CI/CD)

This repository is combined with CI/CD pipeline which works as following:
- Whenever code is pushed to master branch, or a Pull Request is created
	- The code is built in Release configuration
	- The code is tested using the provided test suite
	- The code is packed into an Artifact which can be used for deployment.
- If the code was pushed or merged to the master branch, and previous steps have been completed successfully
	- An update to Azure resource group is made using Azure Resource Manager (ARM) with [template contained within repository](Azure/template.json)
	- The packed Artifact is then uploaded to service in Azure.

This CI/CD pipeline can be [viewed here](.github/workflows/dotnet.yml)

# Deployed instance

An instance of this API is deployed onto Azure under publicly accessible

https://interviewcitysearch.azurewebsites.net

Queries can be created by adding a prefix to `/api/citysearch/` endpoint, for example:

- [Search "LON" `https://interviewcitysearch.azurewebsites.net/api/citysearch/lon`](https://interviewcitysearch.azurewebsites.net/api/citysearch/lon)
- [Search "KRA" `https://interviewcitysearch.azurewebsites.net/api/citysearch/kra`](https://interviewcitysearch.azurewebsites.net/api/citysearch/kra)

# Performance

Results from [performance test](CitySearch.PerformanceTest.Service) provided within repository made on 30/06/2024, using full dataset are shown below.

| Method       | CityPrefix | Mean        | Error      | StdDev     | Gen0   | Allocated |
|------------- |----------- |------------:|-----------:|-----------:|-------:|----------:|
| BenchmarkRun | F          | 8,422.79 ns | 155.738 ns | 145.677 ns | 0.1373 |    7240 B |
| BenchmarkRun | GDA        |    55.98 ns |   0.960 ns |   0.898 ns | 0.0044 |     224 B |
| BenchmarkRun | J          | 8,356.81 ns | 163.309 ns | 152.759 ns | 0.1221 |    6400 B |
| BenchmarkRun | LA         | 8,311.74 ns |  90.636 ns |  75.685 ns | 0.1221 |    6768 B |
| BenchmarkRun | LON        |   789.32 ns |  12.488 ns |  10.428 ns | 0.0210 |    1096 B |
| BenchmarkRun | LOS        |   485.50 ns |   8.664 ns |   8.897 ns | 0.0134 |     704 B |
| BenchmarkRun | ZA         | 1,983.13 ns |  28.238 ns |  25.032 ns | 0.0458 |    2320 B |

# City Dataset

The city names dataset has been obtained from [https://simplemaps.com](https://simplemaps.com/data/world-cities)
under [Creative Commons Attribution 4.0 license.](https://creativecommons.org/licenses/by/4.0/)
The dataset provided was modified to only contain ASCII names of cities.
You can [view full license here](CityNames-license.txt)