using System.ComponentModel;
using System.Runtime.CompilerServices;
using UserManagementTestApp.Models;

namespace UserManagementTestApp.Services
{
    // 현재 로그인된 사용자 정보를 보유하는 싱글톤 세션 클래스
    public class UserSession : INotifyPropertyChanged
    {
        private static UserSession _instance;
        public static UserSession Instance => _instance ?? (_instance = new UserSession());

        private User _currentUser;

        // 외부에서 함부로 생성하지 못하도록 private 생성자
        private UserSession() { }

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLoggedIn));
                    OnPropertyChanged(nameof(UserName));
                }
            }
        }

        public bool IsLoggedIn => _currentUser != null;

        public string UserName => _currentUser?.Name;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Clear()
        {
            CurrentUser = null;
        }
    }
}
