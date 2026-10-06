namespace LojaTintas.Application.DTOs.Common;

public sealed class PaginationParams
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; }
    public int PageSize { get; }

    public long Skip => (long)(Page - 1) * PageSize;

    private PaginationParams(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public static PaginationParams Create(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentException("O parâmetro 'page' deve ser um inteiro maior ou igual a 1.");

        if (pageSize < 1 || pageSize > MaxPageSize)
            throw new ArgumentException($"O parâmetro 'pageSize' deve ser um inteiro entre 1 e {MaxPageSize}.");

        return new PaginationParams(page, pageSize);
    }
}
