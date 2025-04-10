using Microsoft.AspNetCore.Mvc;
using StorageSystem.DTOs;
using StorageSystem.Models;
using StorageSystem.Services;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using API.Helpers;

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
                LogService.LogError($"Error in GetAll(OrderController):{ex.Message}");
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
                LogService.LogError($"Error in GetByID(OrderController):{ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrderDTO orderDTO)
        {

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var customer = CustomerService.Get(orderDTO.CustomerID);
                var product = ProductService.Get(orderDTO.ProductID);

                // Use EntityValidationHelper to check for nulls and return a combined error message
                var notFoundMessage = EntityValidationHelper.GenerateNotFoundMessage(product, customer, orderDTO.ProductID, orderDTO.CustomerID);
                if (!string.IsNullOrEmpty(notFoundMessage))
                    return NotFound(notFoundMessage);

                if (customer.Type != 0)
                    return BadRequest("Order creation is only allowed for regular customers.");

                var orderList = OrderListService.Get(orderDTO.OrderListID) ?? OrderListService.Create(orderDTO.CustomerID);

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
                    CustomerID = customer.ID
                };
                return CreatedAtAction(nameof(GetByID), new { id = createdOrderDTO.ID }, createdOrderDTO);
            }
            catch (Exception ex)
            {
                LogService.LogError($"Error in Create(OrderController): {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] OrderDTO orderDTO)
        {
            try
            {
                var order = OrderService.Get(id);
                if (order == null)
                    return NotFound();

                order.Quantity = orderDTO.Quantity;
                order.Discount = orderDTO.Discount;
                order.Price = orderDTO.Price;

                var updatedOrder = OrderService.Update(order);
                if (updatedOrder)
                    return NoContent(); // Return 204 No Content if the update was successful
                else
                    return StatusCode(500, "Failed to update the order due to an internal error."); // Return 500 Internal Server Error
            }
            catch (Exception e)
            {
                LogService.LogError($"Error in Update: {e.Message}");
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var order = OrderService.Get(id);
                if (order == null)
                    return NotFound();

                if (!OrderService.Remove(order))
                    return NotFound();
                else
                    return NoContent();
            }
            catch (Exception ex)
            {
                LogService.LogError($"Error in Delete(OrderController): {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
