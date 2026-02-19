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

        public bool ValidatePasswordPolicy(string userId, string password, out string message)
        {
            message = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                message = "비밀번호를 입력해주세요.";
                return false;
            }

            // 1. 9~12자리, 문자/숫자/특수문자 포함
            if (password.Length < 9 || password.Length > 12)
            {
                message = "비밀번호는 문자, 숫자, 특수문자를 포함하여 9~12자리로 설정해야합니다.";
                return false;
            }

            bool hasLetter = password.Any(char.IsLetter);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            if (!hasLetter || !hasDigit || !hasSpecial)
            {
                message = "비밀번호는 문자, 숫자, 특수문자를 포함하여 9~12자리로 설정해야합니다.";
                return false;
            }

            // 4. 사용자 ID 포함 불가 (대소문자 무시)
            if (!string.IsNullOrEmpty(userId) && password.IndexOf(userId, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                message = "비밀번호에 사용자 ID가 존재합니다.";
                return false;
            }

            // 2. 동일 문자 3회 이상 중복 불가 (예: aaa)
            // 3. 연속된 숫자/문자 불가 (예: 123, abc)
            for (int i = 0; i < password.Length - 2; i++)
            {
                char c1 = password[i];
                char c2 = password[i + 1];
                char c3 = password[i + 2];

                // 동일 문자 3회 (대문자/소문자 구분함 - aaa는 안되지만 aAa는 허용, 또는 정책에 따라 다름. 
                // 피드백 코드에서는 c1==c2 && c2==c3 유지. 즉 Case Sensitive.)
                if (c1 == c2 && c2 == c3)
                {
                    message = "비밀번호는 동일한 문자를 3회이상 입력할 수 없습니다.";
                    return false;
                }

                // 연속된 문자/숫자
                if (IsSequence(c1, c2, c3))
                {
                    message = "비밀번호는 연속된 문자, 숫자로 할 수 없습니다.";
                    return false;
                }
            }
            
            return true;
        }

        private bool IsSequence(char c1, char c2, char c3)
        {
            // 숫자 연속성 체크 (오름차순 123, 내림차순 321)
            if (char.IsDigit(c1) && char.IsDigit(c2) && char.IsDigit(c3))
            {
                if (c2 == c1 + 1 && c3 == c2 + 1) return true; // 123
                if (c2 == c1 - 1 && c3 == c2 - 1) return true; // 321
            }

            // 영문자 연속성 체크 (대소문자 무시를 위해 소문자로 변환 후 비교)
            if (char.IsLetter(c1) && char.IsLetter(c2) && char.IsLetter(c3))
            {
                char l1 = char.ToLower(c1);
                char l2 = char.ToLower(c2);
                char l3 = char.ToLower(c3);

                if (l2 == l1 + 1 && l3 == l2 + 1) return true; // abc
                if (l2 == l1 - 1 && l3 == l2 - 1) return true; // cba
            }

            return false;
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
