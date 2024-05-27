using Dtos.Category;
using Dtos.Pagination;
using Microsoft.EntityFrameworkCore;

public class CategoryService
{
    private AppDBContext _appDbContext;
    public CategoryService(AppDBContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<PaginationResult<Category>> GetAllCategoryService(QueryParameters queryParams)
    {
        var query = _appDbContext.Categories.AsQueryable();
        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(queryParams.SearchTerm) || c.Description.Contains(queryParams.SearchTerm));
        }
        switch (queryParams.SortBy.ToLower())
        {
            case "name":
                query = queryParams.IsAscending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
                break;
            case "date":
                query = queryParams.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                break;
            default:
                query = query.OrderBy(p => p.CreatedAt);
                break;
        }

        var totalPages = (int)Math.Ceiling((decimal)totalCount / queryParams.PageSize);
        var categories = await query
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            // .Select(p => _mapper.Map<ProductDto>(p))
            .ToListAsync();

        return new PaginationResult<Category>
        {
            Items = categories,
            TotalCount = totalCount,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
        };
        // return await _appDbContext.Categories.Include(c => c.Products).ToListAsync();
    }

    public async Task<Category?> GetCategoryById(Guid categoryId)
    {
        return await _appDbContext.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryID == categoryId);
    }

    public async Task<Category> CreateCategoryService(CategoryDto newCategory)
    {
        Category category = new Category
        {
            Name = newCategory.Name,
            Description = newCategory.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _appDbContext.Categories.AddAsync(category);
        await _appDbContext.SaveChangesAsync();
        return category;
    }

    public async Task<bool> UpdateCategoryService(Guid categoryId, CategoryDto updateCategory)
    {
        var existingCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        if (existingCategory != null)
        {
            existingCategory.Name = updateCategory.Name;
            existingCategory.Description = updateCategory.Description;
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;

    }

    public async Task<bool> DeleteCategoryService(Guid categoryId)
    {
        var categoryToRemove = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        if (categoryToRemove != null)
        {
            _appDbContext.Categories.Remove(categoryToRemove);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}