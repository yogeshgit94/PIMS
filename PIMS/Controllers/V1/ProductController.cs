using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.Security.Claims;
using ServiceContracts.DTO;

namespace PIMS.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        #region GetAllProducts
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            //var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            //if (userRole != "Administrator" || userRole!="User")
            //{
            //    return Unauthorized("You are not authorized to access the product list.");
            //}

            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        #endregion

        #region GetProductById
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }
        #endregion

        #region AddProduct
        //[Authorize(Roles = "Administrator")]
        [HttpPost("add")]
        public async Task<IActionResult> AddProduct([FromBody] ProductInputDTO productInputDTO)
        {
            try
            {
                await _productService.AddProductAsync(productInputDTO);
                return Ok("Product added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            //var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            //if (userRole != "Administrator")
            //{
            //    return Unauthorized("You are not authorized to add products.");
            //}

            //await _productService.AddProductAsync(product);
            //return Ok("Product added successfully");

            //if (product == null)
            //{
            //    return BadRequest("Product data is required.");
            //}

            //if (product.ProductCategories == null || !product.ProductCategories.Any())
            //{
            //    return BadRequest("At least one category must be associated with the product.");
            //}

            //try
            //{
            //    await _productService.AddProductAsync(product);
            //    return Ok("Product added successfully.");
            //}
            //catch (InvalidOperationException ex)
            //{
            //    return BadRequest(ex.Message);
            //}
        }
        #endregion

        #region AdjustPrice
        [HttpPost("{productId}/adjust-price")]
        public async Task<IActionResult> AdjustPrice(int productId, [FromQuery] decimal adjustmentAmount, [FromQuery] bool isPercentage)
        {
            try
            {
                await _productService.AdjustPriceAsync(productId, adjustmentAmount, isPercentage);
                return Ok("Price adjusted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Product not found.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region UpdateProduct
        [HttpPut("{ProductID}")]
        public async Task<IActionResult> UpdateProduct(int ProductID, [FromBody] ProductUpdateDTO updatedProduct)
        {
            var result = await _productService.UpdateProductAsync(ProductID, updatedProduct);

            if (result == "Product not found.")
            {
                return NotFound(new { message = result });
            }

            if (result == "SKU must be unique. A product with this SKU already exists.")
            {
                return BadRequest(new { message = result });
            }

            return Ok(new { message = result });
        }
        #endregion

        #region GetProductsByCategoryId
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategoryId(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);

            if (products == null || !products.Any())
            {
                return NotFound(new { message = "No products found for the specified category." });
            }

            return Ok(products);
        }
        #endregion
    }
}
