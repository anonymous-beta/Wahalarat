using System;

namespace WAHALA.Server.Models
{
    public class CommandEnvelope
    {
        public string CommandId { get; set; } = Guid.NewGuid().ToString();
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
