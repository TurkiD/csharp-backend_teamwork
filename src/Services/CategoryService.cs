using Dtos.Category;
using Microsoft.EntityFrameworkCore;

public class CategoryService
{
    private AppDBContext _appDbContext;
    public CategoryService(AppDBContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Category>> GetAllCategoryService()
    {
        return await _appDbContext.Categories.Include(c => c.Products).ToListAsync();
    }

    public async Task<Category?> GetCategoryById(Guid categoryId)
    {
        return await _appDbContext.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryID == categoryId);
    }

    public async Task<bool> CreateCategoryService(CategoryDto newCategory)
    {
        Category category = new Category
        {
            Name = newCategory.Name,
            Description = newCategory.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _appDbContext.Categories.AddAsync(category);
        await _appDbContext.SaveChangesAsync();
        return true;
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