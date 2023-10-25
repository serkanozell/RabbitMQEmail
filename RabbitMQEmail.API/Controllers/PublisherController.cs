using Microsoft.AspNetCore.Mvc;
using RabbitMQEmail.API.Services.PublishServices;
using RabbitMQEmail.DTO;

namespace RabbitMQEmail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IPublishService _publishService;

        public PublisherController(IPublishService publishService)
        {
            _publishService = publishService;
        }

        [HttpPost]
        public async Task<IActionResult> Publish(EmailDto emailDto)
        {
            await _publishService.PublishToQueue(emailDto);
            return Ok();
        }
    }
}
