public class QueryParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public string? SearchTerm { get; set; } = "";
    public string? SortBy { get; set; } = "Name";
    public List<Guid>? SelectedCategories { get; set; } = [];
    public bool IsAscending { get; set; } = false;
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}