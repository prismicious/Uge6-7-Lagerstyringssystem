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
                if (customer == null)
                    return NotFound($"Customer with ID {orderDTO.CustomerID} not found.");
                if (customer.Type != 0)
                    return BadRequest("Order creation is only allowed for regular customers.");

                var product = ProductService.Get(orderDTO.ProductID);
                if (product == null)
                    return NotFound($"Product with ID {orderDTO.ProductID} not found.");

                // Use EntityValidationHelper to check for nulls and return a combined error message
                var notFoundMessage = EntityValidationHelper.GenerateNotFoundMessage(product, customer, orderDTO.ProductID, orderDTO.CustomerID);
                if (!string.IsNullOrEmpty(notFoundMessage))
                    return NotFound(notFoundMessage);


                var orderList = OrderListService.Get(orderDTO.OrderListID) ?? OrderListService.Create(orderDTO.CustomerID);

                var createdOrder = OrderService.Create(orderList, product, orderDTO.Quantity);
                var addedOrder = OrderListService.AddOrder(orderList, createdOrder);
                var createdOrderDTO = new OrderDTO
                {
                    ID = createdOrder.ID,
                    Quantity = createdOrder.Quantity,
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

                OrderService.Update(order);
                return NoContent(); // Return 204 No Content if the update was successful
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
