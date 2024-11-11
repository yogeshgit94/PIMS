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
        /// <summary>
        /// Get all products.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     /api/v{version}/Product
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     [
        ///         {
        ///             "ProductID": 1,
        ///             "Name": "Motorola",
        ///             "Description": "Motog4",
        ///             "Price": 20.50,
        ///             "SKU": "M001",
        ///             "CreatedDate": "2024-11-11T00:00:00",
        ///             "Categories": [
        ///                 {
        ///                     "CategoryID": 1,
        ///                     "CategoryName": "Electronic"
        ///                 },
        ///                 {
        ///                     "CategoryID": 4,
        ///                     "CategoryName": "Cell Phones"
        ///                 }
        ///             ]
        ///         },
        ///         ...
        ///     ]
        /// 
        ///     401 Unauthorized
        ///     "Unauthorized access"
        /// 
        ///     500 Internal Server Error
        ///     "Error message"
        /// </remarks>
        /// <response code="200">If the products are successfully retrieved.</response>
        /// <response code="401">If the user is not authorized.</response>
        /// <response code="500">If an internal server error occurs.</response>

        [HttpGet]
        [Authorize(Policy = "UserOrAdmin")]        
        public async Task<IActionResult> GetAllProducts()
        {            
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        #endregion

        #region GetProductById     

        /// <summary>
        /// Get a product by its ID.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     /api/v{version}/Product/{productId}
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///      {
        ///             "ProductID": 1,
        ///             "Name": "Motorola",
        ///             "Description": "Motog4",
        ///             "Price": 20.50,
        ///             "SKU": "M001",
        ///             "CreatedDate": "2024-11-11T00:00:00",
        ///             "Categories": [
        ///                 {
        ///                     "CategoryID": 1,
        ///                     "CategoryName": "Electronic"
        ///                 },
        ///                 {
        ///                     "CategoryID": 4,
        ///                     "CategoryName": "Cell Phones"
        ///                 }
        ///             ]
        ///         }
        /// 
        ///     404 Not Found
        ///     "Product not found"
        /// 
        ///     500 Internal Server Error
        ///     "Error message"
        /// </remarks>
        /// <param name="productId">The ID of the product to be retrieved.</param>
        /// <response code="200">If the product is successfully retrieved.</response>
        /// <response code="404">If the product with the specified ID is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>



        [HttpGet("{productId}")]
        [Authorize(Policy = "UserOrAdmin")]        
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null) return NotFound("Product not found");
            return Ok(product);
        }
        #endregion

        #region AddProduct     
        /// <summary>
        /// Add a new product.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     /api/v{version}/Product/add
        ///     
        /// 
        /// {
      ///"name": "Motorola",
      ///"description": "Motog4",
      ///"price": 50000,
      ///"sku": "M001",
      ///"createdDate": "2024-11-11T19:36:49.398Z",
      ///"productCategories": [1,4]
    ///}    
    /// 
    /// Sample Response:
    /// 
    ///     200 OK
    ///     "Product added successfully."
    /// 
    ///     400 Bad Request
    ///     "Invalid data: SKU must be unique"
    /// 
    ///     500 Internal Server Error
    ///     "Error message"
    /// </remarks>
    /// <param name="productInputDTO">The product object containing the details of the product to be added.</param>
    /// <response code="200">If the product is successfully added.</response>
    /// <response code="400">If the SKU already exists or the provided data is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>

    [HttpPost("add")]
        [Authorize(Policy = "AdminOnly")]
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
        }
        #endregion


        #region AdjustPrice     

        /// <summary>
        /// Adjust the price of a product.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     /api/v{version}/Product/{productId}/adjust-price
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     "Price adjusted successfully."
        /// 
        ///     404 Not Found
        ///     "Product not found."
        /// 
        ///     400 Bad Request
        ///     "Invalid price adjustment"
        /// 
        ///     500 Internal Server Error
        ///     "Error message"
        /// </remarks>
        /// <param name="productId">The ID of the product to be adjusted. Ex =1</param>
        /// <param name="adjustmentAmount">The amount to adjust the price by. Ex=5</param>
        /// <param name="isPercentage">Whether the adjustment is a percentage (true) or a fixed amount (false). Example =True</param>
        /// <response code="200">If the price is successfully adjusted.</response>
        /// <response code="404">If the product with the specified ID is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("{productId}/adjust-price")]
        [Authorize(Policy = "AdminOnly")]
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
        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     /api/v{version}/Product/{productId}
        /// 
        ///     {
        ///         "Name": "SamsungS24",
        ///         "Description": "samsung24 mobile",
        ///         "Price": 2500000.00,
        ///         "SKU": "SAM002",
        ///         "CategoryIDs": [1, 4]
        ///     }
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     {
        ///         "message": "Product updated successfully."
        ///     }
        /// 
        ///     400 Bad Request
        ///     {
        ///         "message": "SKU must be unique. A product with this SKU already exists."
        ///     }
        /// 
        ///     404 Not Found
        ///     {
        ///         "message": "Product not found."
        ///     }
        /// 
        ///     500 Internal Server Error
        ///     "Error message"
        /// </remarks>
        /// <param name="ProductID">The ID of the product to be Update. Ex =1</param>        
        /// <response code="200">If the product is successfully updated.</response>
        /// <response code="400">If the SKU is not unique or invalid data is provided.</response>
        /// <response code="404">If the product with the specified ID is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>




        [HttpPut("{ProductID}")]
        [Authorize(Policy = "AdminOnly")]
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

        /// <summary>
        /// Get products by category ID.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     /api/v{version}/Product/category/{categoryId}
        /// 
        /// Sample Response:
        /// 
        ///     200 OK
        ///     [
        ///         {
        ///             "ProductID": 1,
        ///             "Name": "Motorola",
        ///             "Description": "Motorola Phones",
        ///             "Price": 25000,
        ///             "SKU": "M001",
        ///             "CreatedDate": "2024-11-11T00:00:00",
        ///             "Categories": [
        ///                 {
        ///                     "CategoryID": 1,
        ///                     "CategoryName": "Electronic"
        ///                 }
        ///             ]
        ///         },
        ///         ...
        ///     ]
        /// 
        ///     404 Not Found
        ///     "No products found for the specified category."
        /// 
        ///     500 Internal Server Error
        ///     "Error message"
        /// </remarks>
        /// <param name="categoryId">The ID of the category to filter products by.</param>
        /// <response code="200">If the products in the specified category are successfully retrieved.</response>
        /// <response code="404">If no products are found for the given category.</response>
        /// <response code="500">If an internal server error occurs.</response>



        [HttpGet("category/{categoryId}")]
        [Authorize(Policy = "UserOrAdmin")]        
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
