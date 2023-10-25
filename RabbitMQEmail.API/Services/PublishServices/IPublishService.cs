using RabbitMQEmail.DTO;

namespace RabbitMQEmail.API.Services.PublishServices
{
    public interface IPublishService
    {
        Task PublishToQueue(EmailDto emailDto);
    }
}
