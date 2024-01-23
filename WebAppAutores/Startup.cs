using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WebAppAutores.Filtros;
using WebAppAutores.Middlewares;
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
            // adding filter
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(MyExceptionFilter));
            }).AddJsonOptions(x => 
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
            services.AddTransient<MyActionFilter>();

            // actions to be executed when api starts/ends
            services.AddHostedService<WriteInFile>(); 

            services.AddResponseCaching(); // needed for the UseResponseCaching middleware

            // Microsoft.AspNetCore.Authentication.JwtBearer package needed for authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        // this method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            // MIDDLEWARES

            //app.UseMiddleware<LogResponseHTTPMiddleware>();  // calling custom Middleware

            // calling same custom middleware but using static extension class
            app.UseLogResponseHTTP(); 

            // It only runs on the specified route, so we can use a different
            // pipeline depends on the route
            app.Map("/ruta1", app =>
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Pipeline intercepted");
                });
            });

            //// creates a middleware and cuts the folowing ones execution
            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Pipeline intercepted");
            //});

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
