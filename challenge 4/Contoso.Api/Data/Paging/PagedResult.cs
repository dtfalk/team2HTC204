namespace Contoso.Api.Data;

public class PagedResult<T>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public List<T> Items { get; set; }
    
}