using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ProductServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;

        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _productService = new ProductService(_context);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            await _context.Products.AddAsync(new Product { Name = "MotoRola", Price = 10, SKU = "MotoG5",Description="MotoRolaset", CreatedDate = DateTime.Now });
            await _context.Products.AddAsync(new Product { Name = "Samsung", Price = 20, SKU = "SamS24",Description="SamsungSet", CreatedDate = DateTime.Now });
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product { Name = "Samsung2", Price = 10, SKU = "Samsungs234", Description="My mobile", CreatedDate = DateTime.Now };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductByIdAsync(product.ProductID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductID, result.ProductID);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Act
            var result = await _productService.GetProductByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddProduct_WhenValidDataProvided()
        {
            // Arrange
            var productDto = new ProductInputDTO
            {
                Name = "RealME",
                Description = "realme Narzo",
                Price = 100,
                SKU = "RealMeN2",
                CreatedDate = DateTime.Now,
                ProductCategories = new List<int>()
            };

            // Act
            await _productService.AddProductAsync(productDto);
            var product = await _context.Products.FirstOrDefaultAsync(p => p.SKU == "RealMeN2");

            // Assert
            Assert.NotNull(product);
            Assert.Equal("RealME", product.Name);
        }

        [Fact]
        public async Task AddProductAsync_ShouldThrowException_WhenSKUAlreadyExists()
        {
            // Arrange
            var product = new Product
            {
                Name = "Redme",
                Price = 10,
                SKU = "redme12",
                CreatedDate = DateTime.Now,
                Description = "Redme 2333" 
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductInputDTO
            {
                Name = "lenevo",
                Description = "Description",
                Price = 100,
                SKU = "redme12",
                CreatedDate = DateTime.Now,
                ProductCategories = new List<int>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _productService.AddProductAsync(productDto));
        }


        [Fact]
        public async Task AdjustPriceAsync_ShouldIncreasePrice_WhenPercentageIncrease()
        {
            // Arrange
            var product = new Product
            {
                Name = "Relme",
                Price = 100,
                SKU = "RElmenot8",
                CreatedDate = DateTime.Now,
                Description = "RElmenot8 pro" // Add this line
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _productService.AdjustPriceAsync(product.ProductID, 10, true);
            var updatedProduct = await _context.Products.FindAsync(product.ProductID);

            // Assert
            Assert.Equal(110, updatedProduct.Price);
        }


      


        [Fact]
        public async Task UpdateProductAsync_ShouldUpdateProduct_WhenValidDataProvided()
        {
            // Arrange
            var product = new Product { Name = "k2phones", Price = 10, SKU = "k23f",Description="myDEscription", CreatedDate = DateTime.Now };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var updatedProductDto = new ProductUpdateDTO
            {
                Name = "Lavaphonws",
                Description = "Updated Description",
                Price = 15,
                SKU = "12345"
            };

            // Act
            var result = await _productService.UpdateProductAsync(product.ProductID, updatedProductDto);
            var updatedProduct = await _context.Products.FindAsync(product.ProductID);

            // Assert
            Assert.Equal("Product updated successfully.", result);
            Assert.Equal("Lavaphonws", updatedProduct.Name);
            Assert.Equal("12345", updatedProduct.SKU);
        }
    }
}
