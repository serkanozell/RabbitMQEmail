using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQEmail.DTO;
using System.Text;

namespace RabbitMQEmail.API.Services.PublishServices
{
    public class PublishService : IPublishService
    {
        private readonly IConfiguration _configuration;

        public PublishService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishToQueue(EmailDto emailDto)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_configuration.GetSection("RabbitMQ:ConnectionUri").Value!);

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _configuration.GetSection("RabbitMQ:Email-queue").Value!, durable: true, exclusive: false, autoDelete: false);

            var json = JsonConvert.SerializeObject(emailDto);
            byte[] message = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: string.Empty, routingKey: _configuration.GetSection("RabbitMQ:Email-queue").Value!, null, message);
        }
    }
}
