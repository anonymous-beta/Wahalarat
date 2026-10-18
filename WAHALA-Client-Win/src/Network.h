#pragma once
#include <winsock2.h>
#include <ws2tcpip.h>
#include <string>
#include <vector>

#pragma comment(lib, "ws2_32.lib")

class Network {
private:
    SOCKET _socket;
public:
    Network();
    ~Network();
    bool Connect(const std::string& host, int port);
    bool Send(const std::vector<unsigned char>& data);
    std::vector<unsigned char> Receive();
    void Close();
};
