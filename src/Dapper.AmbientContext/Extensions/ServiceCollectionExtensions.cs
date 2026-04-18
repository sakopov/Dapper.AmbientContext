using System;
using Dapper.AmbientContext.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dapper.AmbientContext.Extensions
{
    /// <summary>
    /// Extension methods for configuring Dapper.AmbientContext services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Dapper.AmbientContext services to the specified <see cref="IServiceCollection"/> using a connection factory instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="connectionFactory">The database connection factory instance.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="connectionFactory"/> is null.</exception>
        /// <remarks>
        /// This method automatically configures the appropriate storage provider based on the target framework:
        /// <list type="bullet">
        /// <item><description><see cref="AsyncLocalContextStorage"/> for .NET Core, .NET 5+</description></item>
        /// <item><description><see cref="LogicalCallContextStorage"/> for .NET Framework</description></item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddDapperAmbientContext(
            this IServiceCollection services,
            IDbConnectionFactory connectionFactory)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            ConfigureStorage();

            services.AddSingleton(connectionFactory);
            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();
            services.AddTransient<IAmbientDbContextFactory, AmbientDbContextFactory>();

            return services;
        }

        /// <summary>
        /// Adds Dapper.AmbientContext services to the specified <see cref="IServiceCollection"/> using a connection factory type.
        /// </summary>
        /// <typeparam name="TConnectionFactory">The type of database connection factory to register.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
        /// <param name="connectionString">The database connection string to pass to the factory constructor.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="connectionString"/> is null or empty.</exception>
        /// <remarks>
        /// This method automatically configures the appropriate storage provider based on the target framework:
        /// <list type="bullet">
        /// <item><description><see cref="AsyncLocalContextStorage"/> for .NET Core, .NET 5+</description></item>
        /// <item><description><see cref="LogicalCallContextStorage"/> for .NET Framework</description></item>
        /// </list>
        /// The factory type must have a constructor that accepts a connection string parameter.
        /// </remarks>
        public static IServiceCollection AddDapperAmbientContext<TConnectionFactory>(
            this IServiceCollection services,
            string connectionString)
            where TConnectionFactory : class, IDbConnectionFactory
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            ConfigureStorage();

            services.TryAddSingleton<TConnectionFactory>();
            services.AddSingleton<IDbConnectionFactory>(sp =>
            {
                var factory = ActivatorUtilities.CreateInstance<TConnectionFactory>(sp, connectionString);
                return factory;
            });
            services.AddSingleton<IAmbientDbContextLocator, AmbientDbContextLocator>();
            services.AddTransient<IAmbientDbContextFactory, AmbientDbContextFactory>();

            return services;
        }

        private static void ConfigureStorage()
        {
#if NET462
            AmbientDbContextStorageProvider.SetStorage(new LogicalCallContextStorage());
#else
            AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());
#endif
        }
    }
}
