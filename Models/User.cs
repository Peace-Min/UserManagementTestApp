using System;

namespace UserManagementTestApp.Models
{
    /// <summary>
    /// 사용자 정보 모델
    /// - Id: 사용자 고유 식별자 (로그인 ID)
    /// - Name: 사용자 이름
    /// - EmployeeNumber: 사번 (조직 내 고유 번호)
    /// - Department: 부서 (소속 조직)
    /// - Password: 비밀번호 (실제 운영 시 해시 저장 권장)
    /// - IsAdmin: 관리자 여부
    /// </summary>
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EmployeeNumber { get; set; } // 사번
        public string Department { get; set; }      // 부서
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
