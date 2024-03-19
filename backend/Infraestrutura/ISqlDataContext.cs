using Microsoft.EntityFrameworkCore;
using WebApi.CalculoCdb;

namespace WebApi.Infraestrutura;

public interface ISqlDataContext
{
    public DbSet<Cdb> Cdb { get; set; }
}
