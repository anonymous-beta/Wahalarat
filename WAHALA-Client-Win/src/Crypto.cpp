#include "Crypto.h"
#include <windows.h>
#include <bcrypt.h>
#include <vector>
#include <string>
#include <iostream>

#pragma comment(lib, "bcrypt.lib")

static std::vector<unsigned char> g_Key;

void Crypto::Init(const std::string& base64Key) {
    // Base64 decode the key
    DWORD size = 0;
    CryptStringToBinaryA(base64Key.c_str(), 0, CRYPT_STRING_BASE64, NULL, &size, NULL, NULL);
    g_Key.resize(size);
    CryptStringToBinaryA(base64Key.c_str(), 0, CRYPT_STRING_BASE64, g_Key.data(), &size, NULL, NULL);
}

std::vector<unsigned char> Crypto::Encrypt(const std::string& plaintext) {
    BCRYPT_ALG_HANDLE hAlg = NULL;
    BCRYPT_KEY_HANDLE hKey = NULL;
    NTSTATUS status;
    std::vector<unsigned char> output;

    status = BCryptOpenAlgorithmProvider(&hAlg, BCRYPT_AES_ALGORITHM, NULL, 0);
    if (!BCRYPT_SUCCESS(status)) return output;
    BCryptSetProperty(hAlg, BCRYPT_CHAINING_MODE, (PUCHAR)BCRYPT_CHAIN_MODE_GCM, sizeof(BCRYPT_CHAIN_MODE_GCM), 0);

    status = BCryptGenerateSymmetricKey(hAlg, &hKey, NULL, 0, g_Key.data(), (ULONG)g_Key.size(), 0);
    if (!BCRYPT_SUCCESS(status)) { BCryptCloseAlgorithmProvider(hAlg, 0); return output; }

    unsigned char nonce[12];
    BCryptGenRandom(NULL, nonce, 12, BCRYPT_USE_SYSTEM_PREFERRED_RNG);

    BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO authInfo;
    BCRYPT_INIT_AUTH_MODE_INFO(authInfo);
    unsigned char tag[16];
    authInfo.pbNonce = nonce;
    authInfo.cbNonce = 12;
    authInfo.pbTag = tag;
    authInfo.cbTag = 16;

    ULONG cbOutput = 0;
    status = BCryptEncrypt(hKey, (PUCHAR)plaintext.c_str(), (ULONG)plaintext.length(), &authInfo, NULL, 0, NULL, 0, &cbOutput, 0);

    std::vector<unsigned char> ciphertext(cbOutput);
    status = BCryptEncrypt(hKey, (PUCHAR)plaintext.c_str(), (ULONG)plaintext.length(), &authInfo, NULL, 0, ciphertext.data(), cbOutput, &cbOutput, 0);

    output.insert(output.end(), nonce, nonce + 12);
    output.insert(output.end(), tag, tag + 16);
    output.insert(output.end(), ciphertext.begin(), ciphertext.end());

    BCryptDestroyKey(hKey);
    BCryptCloseAlgorithmProvider(hAlg, 0);
    return output;
}

std::string Crypto::Decrypt(const std::vector<unsigned char>& packet) {
    if (packet.size() < 28) return "";
    
    BCRYPT_ALG_HANDLE hAlg = NULL;
    BCRYPT_KEY_HANDLE hKey = NULL;
    BCryptOpenAlgorithmProvider(&hAlg, BCRYPT_AES_ALGORITHM, NULL, 0);
    BCryptSetProperty(hAlg, BCRYPT_CHAINING_MODE, (PUCHAR)BCRYPT_CHAIN_MODE_GCM, sizeof(BCRYPT_CHAIN_MODE_GCM), 0);
    BCryptGenerateSymmetricKey(hAlg, &hKey, NULL, 0, g_Key.data(), (ULONG)g_Key.size(), 0);

    BCRYPT_AUTHENTICATED_CIPHER_MODE_INFO authInfo;
    BCRYPT_INIT_AUTH_MODE_INFO(authInfo);
    authInfo.pbNonce = (PUCHAR)packet.data();
    authInfo.cbNonce = 12;
    authInfo.pbTag = (PUCHAR)(packet.data() + 12);
    authInfo.cbTag = 16;

    ULONG cbOutput = 0;
    BCryptDecrypt(hKey, (PUCHAR)(packet.data() + 28), (ULONG)(packet.size() - 28), &authInfo, NULL, 0, NULL, 0, &cbOutput, 0);
    
    std::vector<unsigned char> plaintext(cbOutput + 1);
    BCryptDecrypt(hKey, (PUCHAR)(packet.data() + 28), (ULONG)(packet.size() - 28), &authInfo, NULL, 0, plaintext.data(), cbOutput, &cbOutput, 0);
    plaintext[cbOutput] = '\0';

    BCryptDestroyKey(hKey);
    BCryptCloseAlgorithmProvider(hAlg, 0);
    return std::string((char*)plaintext.data());
}
