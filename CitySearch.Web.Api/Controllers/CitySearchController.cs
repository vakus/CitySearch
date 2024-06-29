using Microsoft.AspNetCore.Mvc;

namespace CitySearch.Web.Api.Controllers
{
    [ApiController]
    public sealed class CitySearchController : Controller
    {
        private readonly ICityFinder _cityFinder;

        public CitySearchController(ICityFinder cityFinder)
        {
            this._cityFinder = cityFinder;
        }
        /// <summary>
        /// List known cities with specified prefix
        /// </summary>
        /// <param name="prefix">prefix of the city</param>
        /// <returns>List of possible next characters and all cities which start with the prefix</returns>
        /// <response code="500">Internal Server Error</response>
        /// <response code="400">Prefix has not been provided</response>
        /// <response code="200">Returns list of all possible cities starting with prefix and list of next characters which can be used.</response>
        [HttpGet("/api/CitySearch/{prefix}")]
        [Produces("application/json")]
        public IActionResult Search(string prefix)
        {
            return Ok(this._cityFinder.Search(prefix));
        }
    }
}
