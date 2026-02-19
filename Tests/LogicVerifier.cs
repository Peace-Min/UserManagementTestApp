using System;
using System.IO;
using System.Diagnostics;
using UserManagementTestApp.Helpers;
using UserManagementTestApp.Models;
using UserManagementTestApp.Services;
using UserManagementTestApp.Data;

namespace UserManagementTestApp.Tests
{
    public static class LogicVerifier
    {
        public static void RunTests()
        {
            Console.WriteLine("=== 기능 로직 검증 테스트 시작 ===");

            try
            {
                TestCryptoHelper();
                TestUserService();
                TestAuthenticationService();
                TestPasswordValidation();
                
                Console.WriteLine("\n[SUCCESS] 모든 테스트를 통과했습니다!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[FAIL] 테스트 실패: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static void TestCryptoHelper()
        {
            Console.Write("1. CryptoHelper (암복호화) 테스트... ");
            
            string original = "TestPassword123!";
            string encrypted = CryptoHelper.Encrypt(original);
            string decrypted = CryptoHelper.Decrypt(encrypted);

            if (original == encrypted) throw new Exception("암호화가 제대로 수행되지 않았습니다 (평문과 동일).");
            if (original != decrypted) throw new Exception($"복호화 결과가 원본과 다릅니다. (원본: {original}, 복호화: {decrypted})");

            Console.WriteLine("OK");
        }

        private static void TestUserService()
        {
            Console.Write("2. UserService (CRUD) 테스트... ");

            // 테스트를 위해 기존 파일 백업 또는 임시 파일 사용이 이상적이나, 
            // 여기서는 단순 검증을 위해 파일이 생성됨을 인지하고 진행.
            // (실제 환경에서는 Test용 별도 파일 경로를 주입받도록 설계하는 것이 좋음)
            
            var userService = new UserService();
            string testId = "test_user_01";
            
            // 정리 (기존에 있다면 삭제)
            if (userService.Exists(testId))
            {
                userService.DeleteUser(testId);
            }

            // A. 추가 테스트
            var newUser = new User 
            { 
                Id = testId, 
                Name = "테스트유저", 
                Password = "pwd" 
            };
            userService.AddUser(newUser);

            if (!userService.Exists(testId)) throw new Exception("사용자 추가 실패: 파일에 저장되지 않음.");

            // B. 중복 방지 테스트
            try
            {
                userService.AddUser(newUser);
                throw new Exception("중복 ID 추가가 허용되었습니다.");
            }
            catch (Exception ex) 
            {
                if (ex.Message != "이미 존재하는 ID입니다.") throw;
            }

            // C. 수정 테스트
            var updateUser = new User
            {
                Id = testId,
                Name = "수정된이름",
                Password = "new_pwd"
            };
            userService.UpdateUser(updateUser);
            
            var retrieved = userService.GetUserById(testId);
            if (retrieved.Name != "수정된이름") throw new Exception("사용자 이름 수정 실패");
            if (retrieved.Password != "new_pwd") throw new Exception("사용자 비밀번호 수정 실패");

            // D. 삭제 테스트 (마지막에 수행)
            userService.DeleteUser(testId);
            if (userService.Exists(testId)) throw new Exception("사용자 삭제 실패");

            Console.WriteLine("OK");
        }

        private static void TestAuthenticationService()
        {
            Console.Write("3. AuthenticationService (로그인) 테스트... ");

            var authService = new AuthenticationService();
            var userService = new UserService();
            string testId = "auth_test_user";
            string testPw = "AuthPass1!";

            // 테스트 데이터 준비
            if (userService.Exists(testId)) userService.DeleteUser(testId);
            userService.AddUser(new User { Id = testId, Name = "AuthTester", Password = testPw });

            // A. 로그인 성공 테스트
            var loggedInUser = authService.Login(testId, testPw);
            if (loggedInUser == null) throw new Exception("올바른 정보로 로그인 실패");
            if (loggedInUser.Id != testId) throw new Exception("로그인된 사용자 ID 불일치");

            // B. 로그인 실패 테스트 (비번 틀림)
            var failUser = authService.Login(testId, "WrongPass");
            if (failUser != null) throw new Exception("틀린 비밀번호로 로그인이 허용됨");

            // C. 로그인 실패 테스트 (ID 없음)
            var noUser = authService.Login("unknown_id", testPw);
            if (noUser != null) throw new Exception("존재하지 않는 ID로 로그인이 허용됨");

            // 정리
            userService.DeleteUser(testId);

            Console.WriteLine("OK");
        }

        private static void TestPasswordValidation()
        {
            Console.Write("4. PasswordPolicy (비밀번호 규칙) 테스트... ");
            var auth = new AuthenticationService();
            string msg;

            // 1. 길이 실패 (짧음)
            if (auth.ValidatePasswordPolicy("user", "Short1!", out msg)) throw new Exception("길이 미달 비번 허용됨");
            if (!msg.Contains("9~12자리")) throw new Exception("길이 오류 메시지 불일치");

            // 1. 길이 실패 (김)
            if (auth.ValidatePasswordPolicy("user", "TooLongPassword123!", out msg)) throw new Exception("길이 초과 비번 허용됨");

            // 1. 복잡성 실패 (숫자 없음)
            if (auth.ValidatePasswordPolicy("user", "NoDigitPass!", out msg)) throw new Exception("숫자 없는 비번 허용됨");

            // 1. 복잡성 실패 (특수문자 없음)
            if (auth.ValidatePasswordPolicy("user", "NoSpecial123", out msg)) throw new Exception("특수문자 없는 비번 허용됨");

            // 4. 아이디 포함 실패 (대소문자 무시 체크)
            if (auth.ValidatePasswordPolicy("TestUser", "123testuser!", out msg)) throw new Exception("대소문자 다른 아이디 포함 비번 허용됨");

            // 2. 동일 문자 3회
            if (auth.ValidatePasswordPolicy("user", "aaapass1!", out msg)) throw new Exception("동일 문자 3회 비번 허용됨");
            if (!msg.Contains("동일한 문자")) throw new Exception("동일 문자 오류 메시지 불일치");

            // 3. 연속 문자 (abc)
            if (auth.ValidatePasswordPolicy("user", "abcPass1!", out msg)) throw new Exception("연속 문자 비번 허용됨");
            
            // 3. 연속 문자 (대소문자 혼합 abc -> aBc)
            if (auth.ValidatePasswordPolicy("user", "aBcPass1!", out msg)) throw new Exception("대소문자 혼합 연속 문자 비번 허용됨");
            
            // 3. 연속 문자 (역순 cba)
            if (auth.ValidatePasswordPolicy("user", "cbaPass1!", out msg)) throw new Exception("역순 문자 비번 허용됨");

            // 3. 연속 숫자 (123)
            if (auth.ValidatePasswordPolicy("user", "Pass123!", out msg)) throw new Exception("연속 숫자 비번 허용됨");
            
            // 3. 연속 숫자 (역순 321)
            if (auth.ValidatePasswordPolicy("user", "Pass321!", out msg)) throw new Exception("역순 숫자 비번 허용됨");

            // 성공 케이스
            if (!auth.ValidatePasswordPolicy("user", "ValidPass1!", out msg)) throw new Exception($"정상 비번 거부됨: {msg}");

            Console.WriteLine("OK");
        }
    }
}
