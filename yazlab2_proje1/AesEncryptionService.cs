using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AesEncryptionService
{
    private readonly string key = "12345678901234567890123456789012";  // 32 byte key (AES-256)
    private readonly string iv = "1234567890123456";  // 16 byte IV

    public string Encrypt(string plainText)
    {
        // AES şifreleme işlemi
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);  // 32 byte key
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);    // 16 byte IV

            // Padding'i kontrol et
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);  // Veriyi şifrele
                    }
                }

                // Şifrelenmiş veriyi Base64 string olarak döndür
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        // Base64 string'in geçerliliğini kontrol et
        if (string.IsNullOrEmpty(cipherText))
        {
            throw new ArgumentException("Cipher text cannot be null or empty");
        }

        try
        {
            // Base64 string'i byte dizisine dönüştür
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // AES şifre çözme işlemi
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);  // 32 byte key
                aesAlg.IV = Encoding.UTF8.GetBytes(iv);    // 16 byte IV

                // Padding'i kontrol et
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Çözümlenen veriyi döndür
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (FormatException)
        {
            throw new FormatException("The input is not a valid Base-64 string.");
        }
        catch (CryptographicException)
        {
            throw new CryptographicException("An error occurred during decryption.");
        }
    }
}
