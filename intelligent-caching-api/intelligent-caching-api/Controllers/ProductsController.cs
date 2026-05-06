using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace intelligent_caching_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product ID");

            var product = await _service.GetByIdAsync(id);

            if (product == null)
                return NotFound($"Product with ID {id} not found");

            return Ok(product);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto dto)
        {
            if (dto == null)
                return BadRequest("Product data is required");

            try
            {
                var createdProduct = await _service.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdProduct.Id },
                    createdProduct
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        {
            if (id <= 0)
                return BadRequest("Invalid product ID");

            if (dto == null)
                return BadRequest("Product data is required");

            try
            {
                var updatedProduct = await _service.UpdateAsync(id, dto);
                if (updatedProduct == null)
                    return NotFound($"Product with ID {id} not found");

                return Ok(updatedProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product ID");

            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound($"Product with ID {id} not found");

            return NoContent();
        }
    }

}
