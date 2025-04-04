using Microsoft.AspNetCore.Mvc;
using StorageSystem.Models;
using StorageSystem.Services;

namespace API.Controllers
{
    /// <summary>
    /// Controller for managing products in the storage system.
    /// Provides endpoints to create, read, update, and delete products.
    /// </summary>
    // This attribute indicates that this class is a controller for an API.
    // It also means that the controller gets picked up by the ASP.NET Core routing system.
    // This is done in the Program.cs file with app.AddControllers() and app.MapControllers().
    
    [ApiController]
    [Route("api/product")] // I think this is a bit easier to understand.
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            // Handles GET requests to api/product
            try
            {
                var products = ProductService.Get();
                // Returns 200 OK
                return Ok(products);
            }

            catch (Exception e)
            {
                Console.WriteLine($"Error in GetAll: {e.Message}");
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet("{id}")] // Handles GET requests to api/product/{id}
        // The {id} part is a route parameter. It means that the id will be passed in the URL.
        public IActionResult GetById(int id)
        {
            // Handles GET requests to api/product/{id}
            try
            {
                var product = ProductService.Get(id);
                if (product == null)
                    return NotFound();
                // Returns 200 OK
                return Ok(product);
            }

            catch (Exception e)
            {
                Console.WriteLine($"Error in GetById: {e.Message}");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            // Handles POST requests to api/product
            // The [FromBody] attribute indicates that the product object will be deserialized from the request body.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request with validation errors
            }

            try
            {
                var createdProduct = ProductService.Create(product.Price, product.Name, product.Type);
                // Returns 201 Created with the location of the new resource in the header
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.ID }, createdProduct);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in Create: {e.Message}");
                return StatusCode(500, e.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateOrCreate([FromBody] Product product)
        {
            // Handles PUT requests to api/product/{id}
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request with validation errors
            }

            try
            {
                if (!ProductService.Update(product))
                    return Create(product);
                else
                    return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in Update: {e.Message}");
                return StatusCode(500, e.Message);
            }
        }


        [HttpPatch]
        public IActionResult Update([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request with validation errors
            }

            try
            {
                if (!ProductService.Update(product))
                    return NotFound();
                else
                    return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in Update: {e.Message}");
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Handles DELETE requests to api/product/{id}
            try
            {
                if (!ProductService.Remove(id))
                    return NotFound();
                else
                    return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in Delete: {e.Message}");
                return StatusCode(500, e.Message);
            }
        }
    }
}
