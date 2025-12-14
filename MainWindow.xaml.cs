using System.Windows;

namespace UserManagementTestApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnUserManagement_Click(object sender, RoutedEventArgs e)
        {
            // 실제 앱에서는 현재 사용자가 관리자(Admin)인지 확인하세요.
            // 여기서는 단순히 창을 엽니다.
            UserManagementWindow userWindow = new UserManagementWindow();
            userWindow.ShowDialog();
        }
    }
}
