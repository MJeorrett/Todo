using Microsoft.EntityFrameworkCore;

namespace Todo.Application.Common.AppRequests;

public record PaginatedListResponse<T>(
    List<T> Items,
    int TotalCount,
    int TotalPages,
    int PageNumber,
    int PageSize)
{
    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedListResponse<T>> CreateAsync<Tin>(IQueryable<Tin> source, PaginatedListQuery query, Func<Tin, T> mapper)
    {
        var pageNumber = query.PageNumber;
        var pageSize = query.PageSize;

        if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Must be greater than 0.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Must be greater than 0.");

        var totalCount = await source.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var flooredPageNumber = Math.Min(pageNumber, totalPages == 0 ? 1 : totalPages);

        if (totalCount == 0) return new PaginatedListResponse<T>(new List<T>(), totalCount, totalPages, pageNumber, pageSize);

        var inputItems = await source
            .Skip((flooredPageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var outputItems = inputItems
            .Select(mapper)
            .ToList();

        return new PaginatedListResponse<T>(outputItems, totalCount, totalPages, flooredPageNumber, pageSize);
    }
}
