using ProductService.Domain.Entities;
using ProductService.Infrastructure.Repositories;

namespace ProductService.Application.Services;

public interface IProductService
{
    Task<ProductDto?> GetProductByIdAsync(string id);
    Task<ProductDto?> GetProductBySKUAsync(string sku);
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<List<ProductDto>> GetProductsByCategoryAsync(string category);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateProductAsync(string id, UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(string id);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto?> GetProductByIdAsync(string id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto?> GetProductBySKUAsync(string sku)
    {
        var product = await _productRepository.GetProductBySKUAsync(sku);
        return product == null ? null : MapToDto(product);
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllProductsAsync();
        return products.Select(MapToDto).ToList();
    }

    public async Task<List<ProductDto>> GetProductsByCategoryAsync(string category)
    {
        var products = await _productRepository.GetProductsByCategoryAsync(category);
        return products.Select(MapToDto).ToList();
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            Stock = createProductDto.Stock,
            SKU = createProductDto.SKU,
            Category = createProductDto.Category,
            IsActive = true
        };

        var createdProduct = await _productRepository.CreateProductAsync(product);
        return MapToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateProductAsync(string id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetProductByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with id {id} not found");

        product.Name = updateProductDto.Name ?? product.Name;
        product.Description = updateProductDto.Description ?? product.Description;
        product.Price = updateProductDto.Price ?? product.Price;
        product.Stock = updateProductDto.Stock ?? product.Stock;
        product.Category = updateProductDto.Category ?? product.Category;
        product.IsActive = updateProductDto.IsActive ?? product.IsActive;

        var updatedProduct = await _productRepository.UpdateProductAsync(id, product);
        return MapToDto(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(string id)
    {
        return await _productRepository.DeleteProductAsync(id);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            SKU = product.SKU,
            Category = product.Category,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}

public class ProductDto
{
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string SKU { get; set; } = null!;
    public string Category { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string SKU { get; set; } = null!;
    public string Category { get; set; } = null!;
}

public class UpdateProductDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public string? Category { get; set; }
    public bool? IsActive { get; set; }
}
