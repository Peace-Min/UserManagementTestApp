using System.Windows;
using UserManagementTestApp.ViewModels;

namespace UserManagementTestApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            // ViewModel의 이벤트를 구독하여 화면 전환 처리
            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSuccess += Vm_LoginSuccess;
                vm.RequestClose += Vm_RequestClose;
            }
        }

        private void Vm_LoginSuccess(object sender, Models.User user)
        {
            // 로그인 성공 시 사용자 관리 창으로 이동
            UserManagementWindow userMgmtWin = new UserManagementWindow();
            userMgmtWin.Show();
            this.Close();
        }

        private void Vm_RequestClose(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
