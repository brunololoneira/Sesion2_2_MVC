using Amigos.DataAccessLayer;
using Amigos.Models;
using Amigos.Models.TuProyecto.Models;
using Amigos.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Amigos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Registro del IHttpContextAccessor
            builder.Services.AddHttpContextAccessor();

            var supportedCultures = new[] { new CultureInfo("es-ES"), new CultureInfo("en-US") };
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("es-ES");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };

            });

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();


            // Se crea una única instancia de IncImpl que se reutilizará en toda la aplicación

            builder.Services.AddSingleton<IInc, IncImpl>(); // Esta opción incrementa el contador cada vez que se recarga la página en el navegador

            // builder.Services.AddTransient<IInc, IncImpl>(); // Esta opción crea una nueva instancia cada vez que se recarga la página manteniendo el contador a 1 en todas las ocasiones

            // Añadir soporte para la base de datos SQLite
            builder.Services.AddDbContext<AmigoDBContext>(options =>
                options.UseSqlite("Data Source=Amigos.db"));



            var app = builder.Build();

            // Prueba: Insertar un amigo al iniciar la aplicación
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AmigoDBContext>();

                // Asegurarse de que la base de datos esté creada
                dbContext.Database.EnsureCreated();

                // Insertar un amigo si la tabla está vacía
                if (!dbContext.Amigos!.Any())
                {
                    dbContext.Amigos.Add(new Amigo { Name = "Carlos", Longi = "40.4168", Lati = "-3.7038" });
                    dbContext.SaveChanges();
                }

                // Leer y mostrar en consola
                var amigos = dbContext.Amigos.ToList();
                foreach (var amigo in amigos)
                {
                    Console.WriteLine($"Amigo: {amigo.Name}, Ubicación: ({amigo.Lati}, {amigo.Longi})");
                }
            }

            // ----------------------------------------------------------------------------------------------------------------------------------

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseRequestLocalization(); // Esto debe estar antes de UseEndpoints() y otras configuraciones

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Amigo}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Prueba",
                pattern: "{controller}/{action}/{valor}/{veces}");

            app.Run();
        }
    }
}
