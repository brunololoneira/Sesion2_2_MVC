using Amigos.Models.TuProyecto.Models;
using Microsoft.EntityFrameworkCore;

namespace Amigos.DataAccessLayer
{
    public class AmigoDBContext : DbContext
    {
        public DbSet<Amigo>? Amigos { get; set; }  // Tabla de amigos

        public AmigoDBContext(DbContextOptions<AmigoDBContext> options) : base(options)
        {

        }
    }
}
