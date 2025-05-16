using FirstMicroserviceRabbitMQ.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstMicroserviceRabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessagePublisher _publisher;

        public MessageController(MessagePublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] string message)
        {
            _publisher.Publish(message);
            return Ok("Message sent to RabbitMQ");
        }
    }
}
