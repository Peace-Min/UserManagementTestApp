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

            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "아이디와 비밀번호를 입력해주세요.";
                return;
            }

            try
            {
                User user = _authService.Login(Id, Password);
                if (user != null)
                {
                    // 로그인 성공 이벤트 발생
                    LoginSuccess?.Invoke(this, user);
                }
                else
                {
                    ErrorMessage = "아이디 또는 비밀번호가 올바르지 않습니다.";
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
