using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products.Include(p => p.Categories).ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                                 .Include(p => p.Categories)
                                 .FirstOrDefaultAsync(p => p.ProductID == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                                 .Include(p => p.Categories)
                                 .Where(p => p.Categories.Any(c => c.CategoryID == categoryId))
                                 .ToListAsync();
        }

        public async Task AdjustPriceAsync(int productId, decimal adjustmentAmount, bool isPercentage)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product not found");

            decimal adjustment = isPercentage ? product.Price * adjustmentAmount / 100 : adjustmentAmount;
            product.Price += adjustment;

            if (product.Price < 0)
                throw new InvalidOperationException("Price cannot be negative");

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == product.SKU))
                throw new InvalidOperationException("SKU must be unique");

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == product.SKU && p.ProductID != product.ProductID))
                throw new InvalidOperationException("SKU must be unique");

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        //Task<IEnumerable<Product>> IProductService.GetAllProductsAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //Task<Product> IProductService.GetProductByIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task AddProductAsync(Product product)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task UpdateProductAsync(Product product) => throw new NotImplementedException();

        //public Task DeleteProductAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
