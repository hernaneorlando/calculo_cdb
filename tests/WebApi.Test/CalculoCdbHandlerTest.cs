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
    /// Dado um valor monetário inicial inferior a 1(um)
    /// Quando o sitema calcular o CDB
    /// Então uma exceção do tipo <see cref="ValidationErrorException"/> será lançada.
    /// </summary>
    /// <returns><see cref="ValidationErrorException"/></returns>
    [Fact]
    public async Task RetornaExcecaoDevidoAValorInicialNaoPositivo()
    {
        // Cenário
        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = ObtenhaValorMonetarioInicialPositivo() * -1,
            QuantidadeDeMeses = 12
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador, _dataContext.Object);

        // Ação
        var acao = handler.Invoking(h => h.Handle(requisicao, CancellationToken.None));

        // Validação
        await acao.Should()
            .ThrowExactlyAsync<ValidationErrorException>()
            .WithMessage($"'{nameof(CalculoCdbCommand.ValorInicial)}': O valor inicial deve ser maior que 0;");
    }

    /// <summary>
    /// Dado um prazo em meses menor ou igual a 1(um)
    /// Quando o sitema calcular o CDB
    /// Então uma exceção do tipo <see cref="ValidationErrorException"/> será lançada.
    /// </summary>
    /// <returns><see cref="ValidationErrorException"/></returns>
    [Fact]
    public async Task RetornaExcecaoDevidoAPrazoMensalMenorQueDois()
    {
        // Cenário
        var random = new Random();
        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = ObtenhaValorMonetarioInicialPositivo(),
            QuantidadeDeMeses = random.Next(0, 1)
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador, _dataContext.Object);

        // Ação
        var acao = handler.Invoking(h => h.Handle(requisicao, CancellationToken.None));

        // Validação
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
    /// Dado um valor monetário inicial positivo
    /// E um prazo em meses maior que 1(um)
    /// Quando o sitema calcular o CDB
    /// Então uma resposta do tipo <see cref="CalculoCdbDto"/> será retornada.
    /// </summary>
    /// <returns><see cref="CalculoCdbDto"/></returns>
    [Theory]
    [ClassData(typeof(ObtemAliquotaPorFaixaMensal))]
    public async Task RetornaComSucessoOResultadoDoCalculo(decimal valorInicial, int meses, decimal aliquota)
    {
        // Cenário
        DataContextMockSetup();

        var requisicao = new CalculoCdbCommand
        {
            ValorInicial = valorInicial,
            QuantidadeDeMeses = meses
        };

        var validador = new CalculoCdbCommandValidator();
        var handler = new CalculoCdbHandler(validador, _dataContext.Object);

        // Ação
        var calculoDto = await handler.Handle(requisicao, CancellationToken.None);

        // Validação
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
            yield return new object[]       // Alíquota do imposto de 22,5% para um prazo entre 02 e 06 meses.
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
            yield return new object[]       // Alíquota do imposto de 20% para um prazo de até 12 meses.
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
            yield return new object[]       // Alíquota do imposto de 17,5% para um prazo de até 24 meses.
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
            yield return new object[]       // Alíquota do imposto de 15% para um prazo acima de 24 meses.
            {                               // A fim de não estourar o valor máximo do tipo Decimal
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