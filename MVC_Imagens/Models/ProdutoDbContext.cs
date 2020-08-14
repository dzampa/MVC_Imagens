using System.Data.Entity;

namespace MVC_Imagens.Models
{
    public class ProdutoDbContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}