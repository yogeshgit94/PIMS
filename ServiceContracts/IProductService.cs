using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.Models;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IProductService
    {        
        Task<IEnumerable<ProductResponseDTO>> GetAllProductsAsync();
        Task<ProductResponseDTO> GetProductByIdAsync(int productId);      
        Task AddProductAsync(ProductInputDTO productDto);        
        Task<string> UpdateProductAsync(int productId, ProductUpdateDTO updatedProduct);        
        Task AdjustPriceAsync(int productId, decimal adjustmentAmount, bool isPercentage);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    }
}
