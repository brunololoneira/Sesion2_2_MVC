using Amigos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Amigos.Controllers
{
    public class PruebaController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Adios(string valor, int veces)
        {
            ViewBag.valor = valor;
            ViewBag.veces = veces;

            return View();
        }


        /*
        private readonly IInc _inc;

        public PruebaController(IInc inc)
        {
            _inc = inc;
        }

        public string Contador()
        {
            return $"Valor del contador: {_inc.Inc()}";
        }

        public string Index()
        {
            return "Bienvenido a mi primer controlador en ASP.NET Core MVC";
        }

        public string Hola(string valor, int veces)
        {
            return string.Concat(Enumerable.Repeat(valor + " ", veces));
        }
        */
    }
}
