using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TFTEC.Web.Ecommerce.Models;

namespace TFTEC.Web.Ecommerce.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Produto> Produto { get; set; }

        public DbSet<ListaPreco> ListaPreco { get; set; }

        public DbSet<ItensListaPreco> ItensListaPreco { get; set; }

        public DbSet<Categoria> Categoria { get; set; }
    }
}
