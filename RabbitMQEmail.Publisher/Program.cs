using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://slcpubxz:Vw8yWtmMeNJxGC5iwM7hWYns3wRFOKt5@shrimp.rmq.cloudamqp.com/slcpubxz");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();
channel.QueueDeclare("hello-queue", true, false, false);

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"Message is : {x}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

    Console.WriteLine($"Message sended : {x}");
});



Console.ReadLine();