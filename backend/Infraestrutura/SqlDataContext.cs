using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApi.CalculoCdb;

namespace WebApi.Infraestrutura;

public class SqlDataContext(DbContextOptions<SqlDataContext> options) : DbContext(options)
{
    public DbSet<Cdb> Cdb { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
