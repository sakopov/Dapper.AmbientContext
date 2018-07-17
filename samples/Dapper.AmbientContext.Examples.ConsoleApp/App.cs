using System.Threading.Tasks;

namespace Dapper.AmbientContext.Examples.ConsoleApp
{
    public class App
    {
        private readonly IAmbientDbContextFactory _ambientContextFactory;
        private readonly AnUpdateQuery _anUpdateQuery;
        private readonly AnInsertQuery _anInsertQuery;

        public App(
            IAmbientDbContextFactory ambientContextFactory, 
            AnUpdateQuery anUpdateQuery,
            AnInsertQuery anInsertQuery)
        {
            _ambientContextFactory = ambientContextFactory;
            _anUpdateQuery = anUpdateQuery;
            _anInsertQuery = anInsertQuery;
        }

        public async Task RunAsync()
        {
            // This will not actually work because there is no database to connect to.
            // Rather, this is a way to showcase how the library is supposed to be used.
            using (var ambientContext = _ambientContextFactory.Create())
            {
                await _anUpdateQuery.ExecuteAsync();
                await _anInsertQuery.ExecuteAsync();

                ambientContext.Commit();
            }
        }
    }
}