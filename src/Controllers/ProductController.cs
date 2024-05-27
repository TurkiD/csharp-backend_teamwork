using System.ComponentModel.DataAnnotations;
using api.Controllers;
using api.Middlewares;
using Dtos.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("/api/")]
public class ProductController : ControllerBase
{

    private readonly ProductService _productService;
    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    // [HttpGet("products")]
    // public async Task<IActionResult> GetAllProduct([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    // {

    //     var product = await _productService.GetAllProductService(pageNumber, pageSize);
    //     // if (product == null)
    //     // {
    //     //     throw new NotFoundException("No Product Found");
    //     // }
    //     // return ApiResponse.Success(product, "All products are returned successfully");
    //     return Ok(product);
    // }

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] QueryParameters queryParameters)
    {
        var products = await _productService.GetAllProductService(queryParameters);
        if (products.Items.Any())
        {
            return Ok(products);
        }
        else
        {
            throw new NotFoundException("No products found matching the search criteria");
        }
    }

    [HttpGet("products/{productId}")]
    public async Task<IActionResult> GetProductById(string productId)
    {
        if (!Guid.TryParse(productId, out Guid productIdGuid))
        {
            throw new BadRequestException("Invalid product ID format");
        }
        var product = await _productService.GetProductById(productIdGuid);
        if (product == null)
        {
            throw new NotFoundException("No Product Found");
        }
        else
        {
            return ApiResponse.Success(product, "single product is returned successfully");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("products")]
    public async Task<IActionResult> AddProduct([FromBody] ProductDto newProduct)
    {
        var response = await _productService.AddProductAsync(newProduct);
        return ApiResponse.Created(response, "Product is created successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("products/{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid productId, UpdateProductDto updateProduct)
    {
        var result = await _productService.UpdateProductService(productId, updateProduct);
        if (result == null)
        {
            throw new NotFoundException("product Not Found");
        }
        return ApiResponse.Updated(result, "Product is Updated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("products/{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(string productId)
    {
        if (!Guid.TryParse(productId, out Guid productIdGuid))
        {
            throw new BadRequestException("Invalid product ID format");
        }
        var result = await _productService.DeleteProductService(productIdGuid);
        if (!result)
        {
            throw new NotFoundException("No Product Found");
        }
        return ApiResponse.Deleted("product is Deleted successfully");
    }
}