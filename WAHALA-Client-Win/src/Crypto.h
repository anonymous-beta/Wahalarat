#pragma once
#include <string>
#include <vector>

class Crypto {
public:
    static std::vector<unsigned char> Encrypt(const std::string& plaintext);
    static std::string Decrypt(const std::vector<unsigned char>& packet);
    static void Init(const std::string& base64Key);
};
