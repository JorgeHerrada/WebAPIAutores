﻿using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebAppAutores.Filtros;
using WebAppAutores.Middlewares;
using WebAppAutores.Servicios;
using WebAppAutores.Utilities;

namespace WebAppAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // clears the mapping on the claims keys
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 
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
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).
                AddNewtonsoftJson(); // for the HTTP patch that uses JsonPatchDocument

            // when ApplicationDbContext is a dep, it gets instanciated properly with all configs
            // Scope service by default 
            services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

             // Microsoft.AspNetCore.Authentication.JwtBearer package needed for authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["keyjwt"])),
                    ClockSkew = TimeSpan.Zero
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIAutores", Version = "v1" });
                c.OperationFilter<AddParamHATEOAS>(); // filter to add the includeHATEOAS param

                // Configure swagger to send JWT
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            services.AddAutoMapper(typeof(Startup));

            // configure Identity 
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // add policy for EsAdmin
            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
                //opciones.AddPolicy("EsVendedor", politica => politica.RequireClaim("esVendedor"));
            });

            // access to data protection services (for encryption)
            services.AddDataProtection();

            // hash service 
            services.AddTransient<HashService>();

            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        // this method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger logger)
        {
            // MIDDLEWARES

            // calling same custom middleware but using static extension class
            app.UseLogResponseHTTP();  

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
