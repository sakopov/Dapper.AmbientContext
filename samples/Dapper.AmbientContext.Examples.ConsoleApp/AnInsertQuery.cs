using System.Threading.Tasks;

namespace Dapper.AmbientContext.Examples.ConsoleApp
{
    public class AnInsertQuery : AbstractQuery
    {
        public AnInsertQuery(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }

        public async Task ExecuteAsync()
        {
            await Context.ExecuteAsync("INSERT INTO SomeTable (SomeColumn) VALUES (SomeColumn = 'inserted');");
        }
    }
}