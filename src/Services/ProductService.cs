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

    // public async Task<PaginationResult<ProductDto>> GetAllProductService(int pageNumber, int pageSize)
    // {
    //     var totalCount = _appDbContext.Products.Count();
    //     var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
    //     var page = await _appDbContext.Products
    //         .OrderByDescending(b => b.CreatedAt)
    //         .ThenByDescending(b => b.ProductID)
    //         .Skip((pageNumber - 1) * pageSize)
    //         // .Include(p => p.Category)
    //         .Select(p => _mapper.Map<ProductDto>(p))
    //         .Take(pageSize)
    //         .ToListAsync();

    //     // return page;

    //     return new PaginationResult<ProductDto>
    //     {
    //         Items = page,
    //         TotalCount = totalCount,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //     };
    // }
    public async Task<PaginationResult<ProductDto>> GetAllProductService(QueryParameters queryParams)
    {
        var query = _appDbContext.Products.AsQueryable();
        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            query = query.Where(p => p.ProductName.Contains(queryParams.SearchTerm) || p.Description.Contains(queryParams.SearchTerm));
        }

        if (queryParams.SelectedCategories != null && queryParams.SelectedCategories.Any())
        {
            query = query.Where(p => queryParams.SelectedCategories.Contains(p.CategoryId));
        }

        if (queryParams.MinPrice > 0)
        {
            query = query.Where(p => p.Price >= queryParams.MinPrice);
        }

        if (queryParams.MaxPrice > 0)
        {
            query = query.Where(p => p.Price <= queryParams.MaxPrice);
        }

        switch (queryParams.SortBy.ToLower())
        {
            case "name":
                query = queryParams.IsAscending ? query.OrderBy(p => p.ProductName) : query.OrderByDescending(p => p.ProductName);
                break;
            case "price":
                query = queryParams.IsAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                break;
            case "date":
                query = queryParams.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                break;
            default:
                query = query.OrderBy(p => p.CreatedAt);
                break;
        }

        var totalPages = (int)Math.Ceiling((decimal)totalCount / queryParams.PageSize);
        var products = await query
            .Include(p => p.Category)
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .Select(p => _mapper.Map<ProductDto>(p))
            .ToListAsync();

        return new PaginationResult<ProductDto>
        {
            Items = products,
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
        };
    }
    public async Task<Product?> GetProductById(Guid productId)
    {
        return await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
    }

    public async Task<ProductDto> AddProductAsync(ProductDto newProduct)
    {
        var product = new Product
        {
            ProductName = newProduct.ProductName,
            Description = newProduct.Description,
            Image = newProduct.Image,
            Quantity = newProduct.Quantity,
            Price = newProduct.Price,
            CategoryId = newProduct.CategoryID,
        };

        await _appDbContext.Products.AddAsync(product);
        await _appDbContext.SaveChangesAsync();

        return new ProductDto
        {
            ProductID = product.ProductID,
            ProductName = product.ProductName,
            Description = product.Description,
            Image = product.Image,
            Quantity = product.Quantity,
            Price = product.Price,
            CategoryID = product.CategoryId
        };
    }

    public async Task<Product?> UpdateProductService(Guid productId, UpdateProductDto updateProduct)
    {
        var existingProduct = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (existingProduct != null)
        {
            existingProduct.ProductName = updateProduct.ProductName;
            existingProduct.Description = updateProduct.Description;
            await _appDbContext.SaveChangesAsync();
            return existingProduct;
        }

        return existingProduct;
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

}