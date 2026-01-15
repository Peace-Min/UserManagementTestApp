using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UserManagementTestApp.Helpers
{
    public static class CryptoHelper
    {
        // DPAPI (Data Protection API) 사용
        // Key 관리는 OS(Windows)가 담당하므로 소스코드에 Key를 하드코딩할 필요가 없음.
        
        // 추가적인 보안을 위해 Entropy(Salt 역할)를 사용할 수 있음.
        // 현재는 null로 설정하여 기본 사용자 계정 키만 사용.
        private static readonly byte[] AdditionalEntropy = null; 

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                
                // DataProtectionScope.CurrentUser: 현재 로그인한 윈도우 사용자만 복호화 가능
                byte[] encryptedData = ProtectedData.Protect(data, AdditionalEntropy, DataProtectionScope.CurrentUser);
                
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception)
            {
                return null; // 또는 예외 전파
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                byte[] data = Convert.FromBase64String(cipherText);
                
                // DataProtectionScope.CurrentUser: 암호화할 때와 동일한 스코프 사용
                byte[] decryptedData = ProtectedData.Unprotect(data, AdditionalEntropy, DataProtectionScope.CurrentUser);
                
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception)
            {
                // 복호화 실패 시 (다른 사용자, 데이터 손상 등) 처리
                return null; 
            }
        }
    }
}
