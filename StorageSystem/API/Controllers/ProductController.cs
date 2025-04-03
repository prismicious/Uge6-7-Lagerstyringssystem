using Microsoft.AspNetCore.Mvc;
using StorageSystem.Models;
using StorageSystem.Services;

namespace API.Controllers
{
    /// <summary>
    /// Controller for managing products in the storage system.
    /// Provides endpoints to create, read, update, and delete products.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = ProductService.Get();
            // Returns 200 OK
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = ProductService.Get(id);
            if (product == null)
                return NotFound();
            // Returns 200 OK
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            var createdProduct = ProductService.Create(product.Price, product.Name, product.Type);
            // Returns 201 Created with the location of the new resource in the header
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ID }, createdProduct);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product)
        {
            var existingProduct = ProductService.Get(id);
            if (existingProduct == null)
                return NotFound();

            ProductService.Update(existingProduct, product.Price, product.Name, product.Type);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = ProductService.Get(id);
            if (product == null)
                return NotFound();

            ProductService.Remove(product);
            return NoContent();
        }
    }
}
