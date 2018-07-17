using System;
using System.Threading.Tasks;
using Dapper.AmbientContext.Storage;
using SimpleInjector;

namespace Dapper.AmbientContext.Examples.ConsoleApp
{
    class Program
    {
        private static void Main(string[] args)
        {
            AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());

            Task.Run(() => MainAsync(args)).Wait();
        }

        private static async Task MainAsync(string[] args)
        {
            var container = ConfigureContainer();

            var app = container.GetInstance<App>();

            await app.RunAsync();
        }

        private static Container ConfigureContainer()
        {
            var container = new Container();

            container.RegisterSingleton<IDbConnectionFactory>(() => new SqlServerConnectionFactory("Connection-String-Here"));
            container.RegisterSingleton<IAmbientDbContextFactory, AmbientDbContextFactory>();
            container.Register<IAmbientDbContextLocator, AmbientDbContextLocator>();

            container.Register<App>();
            container.Register<AnUpdateQuery>();
            container.Register<AnInsertQuery>();

            return container;
        }
    }
}