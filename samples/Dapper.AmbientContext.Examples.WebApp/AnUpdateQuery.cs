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
            await Context.ExecuteAsync("UPDATE SomeTable SET SomeColumn = 'updated';");
        }
    }
}