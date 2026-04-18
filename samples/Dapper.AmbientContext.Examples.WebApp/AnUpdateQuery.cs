using System.Threading.Tasks;

namespace Dapper.AmbientContext.Examples.WebApp
{
    public class AnUpdateQuery : AbstractQuery
    {
        public AnUpdateQuery(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }

        public async Task ExecuteAsync()
        {
            var prepared = await Context.PrepareAsync();
            await prepared.Connection.ExecuteAsync("UPDATE SomeTable SET SomeColumn = 'updated';", transaction: prepared.Transaction);
        }
    }
}