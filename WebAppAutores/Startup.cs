using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Servicios;

namespace WebAppAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // dependency injection
        // Services configuration  
        // authom resolves all the classes dependencies
        // what we define here gets instanciated properly with all config when used in a class
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(x => 
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // when ApplicationDbContext is a dep, it gets instanciated properly with all configs
            // Scope service by default 
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            // When someone needs an IServicio, we send ServicioA for default
            services.AddTransient<IServicio, ServicioA>();      // interfase and class that implements it
            //services.AddTransient<ServicioA>();    // just the class 
            //services.AddScope<ServicioA>();    // just the class 
            //services.AddSingleton<ServicioA>();    // just the class 

            // types of services and its differences 
            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScoped>();
            services.AddSingleton<ServicioSingleton>();


            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
