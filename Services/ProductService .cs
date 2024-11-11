using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using PIMS.Exceptions;
using ServiceContracts;
using ServiceContracts.DTO;


namespace Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }


        #region GetAllProductsAsync
        public async Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
                .Select(p => new ProductResponseDTO
                {
                    ProductID = p.ProductID,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    SKU = p.SKU,
                    CreatedDate = p.CreatedDate,
                    Categories = p.ProductCategories.Select(pc => new CategoryDTO
                    {
                        CategoryID = pc.CategoryID,
                        CategoryName = pc.Category.Name
                    }).ToList()
                }).ToListAsync();
        }
        #endregion

        #region GetProductByIdAsync
        public async Task<ProductResponseDTO> GetProductByIdAsync(int productId)
        {
            var product = await _context.Products
                .Where(p => p.ProductID == productId)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return null; // Return null if product is not found
            }

            // Map to ProductDto and return
            return new ProductResponseDTO
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CreatedDate = product.CreatedDate,
                Categories = product.ProductCategories.Select(pc => new CategoryDTO
                {
                    CategoryID = pc.CategoryID,
                    CategoryName = pc.Category.Name
                }).ToList()
            };
        }
        #endregion

        #region GetProductsByCategoryAsync
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products
                                .Where(p => p.ProductCategories
                                             .Any(pc => pc.CategoryID == categoryId)) // Filter products by category ID
                                .Include(p => p.ProductCategories) // Include product categories for the result
                                .ThenInclude(pc => pc.Category)  // Optional: To include category details as well
                                .ToListAsync();
            return products;
        }
        #endregion

        #region AdjustPriceAsync

        public async Task AdjustPriceAsync(int productId, decimal adjustmentAmount, bool isPercentage)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new KeyNotFoundException ("Product not found");

            decimal adjustment = isPercentage ? product.Price * adjustmentAmount / 100 : adjustmentAmount;
            product.Price=product.Price+adjustment>0?product.Price + adjustment:0;

            if (product.Price < 0)
                throw new InvalidOperationException("Price cannot be negative");

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region AddProductAsync

        public async Task AddProductAsync(ProductInputDTO ProductDTO)
        {

            var product = new Product
            {
                Name = ProductDTO.Name,
                Description = ProductDTO.Description,
                Price = ProductDTO.Price,
                SKU = ProductDTO.SKU,
                CreatedDate = ProductDTO.CreatedDate,
                ProductCategories = new List<ProductCategory>()
            };

            if (await _context.Products.AnyAsync(p => p.SKU == product.SKU))
                throw new InvalidOperationException("SKU must be unique");

            // Step 1: Add the new product
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync(); // Save to get ProductID

            // Step 2: Add associated categories to the ProductCategories table
            foreach (var CategoryID in ProductDTO.ProductCategories)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    ProductID = product.ProductID,
                    CategoryID = CategoryID
                });               
            }

            await _context.SaveChangesAsync(); // Save ProductCategories
        }
        #endregion

        #region UpdateProductAsync
        public async Task<string> UpdateProductAsync(int productId, ProductUpdateDTO updatedProduct)
        {
            // Find the product by ID
            var product = await _context.Products
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.ProductID == productId);

            if (product == null)
            {
                return "Product not found.";
            }

            // Check if the SKU is unique
            var existingProductWithSameSku = await _context.Products
                .FirstOrDefaultAsync(p => p.SKU == updatedProduct.SKU && p.ProductID != productId);

            if (existingProductWithSameSku != null)
            {
                return "SKU must be unique. A product with this SKU already exists.";
            }

            // Update product properties
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;
            product.SKU = updatedProduct.SKU;

            // Update categories
            product.ProductCategories.Clear(); // Clear existing categories

            if (updatedProduct.CategoryIDs != null && updatedProduct.CategoryIDs.Any())
            {
                foreach (var categoryId in updatedProduct.CategoryIDs)
                {
                    var category = await _context.Categories.FindAsync(categoryId);
                    if (category != null)
                    {
                        product.ProductCategories.Add(new ProductCategory
                        {
                            ProductID = productId,
                            CategoryID = categoryId
                        });
                    }
                }
            }

            // Save changes
            await _context.SaveChangesAsync();
            return "Product updated successfully.";
        }
        #endregion             
    }
}
