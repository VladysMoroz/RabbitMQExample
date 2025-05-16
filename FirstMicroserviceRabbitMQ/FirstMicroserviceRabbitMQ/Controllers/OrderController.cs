using FirstMicroserviceRabbitMQ.Entities;
using FirstMicroserviceRabbitMQ.Interfaces;
using FirstMicroserviceRabbitMQ.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstMicroserviceRabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMessageTopicPublisher _publisher;

        public OrderController(IMessageTopicPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderDto order)
        {
            _publisher.Publish(order, "order.created");
            return Ok("Order created.");
        }

        [HttpPut]
        public IActionResult UpdateOrder([FromBody] OrderDto order)
        {
            _publisher.Publish(order, "order.updated");
            return Ok("Order updated.");
        }

        [HttpDelete("{id}")]
        public IActionResult CancelOrder(Guid id)
        {
            _publisher.Publish(new { OrderId = id }, "order.cancelled");
            return Ok("Order cancelled.");
        }
    }
}
