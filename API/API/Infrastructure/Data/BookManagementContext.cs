using API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Data
{
    public class BookManagementContext : DbContext
    {
        public BookManagementContext(DbContextOptions<BookManagementContext> options)
            : base(options)
        {
        }

        private string _connectionStringRemote = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BookManagementV0;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public DbSet<Administrator> administrators { get; set; }
        public DbSet<Book> books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(new Administrator
            {
                Id = 1,
                Email = "adm@test.com",
                Password = "123456",
                Profile = "Adm"
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(_connectionStringRemote)
                    .UseLazyLoadingProxies();
            }
        }
    }
}
