using System;
using System.Windows.Input;
using UserManagementTestApp.Helpers;
using UserManagementTestApp.Models;
using UserManagementTestApp.Services;

namespace UserManagementTestApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;

        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                if (SetProperty(ref _id, value))
                {
                    ErrorMessage = ""; // 입력 시 에러 메시지 초기화
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ErrorMessage = ""; // 입력 시 에러 메시지 초기화
                }
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }

        // 로그인 성공 시 View에 알리기 위한 이벤트
        public event EventHandler<User> LoginSuccess;
        public event EventHandler RequestClose;

        public LoginViewModel()
        {
            _authService = new AuthenticationService();
            LoginCommand = new RelayCommand(ExecuteLogin);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private void ExecuteLogin(object obj)
        {
            ErrorMessage = ""; // 초기화

            // 1. 텍스트 입력 확인 (순차적)
            if (string.IsNullOrWhiteSpace(Id))
            {
                ErrorMessage = "아이디를 입력해주세요.";
                return;
            }

            // 2. 아이디 유효성 확인 (존재 여부)
            // 텍스트가 있어도 아이디가 없으면 비밀번호 입력 여부와 상관없이 아이디 에러를 먼저 내거나,
            // 텍스트 입력을 모두 확인한 후 아이디 유효성을 체크하는 것이 일반적이나
            // 사용자의 '순차적' 요청에 따라 아이디 입력됨 -> 아이디 유효성 체크 -> 비밀번호 입력됨 -> 비밀번호 일치 체크 순으로 갈 수도 있음.
            // 여기서는 요청된 순서(1. 텍스트, 2. 아이디유효성, 3. 비밀번호 등)를 고려하여
            // 모든 텍스트 필드가 채워져 있는지 먼저 확인.
            
            if (string.IsNullOrWhiteSpace(Password))
            {
                // 아이디는 입력되었으나 비밀번호가 없는 경우
                ErrorMessage = "비밀번호를 입력해주세요.";
                return;
            }

            try
            {
                // 3. 아이디 존재 여부 확인
                bool isIdValid = _authService.CheckIdExists(Id);
                if (!isIdValid)
                {
                    ErrorMessage = "존재하지 않는 아이디입니다.";
                    return;
                }

                // 4. 아이디 + 비밀번호 확인 (Login 호출)
                User user = _authService.Login(Id, Password);
                if (user != null)
                {
                    // 로그인 성공 이벤트 발생
                    LoginSuccess?.Invoke(this, user);
                }
                else
                {
                    // 아이디는 존재하나 로그인이 실패했으므로 비밀번호 불일치
                    ErrorMessage = "비밀번호가 일치하지 않습니다.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"로그인 중 오류 발생: {ex.Message}";
            }
        }

        private void ExecuteCancel(object obj)
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
