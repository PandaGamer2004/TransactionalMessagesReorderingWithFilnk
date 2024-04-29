using System.Text.Json;

namespace RocketGateway.Features.Rockets.Framework.ExternalModels.Inbound.Events;

public class RocketChangeEvent {
    
    public RocketEventMetadata Metadata { get; set; }
    
    //Dynamically will be deducted from the message type
    public JsonDocument Message { get; set; }
}