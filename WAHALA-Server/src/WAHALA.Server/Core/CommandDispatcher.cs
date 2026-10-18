using System;
using System.Collections.Generic;
using System.Text.Json;
using WAHALA.Server.Models;

namespace WAHALA.Server.Core
{
    public class CommandDispatcher
    {
        public static ClientNode ParseHandshake(string clientId, string ip, string handshakeJson)
        {
            var node = new ClientNode
            {
                ClientId = clientId,
                IPAddress = ip,
                LastSeen = "just now",
                IsOnline = true
            };

            try
            {
                using var doc = JsonDocument.Parse(handshakeJson);
                var root = doc.RootElement;

                string platform = root.TryGetProperty("platform", out var p) ? p.GetString() : "unknown";
                node.Platform = platform.ToLower() switch
                {
                    "windows" => PlatformType.Windows,
                    "android" => PlatformType.Android,
                    _ => PlatformType.Unknown
                };

                if (root.TryGetProperty("os", out var os))
                    node.OSVersion = os.GetString() ?? "";
                if (root.TryGetProperty("admin", out var admin))
                    node.IsAdminOrRooted = admin.GetBoolean();
                if (root.TryGetProperty("rooted", out var rooted))
                    node.IsAdminOrRooted = rooted.GetBoolean();
                if (root.TryGetProperty("country", out var c))
                    node.Country = c.GetString() ?? "";
            }
            catch
            {
                node.Platform = PlatformType.Unknown;
            }

            return node;
        }

        public static List<string> GetModulesForPlatform(PlatformType platform)
        {
            if (platform == PlatformType.Windows)
                return new List<string>(ModuleRegistry.WindowsModules.Keys);
            if (platform == PlatformType.Android)
                return new List<string>(ModuleRegistry.AndroidModules.Keys);
            return new List<string>();
        }

        public static string BuildCommand(string module, string action, string payload)
        {
            var envelope = new CommandEnvelope
            {
                Module = module,
                Action = action,
                Payload = payload
            };
            return JsonSerializer.Serialize(envelope);
        }
    }
}