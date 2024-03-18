namespace WebApi.CalculoCdb;

public record CalculoCdbDto(decimal ValorBruto, decimal ValorLiquido)
{
    public decimal Desconto => ValorBruto - ValorLiquido;
}
