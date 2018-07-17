using Dapper.AmbientContext.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dapper.AmbientContext.Examples.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AmbientDbContextStorageProvider.SetStorage(new AsyncLocalContextStorage());

            services.AddSingleton<IDbConnectionFactory>(provider => new SqlServerConnectionFactory("your connection string"));
            services.AddSingleton<IAmbientDbContextFactory, AmbientDbContextFactory>();
            services.AddTransient<IAmbientDbContextLocator, AmbientDbContextLocator>();

            services.AddTransient<AnUpdateQuery>();
            services.AddTransient<AnInsertQuery>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}