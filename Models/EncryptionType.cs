namespace UserManagementTestApp.Models
{
    public enum EncryptionType
    {
        Plain,      // 평문
        SHA256,     // SHA256 해시 (기본 제공)
        SHA512,     // SHA512 해시 (기본 제공 - 더 강력함)
        PBKDF2,     // PBKDF2 (Rfc2898DeriveBytes - 기본 제공, 반복 해싱으로 보안성 우수)
        BCrypt,     // BCrypt (외부 라이브러리 필요)
        // ... 여기에 계속 추가 가능
    }
}
