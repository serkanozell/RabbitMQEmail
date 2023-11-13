namespace RabbitMQEmail.DTO;

public class EmailDto
{
    public List<string> To { get; set; } = new List<string>();
    public List<string> CC { get; set; } = new List<string>();
    public string Subject { get; set; }
    public string Body { get; set; }

}
