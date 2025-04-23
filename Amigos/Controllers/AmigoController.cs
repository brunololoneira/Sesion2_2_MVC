using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Amigos.DataAccessLayer;
using Amigos.Models.TuProyecto.Models;
using Amigos.Models;
using System.Diagnostics;

namespace Amigos.Controllers
{
    public class AmigoController : Controller
    {
        private readonly AmigoDBContext _context;

        public AmigoController(AmigoDBContext context)
        {
            _context = context;
        }

        // GET: Amigo
        public async Task<IActionResult> Index()
        {
            return View(await _context.Amigos.ToListAsync());
        }

        // GET: Amigo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (amigo == null)
            {
                return NotFound();
            }

            return View(amigo);
        }

        // GET: Amigo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Amigo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Longi,Lati")] Amigo amigo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(amigo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(amigo);
        }

        // GET: Amigo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos.FindAsync(id);
            if (amigo == null)
            {
                return NotFound();
            }
            return View(amigo);
        }

        // POST: Amigo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Longi,Lati")] Amigo amigo)
        {
            if (id != amigo.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(amigo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AmigoExists(amigo.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(amigo);
        }

        // GET: Amigo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var amigo = await _context.Amigos
                .FirstOrDefaultAsync(m => m.ID == id);
            if (amigo == null)
            {
                return NotFound();
            }

            return View(amigo);
        }

        // POST: Amigo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var amigo = await _context.Amigos.FindAsync(id);
            if (amigo != null)
            {
                _context.Amigos.Remove(amigo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AmigoExists(int id)
        {
            return _context.Amigos.Any(e => e.ID == id);
        }
        // GET: Amigo/Filter
        public IActionResult Filter()
        {
            return View();  // Muestra la vista para introducir los parámetros de filtrado
        }

        // GET: Amigo/FilterResults
        public async Task<IActionResult> FilterResults(double? latitude, double? longitude, double? distance)
        {
            // Verifica que los parámetros no sean nulos
            if (!latitude.HasValue || !longitude.HasValue || !distance.HasValue)
            {
                return RedirectToAction(nameof(Index));  // Redirige a la página de inicio si faltan parámetros
            }

            // Obtén todos los amigos de la base de datos (sin filtro de distancia)
            var amigos = await _context.Amigos.ToListAsync();
            try
            {
                var amigosFiltrados = amigos
                    .Where(a =>
                    {
                        double calcDistance = CalculateDistance(latitude.Value, longitude.Value, a.Lati, a.Longi);
                        Console.WriteLine($"Distance between {latitude.Value},{longitude.Value} and {a.Lati},{a.Longi}: {calcDistance} km");
                        return calcDistance <= distance.Value;
                    })
                    .ToList();

                return View(amigosFiltrados);
            }
            catch (Exception ex)
            {
                // Manejar excepciones y posiblemente registrar el error
                // Aquí puedes agregar un mensaje de error más amigable para el usuario
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        private double CalculateDistance(double latitude, double longitude, string? friendLatitude, string? friendLongitude)
        {
            // Usar CultureInfo.InvariantCulture para asegurar que el punto se use como separador decimal
            if (!double.TryParse(friendLatitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lat2) ||
                !double.TryParse(friendLongitude, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lon2))
            {
                // Si la conversión falla, puedes devolver un valor que indique un error o una distancia infinita.
                return double.MaxValue; // Indicando que esta ubicación es muy lejana
            }

            // Fórmula de Haversine para calcular la distancia entre dos puntos geográficos en km
            double R = 6371; // Radio de la Tierra en kilómetros
            double lat1 = ConvertToRadians(latitude); // Asegúrate que lat1 es la latitud (en radianes)
            double lon1 = ConvertToRadians(longitude); // lon1 es la longitud (en radianes)
            double lat2InRad = ConvertToRadians(lat2); // Asegúrate que lat2 es la latitud del amigo
            double lon2InRad = ConvertToRadians(lon2); // lon2 es la longitud del amigo

            double dlat = lat2InRad - lat1;
            double dlon = lon2InRad - lon1;

            double a = Math.Sin(dlat / 2) * Math.Sin(dlat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2InRad) *
                       Math.Sin(dlon / 2) * Math.Sin(dlon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = R * c; // Distancia en kilómetros
            return distance;
        }


        private double ConvertToRadians(double angle)
        {
            return angle * Math.PI / 180.0;
        }


    }
}
