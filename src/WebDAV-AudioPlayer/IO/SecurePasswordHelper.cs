using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebDav.AudioPlayer.IO
{
    // AES code found on: http://www.codeproject.com/Articles/769741/Csharp-AES-bits-Encryption-Library-with-Salt
    internal static class SecurePasswordHelper
    {
        private const int SaltLength = 16;
        private static readonly byte[] Password = {
            0x0A, 0xA9, 0x75, 0x33, 0x7A, 0xE6, 0xFA, 0xA6,
            0xB3, 0xAD, 0x43, 0x0F, 0x3B, 0xB6, 0x5E, 0x43
        };

        public static string Encrypt(string text)
        {
            var salt = CalculateNewSalt();
            var bytes = Encoding.UTF8.GetBytes(text);
            var encryptedBytes = EncryptData(bytes, Password, salt);
            var merged = MergeArrays(salt, encryptedBytes);
            return Convert.ToBase64String(merged);
        }

        public static string Decrypt(string encryptedText)
        {
            try
            {
                var bytes = Convert.FromBase64String(encryptedText);
                var salt = new byte[SaltLength];
                var encryptedBytes = new byte[bytes.Length - salt.Length];
                Array.Copy(bytes, salt, salt.Length);
                Array.Copy(bytes, salt.Length, encryptedBytes, 0, encryptedBytes.Length);
                var decryptedBytes = DecryptData(encryptedBytes, Password, salt);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch
            {
                return null;
            }
        }

        private static byte[] MergeArrays(params byte[][] arrays)
        {
            var index = 0;
            var buff = new byte[arrays.Sum(a => a.Length)];
            foreach (var a in arrays)
            {
                Array.Copy(a, 0, buff, index, a.Length);
                index += a.Length;
            }
            return buff;
        }

        private static byte[] CalculateNewSalt()
        {
            var salt = new byte[SaltLength];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetNonZeroBytes(salt);
            }
            return salt;
        }

        private static byte[] EncryptData(byte[] bytesToBeEncrypted, byte[] passwordBytes, byte[] saltBytes)
        {
            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    }
                    return ms.ToArray();
                }
            }
        }

        private static byte[] DecryptData(byte[] bytesToBeDecrypted, byte[] passwordBytes, byte[] saltBytes)
        {
            using (var ms = new MemoryStream())
            {
                using (var aes = new RijndaelManaged())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    aes.Key = key.GetBytes(aes.KeySize / 8);
                    aes.IV = key.GetBytes(aes.BlockSize / 8);

                    aes.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    }
                    return ms.ToArray();
                }
            }
        }
    }
}