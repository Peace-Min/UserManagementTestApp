using System.Linq;
using UserManagementTestApp.Data;
using UserManagementTestApp.Models;

namespace UserManagementTestApp.Services
{
    public class AuthenticationService
    {
        private readonly UserRepository _repository;

        public AuthenticationService()
        {
            _repository = new UserRepository();
        }

        public bool ValidatePasswordPolicy(string password, out string message)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                message = "비밀번호를 입력해주세요.";
                return false;
            }
            // 여기에 복잡한 정책(길이, 특수문자 등)을 추가할 수 있습니다.
            
            message = string.Empty;
            return true;
        }

        public bool CheckIdExists(string id)
        {
            var users = _repository.LoadAll();
            return users.Any(u => u.Id == id);
        }

        public User Login(string id, string password)
        {
            // 사용자를 로드 (UserRepository가 이미 복호화된 데이터를 제공)
            var users = _repository.LoadAll();

            // ID/PW 일치 여부 확인
            var user = users.FirstOrDefault(u => u.Id == id && u.Password == password);
            return user;
        }
    }
}
