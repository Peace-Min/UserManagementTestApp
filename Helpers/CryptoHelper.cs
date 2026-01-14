using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UserManagementTestApp.Helpers
{
    public static class CryptoHelper
    {
        // 32byte (256bit) Key
        private static readonly string KeyString = "UserManagementApp_SecretKey_2026"; 
        // 16byte (128bit) IV
        private static readonly string IvString = "UserMgmtApp_IV12"; 

        private static readonly byte[] Key;
        private static readonly byte[] IV;

        static CryptoHelper()
        {
            using (var sha256 = SHA256.Create())
            {
                Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(KeyString));
            }
            IV = Encoding.UTF8.GetBytes(IvString);
            
            if (IV.Length != 16)
            {
                var tmp = new byte[16];
                Array.Copy(IV, tmp, Math.Min(IV.Length, 16));
                IV = tmp;
            }
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Key;
                    aes.IV = IV;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                // 복호화 실패 시 원본 반환 (하위 호환성)
                return cipherText;
            }
        }
    }
}
