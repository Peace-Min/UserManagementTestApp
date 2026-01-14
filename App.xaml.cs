using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UserManagementTestApp
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if DEBUG
            // 앱 시작 시 로직 검증 테스트 실행 (Output 창 확인)
            try
            {
                UserManagementTestApp.Tests.LogicVerifier.RunTests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"테스트 실행 중 치명적 오류: {ex.Message}");
            }
#endif

            string requireLogin = System.Configuration.ConfigurationManager.AppSettings["RequireLogin"];

            // "false"일 때만 로그인을 건너뜁니다. (기본값 또는 없으면 로그인 수행)
            if (requireLogin != null && requireLogin.ToLower() == "false")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
            }
        }
    }
}
