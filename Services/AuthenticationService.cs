using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using UserManagementTestApp.Models;

namespace UserManagementTestApp.Services
{
    public class AuthenticationService
    {
        private const string UsersFileName = "users.json";
        private readonly string _usersFilePath;

        // 현재 암호화 방식 설정 (나중에 App.config에서 읽어오도록 변경 가능)
        private readonly EncryptionType _currentEncryptionType = EncryptionType.Plain;

        public AuthenticationService()
        {
            _usersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UsersFileName);
        }

        // 비밀번호 정책 검증 (유효성 검사)
        // true: 통과, false: 실패 (message에 사유 리턴)
        public bool ValidatePasswordPolicy(string password, out string message)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                message = "비밀번호를 입력해주세요.";
                return false;
            }

            // [유지보수 포인트] 일반적인 비밀번호 복잡도 패턴 (필요시 주석 해제하여 사용)

            /*
            // 1. 길이 검사 (예: 8자리 이상)
            if (password.Length < 8)
            {
                message = "비밀번호는 최소 8자리 이상이어야 합니다.";
                return false;
            }

            // 2. 숫자 포함 여부
            if (!password.Any(char.IsDigit))
            {
                message = "비밀번호에는 최소 하나의 숫자가 포함되어야 합니다.";
                return false;
            }

            // 3. 영문자 포함 여부
            if (!password.Any(char.IsLetter))
            {
                message = "비밀번호에는 최소 하나의 영문자가 포함되어야 합니다.";
                return false;
            }

            // 4. 특수문자 포함 여부 (예시 특수문자 집합)
            string specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            if (!password.Any(c => specialChars.Contains(c)))
            {
                message = "비밀번호에는 최소 하나의 특수문자가 포함되어야 합니다.";
                return false;
            }
            */

            message = string.Empty;
            return true;
        }

        public User Login(string id, string password)
        {
            // 입력받은 비번을 현재 방식대로 '해시'해서 비교
            string hashedInputPw = HashPassword(password);

            // 관리자 확인
            string adminId = ConfigurationManager.AppSettings["AdminId"];
            string adminPw = ConfigurationManager.AppSettings["AdminPassword"];

            // 주의: 관리자 비번은 Config에 평문으로 저장되어 있다고 가정
            // 만약 관리자 비번도 해시로 저장한다면 여기서 HashPassword(password)와 비교해야 함.

            if (id == adminId && password == adminPw)
            {
                return new User { Id = adminId, Name = "관리자", IsAdmin = true };
            }

            // 사용자 확인
            List<User> users = GetUsers();

            // 사용자 DB의 비번과 비교
            var user = users.FirstOrDefault(u => u.Id == id && u.Password == hashedInputPw);
            return user;
        }

        public List<User> GetUsers()
        {
            if (!File.Exists(_usersFilePath))
            {
                return new List<User>();
            }

            try
            {
                string json = File.ReadAllText(_usersFilePath);
                var serializer = new JavaScriptSerializer();
                var users = serializer.Deserialize<List<User>>(json);
                return users ?? new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        public void AddUser(User newUser)
        {
            List<User> users = GetUsers();
            if (users.Any(u => u.Id == newUser.Id))
            {
                throw new Exception("이미 존재하는 ID입니다.");
            }

            // 저장 전 비밀번호 암호화 (이미 해시된 상태로 들어오지 않는다고 가정)
            // 호출하기 전에 ValidatePasswordPolicy를 통과했다고 가정하거나 여기서 체크

            // 여기서는 이미 Policy를 통과한 '평문'이 들어왔다고 가정하고 해시 처리
            newUser.Password = HashPassword(newUser.Password);

            users.Add(newUser);
            SaveUsers(users);
        }

        public void UpdateUser(User updatedUser)
        {
            List<User> users = GetUsers();
            var existingUser = users.FirstOrDefault(u => u.Id == updatedUser.Id);

            if (existingUser != null)
            {
                // 필드 업데이트
                existingUser.Name = updatedUser.Name;
                existingUser.EmployeeNumber = updatedUser.EmployeeNumber;
                existingUser.Department = updatedUser.Department;

                // 비밀번호 업데이트: 
                // 주의: UI에서 '해시된' 값을 보냈는지 '평문'을 보냈는지 약속이 중요함.
                // 여기서는 "UI는 항상 사용자가 입력한 평문을(혹은 변경 없으면 기존 값을) 보낸다"고 가정.
                // 하지만 기존 값은 이미 해시되어 있을 수 있음.

                // 간단한 해결책:
                // 1. 만약 입력된 비번이 기존 비번과 같다면 -> 변경 없음 (재해싱 금지)
                // 2. 다르다면 -> 새 비번이므로 해싱

                if (existingUser.Password != updatedUser.Password)
                {
                    existingUser.Password = HashPassword(updatedUser.Password);
                }

                SaveUsers(users);
            }
        }

        public void DeleteUser(string id)
        {
            List<User> users = GetUsers();
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
                SaveUsers(users);
            }
        }

        // [유지보수 포인트] 암호화 로직 중앙 관리
        public string HashPassword(string password)
        {
            switch (_currentEncryptionType)
            {
                case EncryptionType.SHA256:

                    using (var sha256 = System.Security.Cryptography.SHA256.Create())
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                        var hash = sha256.ComputeHash(bytes);
                        return Convert.ToBase64String(hash);
                    }

                case EncryptionType.SHA512:
                    using (var sha512 = System.Security.Cryptography.SHA512.Create())
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                        var hash = sha512.ComputeHash(bytes);
                        return Convert.ToBase64String(hash);
                    }

                case EncryptionType.PBKDF2:
                    // .NET 기본 라이브러리 (Rfc2898DeriveBytes) 사용
                    // 주의: 실제 운영 시에는 각 사용자마다 고유한 Salt를 사용해야 안전합니다.
                    // 여기서는 편의상 고정 Salt 또는 간단한 방식을 예시로 듭니다.
                    // 실제로는 DB에 Salt 컬럼을 추가해야 합니다.

                    // 고정 Salt 예시 (보안상 취약할 수 있으나 데모용)
                    byte[] salt = System.Text.Encoding.UTF8.GetBytes("StaticSaltForDemo");

                    // 10000번 반복
                    using (var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 10000))
                    {
                        byte[] hash = pbkdf2.GetBytes(32); // 32 bytes length
                        return Convert.ToBase64String(hash);
                    }

                case EncryptionType.BCrypt:
                    // return BCrypt.Net.BCrypt.HashPassword(password);
                    return password; // 임시

                case EncryptionType.Plain:
                default:
                    return password;
            }
        }

        private void SaveUsers(List<User> users)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(users);
                File.WriteAllText(_usersFilePath, json);
            }
            catch (IOException ex)
            {
                // 파일이 사용 중이거나 접근할 수 없음
                throw new Exception("파일에 접근할 수 없습니다. 다른 프로그램에서 'users.json' 파일을 열고 있는지 확인해주세요.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 권한 없음
                throw new Exception("파일 저장 권한이 없습니다. 관리자 권한으로 실행하거나 파일 권한을 확인해주세요.", ex);
            }
            catch (Exception ex)
            {
                // 기타 오류
                throw new Exception("사용자 데이터를 저장하는 중 알 수 없는 오류가 발생했습니다.", ex);
            }
        }
    }
}
