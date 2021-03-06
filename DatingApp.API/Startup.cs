﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API
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
            // giving a MySql config for Production mode
            // the ConfigureWarnings part is included to ignore the MySql's Include warnings when the app is run
            // https://docs.microsoft.com/en-us/ef/core/querying/related-data#ignored-includes
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontextoptionsbuilder.configurewarnings?view=efcore-2.1
            // Section 17 Lecture 182
            services.AddDbContext<DataContext>(x => 
            x.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning)));

            // the foll.line of code are part of normal DBContext code in an ASP.NET Web App, say. Unsure of another way 
            // to add this code other than physically type it out in a ASP.NET Web API
            // plus the Configuration. is part of the Configuration declared in the ctor
            // -- Note: This particular config was commented out for Section 17 Lecture 181, when we swapped SQLite for MySql (above)
            // The instructor wanted to copy the entire ConfigureServices method, paste it with a new name called ConfigureDevelopmentServices
            // because MVC would recognize the config as the config for Development mode, based on convention based naming
            // But I was having trouble trying to get the application to take this particular ConfigureServices method for Production, so I am
            // just commenting the below line out instead of having another separate ConfigureDevelopmentServices
            // services.AddDbContext<DataContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddJsonOptions(opt => {
                // to bypass the self referencing loop error between user reference in photo and photo reference in user, for eg. 
                // Json.NET will ignore objects in reference loops and not serialize them. The first time an object is encountered it will be serialized 
                // as usual but if the object is encountered as a child object of itself the serializer will skip serializing it.
                // Link: https://stackoverflow.com/questions/11979637/what-does-referenceloophandling-ignore-in-newtonsoft-json-exactly-do
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddCors();

            // for cloudinary, the configure maps the properties in the CloudinarySettings class with the appsettings part
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

            services.AddAutoMapper();
            
            // Transient lifetime services are created each time they're requested. This lifetime works best for lightweight, stateless services.
            services.AddTransient<Seed>(); // adding the data seed part as a service

            // service created once per request in the current scope
            services.AddScoped<IAuthRepository, AuthRepository>();

            services.AddScoped<IDatingRepository, DatingRepository>();

            // Section 3, Lecture 34
            // letting the system know what authentication we need if we put the annotation [Authorize] in our controller
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options => {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes( Configuration.GetSection("AppSettings:Token").Value)),
                            ValidateIssuer = false,
                            ValidateAudience = false
                        };
                    });
            
            services.AddScoped<LogUserActivity>(); // ActionFilter has to be registered here as a service in order to use it in our controller
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment()) // development mode
            {
                app.UseDeveloperExceptionPage();
            }
            else // in production mode
            {
                // section 5 lecture 49
                // handling error globally by asp core rather than tediously decorating all the code with try catch blocks
                // builder because UseExceptionHandler returns IApplicationBuilder
                // accessing context inside the Run method
                app.UseExceptionHandler(builder => {
                    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.requestdelegate?view=aspnetcore-2.1
                    // The Run method takes a RequestDelegate, which is a delegate that in turn takes a HTTPContext (see link above). That's why the 'context' variable below is 
                    // automatically understood by the compiler as an HttpContext variable.
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;// we can control the status code here

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            // Read this to see how extensions work: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
                            context.Response.AddApplicationError(error.Error.Message); // from Extensions class                            
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                // app.UseHsts();
            }

            //seeder.SeedUsers(); // just for the seeding data part

            // app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            
            // this basically makes the application to run the default file like index.html (in our case) or default.aspx, etc.
            // Setting a default home page provides visitors a logical starting point when visiting your site.
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-2.2#serve-a-default-document
            app.UseDefaultFiles(); 
            
            // Static files, such as HTML, CSS, images, and JavaScript, are assets an ASP.NET Core app serves directly to clients.
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-2.2
            app.UseStaticFiles(); // looks inside the wwwroot folder and serves the content from there
            
            // middleware, sits between client request and API end point. We give this configuration so that mvc knows the routes of the SPA
            // for eg: visiting localhost:5000/members would mean that the API would know the SPA route /members is where it should be redirected
            // i.e. It's for when you want to handle the 404 within your front-end application (let all Routes go through so your front-end can handle them)
            // https://github.com/aspnet/JavaScriptServices/issues/973
            app.UseMvc(routes => {
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new {controller = "Fallback", action = "Index"}
                );
            }); 
        }
    }
}
