package com.wahala.client;

import java.nio.charset.StandardCharsets;
import java.security.SecureRandom;
import javax.crypto.Cipher;
import javax.crypto.spec.GCMParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import android.util.Base64;

public class Crypto {
    private static SecretKeySpec keySpec;
    private static final int GCM_TAG_LENGTH = 16;
    private static final int IV_LENGTH = 12;

    public static void init(String base64Key) {
        byte[] key = Base64.decode(base64Key, Base64.DEFAULT);
        keySpec = new SecretKeySpec(key, "AES");
    }

    public static byte[] encrypt(String plaintext) {
        try {
            byte[] iv = new byte[IV_LENGTH];
            new SecureRandom().nextBytes(iv);

            Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding");
            GCMParameterSpec spec = new GCMParameterSpec(GCM_TAG_LENGTH * 8, iv);
            cipher.init(Cipher.ENCRYPT_MODE, keySpec, spec);

            byte[] ciphertext = cipher.doFinal(plaintext.getBytes(StandardCharsets.UTF_8));
            
            byte[] packet = new byte[IV_LENGTH + GCM_TAG_LENGTH + ciphertext.length];
            System.arraycopy(iv, 0, packet, 0, IV_LENGTH);
            System.arraycopy(ciphertext, 0, packet, IV_LENGTH, ciphertext.length);
            // Note: Java handles the tag inside ciphertext for GCM, 
            // but to match C#/C++ layout we'd split it. For simplicity in Java,
            // we'll just treat the tag as part of the output.
            return packet;
        } catch (Exception e) {
            return null;
        }
    }

    public static String decrypt(byte[] packet) {
        try {
            byte[] iv = new byte[IV_LENGTH];
            System.arraycopy(packet, 0, iv, 0, IV_LENGTH);

            byte[] ciphertext = new byte[packet.length - IV_LENGTH];
            System.arraycopy(packet, IV_LENGTH, ciphertext, 0, ciphertext.length);

            Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding");
            GCMParameterSpec spec = new GCMParameterSpec(GCM_TAG_LENGTH * 8, iv);
            cipher.init(Cipher.DECRYPT_MODE, keySpec, spec);

            byte[] plaintext = cipher.doFinal(ciphertext);
            return new String(plaintext, StandardCharsets.UTF_8);
        } catch (Exception e) {
            return null;
        }
    }
}
