using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SecurePlayerPrefs
{
    // ✅ Key and IV must be exactly 16 bytes for AES-128
    private static readonly byte[] key = Encoding.UTF8.GetBytes("ThisIs16ByteKey!");
    private static readonly byte[] iv  = Encoding.UTF8.GetBytes("ThisIs16ByteIV!!");

    public static void SaveEncryptedString(string keyName, string plainText)
    {
        string encrypted = EncryptString(plainText);
        PlayerPrefs.SetString(keyName, encrypted);
        PlayerPrefs.Save();
    }

    public static string GetEncryptedString(string keyName)
    {
        string encrypted = PlayerPrefs.GetString(keyName, null);
        if (string.IsNullOrEmpty(encrypted))
            return null;

        return DecryptString(encrypted);
    }

    private static string EncryptString(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
        public static void DeleteEncryptedKey(string keyName)
    {
        if (PlayerPrefs.HasKey(keyName))
        {
            PlayerPrefs.DeleteKey(keyName);
            PlayerPrefs.Save();
        }
    }

    private static string DecryptString(string encryptedText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
