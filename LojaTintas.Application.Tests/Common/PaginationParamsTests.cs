using LojaTintas.Application.DTOs.Common;
using Xunit;

namespace LojaTintas.Application.Tests.Common;

public class PaginationParamsTests
{
    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 101)]
    [InlineData(1, 9999)]
    public void Create_ComPageOuPageSizeInvalido_DeveLancarArgumentException(int page, int pageSize)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => PaginationParams.Create(page, pageSize));
    }

    [Theory]
    [InlineData(0, 20, "page")]
    [InlineData(1, 9999, "pageSize")]
    public void Create_ComValorInvalido_DeveCitarOParametroQueFalhouNaMensagem(int page, int pageSize, string parametro)
    {
        // Act
        var ex = Assert.Throws<ArgumentException>(() => PaginationParams.Create(page, pageSize));

        // Assert
        Assert.Contains($"'{parametro}'", ex.Message);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 100)]
    [InlineData(3, 20)]
    public void Create_ComValoresValidos_DeveCriarSemErro(int page, int pageSize)
    {
        // Act
        var pagination = PaginationParams.Create(page, pageSize);

        // Assert
        Assert.Equal(page, pagination.Page);
        Assert.Equal(pageSize, pagination.PageSize);
    }

    [Fact]
    public void Skip_ComPageEnorme_NaoDeveEstourarInt()
    {
        // Arrange & Act
        var pagination = PaginationParams.Create(int.MaxValue, 100);

        // Assert
        Assert.Equal((long)(int.MaxValue - 1) * 100, pagination.Skip);
    }
}
