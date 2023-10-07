using App_Xamarin_Firebase.Authentication;
using App_Xamarin_Firebase.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App_Xamarin_Firebase
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
            services.AddControllers();
            // Inicializar FirebaseApp
            var firebaseApp = FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("firebase-config.json"),
            });

            // Agregar FirebaseApp a los servicios
            services.AddSingleton(firebaseApp);

            // Inicializar FirebaseAuth
            var auth = FirebaseAuth.GetAuth(firebaseApp);

            // Agregar FirebaseAuth a los servicios
            services.AddSingleton(auth);

            services.AddScoped<RecipeService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddScheme<JwtBearerOptions, FirebaseAuthenticationHandler>(JwtBearerDefaults.AuthenticationScheme, options =>
               {
                   options.Authority = "https://securetoken.google.com/net-firebase-b8026";
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuer = "https://securetoken.google.com/net-firebase-b8026",
                       ValidateAudience = false,
                       ValidateLifetime = true
                   };
                   options.RequireHttpsMetadata = false; // Solo para desarrollo
                   options.SaveToken = true;
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
