using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Dapper.AmbientContext.Examples.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IAmbientDbContextFactory _ambientContextFactory;
        private readonly AnUpdateQuery _anUpdateQuery;
        private readonly AnInsertQuery _anInsertQuery;

        public ValuesController(
            IAmbientDbContextFactory ambientContextFactory,
            AnUpdateQuery anUpdateQuery,
            AnInsertQuery anInsertQuery)
        {
            _ambientContextFactory = ambientContextFactory;
            _anUpdateQuery = anUpdateQuery;
            _anInsertQuery = anInsertQuery;
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]string value)
        {
            // This will not actually work because there is no database to connect to.
            // Rather, this is a way to showcase how the library is supposed to be used.
            using (var ambientContext = _ambientContextFactory.Create())
            {
                await _anUpdateQuery.ExecuteAsync();
                await _anInsertQuery.ExecuteAsync();

                ambientContext.Commit();
            }

            return NoContent();
        }
    }
}