using System.Collections.Generic;

namespace WAHALA.Server.Core
{
    public class ModuleRegistry
    {
        public static readonly Dictionary<string, List<string>> WindowsModules = new()
        {
            { "RemoteDesktop", new List<string> { "StartHVNC", "StopHVNC", "SendInput" } },
            { "Webcam", new List<string> { "CaptureStill", "StartStream" } },
            { "Keylogger", new List<string> { "GetLogs", "ClearLogs" } },
            { "FileManager", new List<string> { "ListDir", "DownloadFile", "UploadFile", "Delete" } },
            { "Shell", new List<string> { "Execute" } }
        };

        public static readonly Dictionary<string, List<string>> AndroidModules = new()
        {
            { "Screen", new List<string> { "StartStream", "StopStream" } },
            { "Camera", new List<string> { "FrontStill", "BackStill" } },
            { "SMS", new List<string> { "Read", "Send" } },
            { "Files", new List<string> { "List", "Download", "Delete" } }
        };
    }
}
