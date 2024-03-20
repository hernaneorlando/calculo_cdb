using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections;
using WebApi.CalculoCdb;
using WebApi.Infraestrutura;

namespace WebApi.Test;

public class CalculoCdbHandlerTest
{
    private readonly Mock<ISqlDataContext> _dataContext = new();

    /// <summary>
    /// Dado um valor monet�rio inicial inferior a 1(um)
    /// Quando o sitema calcular o CDB
    /// Ent�o uma exce��o do tipo <see cref="ValidationErrorException"/> ser� lan�ada.
    /// </summary>
    /// <returns><see cref="ValidationErrorException"/></returns>
    [Fact]
    public async Task RetornaExcecaoDevidoAValorInicialNaoPositivo()
    {
        // Cen�rio
        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = ObtenhaValorMonetarioInicialPositivo() * -1,
            QuantidadeDeMeses = 12
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador, _dataContext.Object);

        // A��o
        var acao = handler.Invoking(h => h.Handle(requisicao, CancellationToken.None));

        // Valida��o
        await acao.Should()
            .ThrowExactlyAsync<ValidationErrorException>()
            .WithMessage($"'{nameof(CalculoCdbCommand.ValorInicial)}': O valor inicial deve ser maior que 0;");
    }

    /// <summary>
    /// Dado um prazo em meses menor ou igual a 1(um)
    /// Quando o sitema calcular o CDB
    /// Ent�o uma exce��o do tipo <see cref="ValidationErrorException"/> ser� lan�ada.
    /// </summary>
    /// <returns><see cref="ValidationErrorException"/></returns>
    [Fact]
    public async Task RetornaExcecaoDevidoAPrazoMensalMenorQueDois()
    {
        // Cen�rio
        var random = new Random();
        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = ObtenhaValorMonetarioInicialPositivo(),
            QuantidadeDeMeses = random.Next(0, 1)
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador, _dataContext.Object);

        // A��o
        var acao = handler.Invoking(h => h.Handle(requisicao, CancellationToken.None));

        // Valida��o
        await acao.Should()
           .ThrowExactlyAsync<ValidationErrorException>()
           .WithMessage($"'{nameof(CalculoCdbCommand.QuantidadeDeMeses)}': O prazo em meses deve ser maior que 1;");
    }

    private static decimal ObtenhaValorMonetarioInicialPositivo()
    {
        var random = new Random();
        return (decimal)random.NextDouble() + random.Next(1000000000);
    }

    /// <summary>
    /// Dado um valor monet�rio inicial positivo
    /// E um prazo em meses maior que 1(um)
    /// Quando o sitema calcular o CDB
    /// Ent�o uma resposta do tipo <see cref="CalculoCdbDto"/> ser� retornada.
    /// </summary>
    /// <returns><see cref="CalculoCdbDto"/></returns>
    [Theory]
    [ClassData(typeof(ObtemAliquotaPorFaixaMensal))]
    public async Task RetornaComSucessoOResultadoDoCalculo(decimal valorInicial, int meses, decimal aliquota)
    {
        // Cen�rio
        DataContextMockSetup();

        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = valorInicial,
            QuantidadeDeMeses = meses
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador, _dataContext.Object);

        // A��o
        var calculoDto = await handler.Handle(requisicao, CancellationToken.None);

        // Valida��o
        calculoDto.Should().NotBeNull();
        calculoDto.ValorBruto.Should().BeGreaterThan(0);
        calculoDto.ValorLiquido.Should().BeLessThan(calculoDto.ValorBruto);

        var diferencaPercentual = 1 - (calculoDto.ValorLiquido / calculoDto.ValorBruto);
        diferencaPercentual.Should().Be(aliquota);
    }

    private void DataContextMockSetup()
    {
        var cdb = new Cdb { Cdi = 0.09M, TaxaBancaria = 1.08M };

        var dbSetCdbMock = new Mock<DbSet<Cdb>>();
        dbSetCdbMock
            .As<IQueryable<Cdb>>()
            .Setup(m => m.Provider)
            .Returns(new[] { cdb }.AsQueryable().Provider);
        dbSetCdbMock
            .As<IQueryable<Cdb>>()
            .Setup(m => m.Expression)
            .Returns(new[] { cdb }.AsQueryable().Expression);
        dbSetCdbMock
            .As<IQueryable<Cdb>>()
            .Setup(m => m.ElementType)
            .Returns(new[] { cdb }.AsQueryable().ElementType);
        dbSetCdbMock
            .As<IQueryable<Cdb>>()
            .Setup(m => m.GetEnumerator())
            .Returns(new[] { cdb }.AsQueryable().GetEnumerator());


        _dataContext.Setup(m => m.Cdb).Returns(dbSetCdbMock.Object);
    }


    private class ObtemAliquotaPorFaixaMensal : IEnumerable<object[]>
    {
        private readonly Random random = new();

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]       // Al�quota do imposto de 22,5% para um prazo entre 02 e 06 meses.
            {
                100.01M,
                random.Next(2, 7),
                0.225M
            };
            yield return new object[]
            {
                1000000000.01M,
                random.Next(2, 7),
                0.225M
            };
            yield return new object[]       // Al�quota do imposto de 20% para um prazo de at� 12 meses.
            {
                100.01M,
                random.Next(7, 13),
                0.2M
            };
            yield return new object[]
            {
                1000000000.01M,
                random.Next(7, 13),
                0.2M
            };
            yield return new object[]       // Al�quota do imposto de 17,5% para um prazo de at� 24 meses.
            {
                100.01M,
                random.Next(13, 25),
                0.175M
            };
            yield return new object[]
            {
                1000000000.01M,
                random.Next(13, 25),
                0.175M
            };
            yield return new object[]       // Al�quota do imposto de 15% para um prazo acima de 24 meses.
            {                               // A fim de n�o estourar o valor m�ximo do tipo Decimal
                100.01M,                    // o total de meses foi limitado em 360 meses (30 anos).
                random.Next(25, 360),
                0.150M
            };
            yield return new object[]
            {
                1000000000.01M,
                random.Next(25, 360),
                0.150M
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}