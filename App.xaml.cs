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
