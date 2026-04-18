using System.Threading.Tasks;

namespace Dapper.AmbientContext.Examples.WebApp
{
    public class AnInsertQuery : AbstractQuery
    {
        public AnInsertQuery(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }

        public async Task ExecuteAsync()
        {
            var prepared = await Context.PrepareAsync();
            await prepared.Connection.ExecuteAsync("INSERT INTO SomeTable (SomeColumn) VALUES (SomeColumn = 'inserted');", transaction: prepared.Transaction);
        }
    }
}