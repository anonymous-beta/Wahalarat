#include "Network.h"
#include "Crypto.h"

Network::Network() : _socket(INVALID_SOCKET) {
    WSADATA wsaData;
    WSAStartup(MAKEWORD(2, 2), &wsaData);
}

Network::~Network() {
    Close();
    WSACleanup();
}

bool Network::Connect(const std::string& host, int port) {
    struct addrinfo hints = {}, *result = nullptr;
    hints.ai_family = AF_INET;
    hints.ai_socktype = SOCK_STREAM;
    if (getaddrinfo(host.c_str(), std::to_string(port).c_str(), &hints, &result) != 0) return false;

    _socket = socket(result->ai_family, result->ai_socktype, result->ai_protocol);
    if (_socket == INVALID_SOCKET) { freeaddrinfo(result); return false; }

    if (connect(_socket, result->ai_addr, (int)result->ai_addrlen) == SOCKET_ERROR) {
        freeaddrinfo(result);
        closesocket(_socket);
        return false;
    }
    freeaddrinfo(result);
    return true;
}

bool Network::Send(const std::vector<unsigned char>& data) {
    if (_socket == INVALID_SOCKET) return false;
    uint32_t len = htonl((uint32_t)data.size());
    if (send(_socket, (char*)&len, 4, 0) == SOCKET_ERROR) return false;
    return send(_socket, (char*)data.data(), (int)data.size(), 0) != SOCKET_ERROR;
}

std::vector<unsigned char> Network::Receive() {
    std::vector<unsigned char> packet;
    if (_socket == INVALID_SOCKET) return packet;

    uint32_t len = 0;
    if (recv(_socket, (char*)&len, 4, 0) <= 0) return packet;
    len = ntohl(len);

    packet.resize(len);
    int total = 0;
    while (total < (int)len) {
        int r = recv(_socket, (char*)packet.data() + total, len - total, 0);
        if (r <= 0) break;
        total += r;
    }
    return packet;
}

void Network::Close() {
    if (_socket != INVALID_SOCKET) {
        closesocket(_socket);
        _socket = INVALID_SOCKET;
    }
}
