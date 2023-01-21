using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users {get; set;} = null!;
        public DbSet<Item> Items {get; set;} = null!;
        public DbSet<Models.Type> Types {get; set;} = null!;
        public DbSet<Cart> Carts {get; set;} = null!;
        public DbSet<Variant> Variants {get; set;} = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Kit> Kits { get; set; } = null!;
    }
}