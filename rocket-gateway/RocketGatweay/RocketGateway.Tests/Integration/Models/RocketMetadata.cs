namespace RocketGateway.Tests.Integration.Models;

public class RocketMetadata
{
    public string Channel { get; set; }

    public int MessageNumber { get; set; }

    public DateTimeOffset MessageTime { get; set; }

    public string MessageType { get; set; }

    public static RocketMetadata MakeWithType(string messageType, int number)
        => new RocketMetadata
        {
            Channel = "1",
            MessageNumber = number,
            MessageTime = DateTime.Now,
            MessageType = messageType
        };
}