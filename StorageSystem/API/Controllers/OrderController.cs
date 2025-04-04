using Microsoft.AspNetCore.Mvc;
using StorageSystem.Models;
using StorageSystem.Services;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;


namespace API.Controllers
{

    [ApiController]
    [Route("api/order")]

    public class OrderController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var orders = OrderService.Get();
                // Returns 200 OK
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll(OrderController):{ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var order = ProductService.Get(id);
                if (order == null)
                    return NotFound();
                return Ok(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByID(OrderController){ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  
            try
            {
                var createdOrder = OrderService.Create(order.OrderList, order.Product, order.Quantity, order.Discount, order.Price);
                return CreatedAtAction(nameof(GetByID), new { id = createdOrder.ID }, createdOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create(OrderController): {ex.Message}");
                return StatusCode(500, ex.Message); 
            }
        }



        [HttpPatch]
        public IActionResult Update([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                if (!OrderService.Update(order))
                    return NotFound();
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update(OrderController):{ex.Message}");
                return StatusCode(500, ex.Message); 
            }
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {

            try
            {
                if (!OrderService.Remove(new Order { ID = id }))
                    return NotFound();
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete(OrderController): {ex.Message}");
                return StatusCode(500, ex.Message); 
            }
        }


    }
}
