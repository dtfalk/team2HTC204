namespace Contoso.Api.Data;

public class QueryParameters
{
    private int _pageSize = 6;

    public int StartIndex { get; set; }

    public int PageNumber { get; set; }

    public string? filterText { get; set; }


    public int PageSize
    {
        get
        {
            return _pageSize;
         }
        set
        {
            _pageSize = value;
        }
    }

}

