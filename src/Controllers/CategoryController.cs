using System.ComponentModel.DataAnnotations;
using api.Controllers;
using api.Middlewares;
using Dtos.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategory()
    {
        var categories = await _categoryService.GetAllCategoryService();
        if (categories.ToList().Count < 1)
        {
           throw new NotFoundException("No Categories Found");
        }
        return ApiResponse.Success(categories, "all categories are returned successfully");
    }


    [HttpGet("categories/{categoryId:guid}")]
    public async Task<IActionResult> GetCategory(Guid categoryId)
    {
        var category = await _categoryService.GetCategoryById(categoryId);
        if (category == null)
        {
           throw new NotFoundException("Category Not Found");
        }
        else
        {
            return ApiResponse.Success(category, "Category Found");
        }
    }


    // [Authorize(Roles = "Admin")]
    [HttpPost("account/dashboard/categories/new-category")]
    public async Task<IActionResult> CreateCategory(CategoryDto newCategory)
    {
        var result = await _categoryService.CreateCategoryService(newCategory);
        if (!result)
        {
            throw new ValidationException("Invalid Category Data");
        }
        return ApiResponse.Created("Category is created successfully");
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("account/dashboard/categories/{categoryId:guid}/update")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, CategoryDto updateCategory)
    {
       var result = await _categoryService.UpdateCategoryService(categoryId, updateCategory);
        if (!result)
        {
            throw new NotFoundException("Category Not Found");
        }
        return ApiResponse.Updated("Category is updated successfully");
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("account/dashboard/categories/{categoryId:guid}/delete")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        var result = await _categoryService.DeleteCategoryService(categoryId);
        if (!result)
        {
            throw new NotFoundException("Category Not Found");
        }
        return ApiResponse.Deleted("Category is deleted successfully");

    }
}
