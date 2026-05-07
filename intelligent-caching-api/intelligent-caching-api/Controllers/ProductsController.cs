using Application.Common.Pagination;
using Application.Constants;
using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace intelligent_caching_api.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService service,
            ILogger<ProductsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet(ApiRoutes.Products.GetAll)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            _logger.LogInformation("Fetching all products");

            var products = await _service.GetAllAsync(paginationParams);

            return Ok(products);
        }

        [HttpGet(ApiRoutes.Products.GetById, Name = "GetProductById")]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogInformation(
                "Fetching product with ID: {Id}", id);

            var product = await _service.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning(
                    "Product with ID {Id} not found", id);

                return NotFound(new
                {
                    Message = $"Product with ID {id} not found"
                });
            }

            return Ok(product);
        }

        [HttpPost(ApiRoutes.Products.Create)]
        public async Task<IActionResult> Create(
            [FromBody] ProductDto dto)
        {
            _logger.LogInformation("Creating new product");

            var createdProduct = await _service.CreateAsync(dto);

            return CreatedAtRoute(
                "GetProductById",
                new { id = createdProduct.Id },
                createdProduct);
        }

        [HttpPut(ApiRoutes.Products.Update)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] ProductDto dto)
        {
            _logger.LogInformation(
                "Updating product with ID: {Id}", id);

            var updatedProduct =
                await _service.UpdateAsync(id, dto);

            if (updatedProduct == null)
            {
                _logger.LogWarning(
                    "Product with ID {Id} not found for update",
                    id);

                return NotFound(new
                {
                    Message = $"Product with ID {id} not found"
                });
            }

            return Ok(updatedProduct);
        }

        [HttpDelete(ApiRoutes.Products.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation(
                "Deleting product with ID: {Id}", id);

            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                _logger.LogWarning(
                    "Product with ID {Id} not found for deletion",
                    id);

                return NotFound(new
                {
                    Message = $"Product with ID {id} not found"
                });
            }

            return NoContent();
        }
    }
}