using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQEmail.DTO;
using System.Text;

IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                                                         .AddEnvironmentVariables()
                                                         .Build();

var factory = new ConnectionFactory();
//factory.Uri = new Uri("amqps://slcpubxz:Vw8yWtmMeNJxGC5iwM7hWYns3wRFOKt5@shrimp.rmq.cloudamqp.com/slcpubxz");
factory.Uri = new Uri(configuration.GetSection("RabbitMQ:ConnectionUri").Value!);

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.QueueDeclare(configuration.GetSection("RabbitMQ:Email-queue").Value!, true, false, false);

channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

channel.BasicConsume(configuration.GetSection("RabbitMQ:Email-queue").Value!, false, consumer);

consumer.Received += Consumer_Received;

void Consumer_Received(object? sender, BasicDeliverEventArgs e)
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    //Console.WriteLine("Received Message : " + message);

    SendMail(message);

    channel.BasicAck(e.DeliveryTag, true);
}

Console.ReadLine();

async void SendMail(string message)
{
    var emailDto = JsonConvert.DeserializeObject<EmailDto>(message)!;

    var email = new MimeMessage();
    email.From.Add(MailboxAddress.Parse(configuration.GetSection("Email:FromMail").Value!));
    foreach (var currentEmailTo in emailDto.To)
        email.To.Add(MailboxAddress.Parse(currentEmailTo));

    foreach (var currentEmailCc in emailDto.CC)
        email.Cc.Add(MailboxAddress.Parse(currentEmailCc));

    email.Subject = emailDto.Subject;
    email.Body = new TextPart(TextFormat.Html) { Text = emailDto.Body };

    using var smtpClient = new SmtpClient();
    await smtpClient.ConnectAsync(configuration.GetSection("Email:SmtpHost").Value, 587, SecureSocketOptions.StartTls);
    await smtpClient.AuthenticateAsync(configuration.GetSection("Email:FromMail").Value, configuration.GetSection("Email:Password").Value);

    await smtpClient.SendAsync(email);
    await smtpClient.DisconnectAsync(true);
}