using AutoMapper;
using Dtos.Pagination;
using Dtos.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
public class ProductService
{
    private readonly AppDBContext _appDbContext;
    private readonly IMapper _mapper;
    public ProductService(AppDBContext appDBContext, IMapper mapper)
    {
        _appDbContext = appDBContext;
        _mapper = mapper;
    }

    public async Task<PaginationResult<ProductDto>> GetAllProductService(int pageNumber, int pageSize)
    {
        var totalCount = _appDbContext.Products.Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var page = await _appDbContext.Products
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.ProductID)
            .Skip((pageNumber - 1) * pageSize)
            // .Include(p => p.Category)
            .Select(p => _mapper.Map<ProductDto>(p))
            .Take(pageSize)
            .ToListAsync();

        // return page;

        return new PaginationResult<ProductDto>
        {
            Items = page,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<Product?> GetProductById(Guid productId)
    {
        return await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
    }

    public async Task<Guid> AddProductAsync(ProductDto newProduct)
    {
        Product product = new Product
        {

            ProductID = Guid.NewGuid(),
            ProductName = newProduct.ProductName,
            Description = newProduct.Description,
            Image = newProduct.Image,
            Quantity = newProduct.Quantity,
            Price = newProduct.Price,
            CategoryId = newProduct.CategoryID,
            CreatedAt = DateTime.UtcNow

        };
        await _appDbContext.Products.AddAsync(product);
        await _appDbContext.SaveChangesAsync();
        return product.ProductID;
    }

    public async Task<bool> UpdateProductService(Guid productId, ProductDto updateProduct)
    {
        var existingProduct = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (existingProduct != null)
        {
            existingProduct.ProductName = updateProduct.ProductName;
            existingProduct.Description = updateProduct.Description;
            existingProduct.Quantity = updateProduct.Quantity;
            existingProduct.Price = updateProduct.Price;
            existingProduct.CategoryId = updateProduct.CategoryID;
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteProductService(Guid productId)
    {
        var productToRemove = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (productToRemove != null)
        {
            _appDbContext.Products.Remove(productToRemove);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<PaginationResult<ProductDto>> SearchProductsAsync(QueryParameters queryParams)
    {
        var query = _appDbContext.Products.AsQueryable();
        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(queryParams.searchTerm))
        {
            query = query.Where(p => p.ProductName.Contains(queryParams.searchTerm) || p.Description.Contains(queryParams.searchTerm));
        }

        if (queryParams.minPrice > 0)
        {
            query = query.Where(p => p.Price >= queryParams.minPrice);
        }

        if (queryParams.maxPrice > 0)
        {
            query = query.Where(p => p.Price <= queryParams.maxPrice);
        }

        switch (queryParams.sortBy.ToLower())
        {
            case "name":
                query = queryParams.isAscending ? query.OrderBy(p => p.ProductName) : query.OrderByDescending(p => p.ProductName);
                break;
            case "price":
                query = queryParams.isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                break;
            case "date":
                query = queryParams.isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                break;
            default:
                query = query.OrderBy(p => p.CreatedAt);
                break;
        }

        var totalPages = (int)Math.Ceiling((decimal)totalCount / queryParams.pageSize);
        var products = await query
            .Skip((queryParams.pageNumber - 1) * queryParams.pageSize)
            .Take(queryParams.pageSize)
            .Select(p => _mapper.Map<ProductDto>(p))
            .ToListAsync();

        return new PaginationResult<ProductDto>
        {
            Items = products,
            TotalCount = totalCount,
            PageNumber = queryParams.pageNumber,
            PageSize = queryParams.pageSize,
        };
    }
}