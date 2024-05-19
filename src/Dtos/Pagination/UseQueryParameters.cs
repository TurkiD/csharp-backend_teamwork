public class QueryParameters{
    public int pageNumber { get; set;} = 1;
    public int pageSize { get; set;} = 10;
    public string searchTerm { get; set;} = string.Empty;
    public decimal minPrice { get; set;} = 0;
    public decimal maxPrice { get; set;}
    public string sortBy { get; set;} = string.Empty;
    public bool isAscending { get; set;} = false;
}