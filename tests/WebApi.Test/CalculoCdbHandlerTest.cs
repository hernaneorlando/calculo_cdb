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
    /// Dado um valor monetário inicial inferior a 1(um), como por exemplo - R$ 100,00
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
            ValorInicial = -100M,
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
            ValorInicial = 100M,
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

    /// <summary>
    /// Dado um valor monetário inicial positivo
    /// E um prazo em meses maior que 1(um)
    /// Quando o sitema calcular o CDB
    /// Então uma resposta do tipo <see cref="CalculoCdbDto"/> será retornada.
    /// </summary>
    /// <returns><see cref="CalculoCdbDto"/></returns>
    [Theory]
    [ClassData(typeof(ObtemAliquotaPorFaixaMensal))]
    public async Task RetornaComSucessoOResultadoDoCalculo(
        decimal valorInicial, 
        int meses, 
        decimal aliquota,
        decimal valorBruto,
        decimal valorLiquido)
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
        Math.Round(calculoDto.ValorBruto, 2).Should().Be(valorBruto);
        Math.Round(calculoDto.ValorLiquido, 2).Should().Be(valorLiquido);

        var diferencaPercentual = 1 - (calculoDto.ValorLiquido / calculoDto.ValorBruto);
        diferencaPercentual.Should().Be(aliquota);
    }

    private void DataContextMockSetup()
    {
        var cdb = new Cdb { Cdi = 0.009M, TaxaBancaria = 1.08M };

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
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]       // Alíquota do imposto de 22,5% para um prazo de 02 meses.
            {
                1000.50M,
                2,
                0.225M,
                1020.04M,
                790.53M
            };
            yield return new object[]       // Alíquota do imposto de 22,5% para um prazo de 06 meses.
            {
                1000.50M,
                6,
                0.225M,
                1060.29M,
                821.72
            };
            yield return new object[]       // Alíquota do imposto de 20% para um prazo de 07 meses.
            {
                1000.50M,
                7,
                0.2M,
                1070.59M,
                856.47M
            };
            yield return new object[]       // Alíquota do imposto de 20% para um prazo de 12 meses.
            {
                1000.50M,
                12,
                0.2M,
                1123.64M,
                898.91M
            };
            yield return new object[]       // Alíquota do imposto de 17,5% para um prazo 13 meses.
            {
                1000.50M,
                13,
                0.175M,
                1134.57M,
                936.02M
            };
            yield return new object[]       // Alíquota do imposto de 17,5% para um prazo 24 meses.
            {
                1000.50M,
                24,
                0.175M,
                1261.94M,
                1041.10M
            };
            yield return new object[]       // Alíquota do imposto de 15% para um prazo acima de 24 meses.
            {
                1000.50M,
                360,
                0.150M,
                32550.84M,
                27668.21M
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}