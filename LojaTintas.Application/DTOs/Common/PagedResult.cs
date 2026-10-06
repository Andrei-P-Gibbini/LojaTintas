namespace LojaTintas.Application.DTOs.Common;

public class PagedResult<T>
{
    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalItems { get; init; }

    public int TotalPages { get; init; }

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public IReadOnlyList<T> Items { get; init; } = [];

    public static PagedResult<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalItems) => new()
    {
        Page = page,
        PageSize = pageSize,
        TotalItems = totalItems,
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
        Items = items
    };
}
