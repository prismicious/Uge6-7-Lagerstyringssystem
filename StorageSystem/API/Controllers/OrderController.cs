using Microsoft.AspNetCore.Mvc;
using StorageSystem.DTOs;
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
                var orderDTOs = orders.Select(o => new OrderDTO
                {
                    ID = o.ID,
                    Quantity = o.Quantity,
                    Discount = o.Discount,
                    Price = o.Price,
                    ProductID = o.ProductID,
                    OrderListID = o.OrderListID
                });
                return Ok(orderDTOs);
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
                var order = OrderService.Get(id);
                if (order == null)
                    return NotFound();

                var orderDTO = new OrderDTO
                {
                    ID = order.ID,
                    Quantity = order.Quantity,
                    Discount = order.Discount,
                    Price = order.Price,
                    ProductID = order.ProductID,
                    OrderListID = order.OrderListID
                };
                return Ok(orderDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByID(OrderController):{ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var orderList = OrderListService.Get(orderDTO.OrderListID);

                if (orderList == null)
                    orderList = OrderListService.Create(orderDTO.Customer);


                var product = ProductService.Get(orderDTO.ProductID);

                if (product == null)
                    // Product not found
                    throw new Exception($"Product with ID {orderDTO.ProductID} not found.");

                var createdOrder = OrderService.Create(orderList, product, orderDTO.Quantity, orderDTO.Discount, orderDTO.Price);
                var addedOrder = OrderListService.AddOrder(orderList, createdOrder);
                var createdOrderDTO = new OrderDTO
                {
                    ID = createdOrder.ID,
                    Quantity = createdOrder.Quantity,
                    Discount = createdOrder.Discount,
                    Price = createdOrder.Price,
                    ProductID = createdOrder.ProductID,
                    OrderListID = createdOrder.OrderListID,
                    Customer = orderDTO.Customer
                };
                return CreatedAtAction(nameof(GetByID), new { id = createdOrderDTO.ID }, createdOrderDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create(OrderController): {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch]
        public IActionResult Update([FromBody] OrderDTO orderDTO)
        {
            
            var order = OrderService.Get(orderDTO.ID);
            if (order == null)
                return NotFound();

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
        public IActionResult Delete(OrderDTO orderDTO)
        {
            var order = OrderService.Get(orderDTO.ID);
            try
            {
                if (!OrderService.Remove(order))
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
