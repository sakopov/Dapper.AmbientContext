namespace Dapper.AmbientContext.Examples.WebApp
{
    public abstract class AbstractQuery
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        protected AbstractQuery(IAmbientDbContextLocator ambientDbContextLocator)
        {
            _ambientDbContextLocator = ambientDbContextLocator;
        }

        protected IAmbientDbContextQueryProxy Context => _ambientDbContextLocator.Get();
    }
}