namespace RabbitMQEmail.DTO;

public class EmailDto
{
    public string To { get; set; }
    public string CC { get; set; } = string.Empty;
    public string Subject { get; set; }
    public string Body { get; set; }

}
