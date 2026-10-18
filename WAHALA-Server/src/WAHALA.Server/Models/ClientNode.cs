namespace WAHALA.Server.Models
{
    public enum PlatformType { Windows, Android, Unknown }

    public class ClientNode
    {
        public string ClientId { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public PlatformType Platform { get; set; } = PlatformType.Unknown;
        public string OSVersion { get; set; } = string.Empty;
        public bool IsAdminOrRooted { get; set; }
        public string LastSeen { get; set; } = string.Empty;
    }
}
