using WebApi.Infraestrutura;

namespace WebApi.CalculoCdb;

public class Cdb : BaseEntity
{
    public decimal Cdi { get; set; }
    public decimal TaxaBancaria {  get; set; }
}
