using System.Windows;
using System.Windows.Controls;

namespace UserManagementTestApp.Helpers
{
    public static class PasswordBoxMonitor
    {
        public static readonly DependencyProperty PasswordLengthProperty =
            DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor), new UIPropertyMetadata(0));

        public static int GetPasswordLength(DependencyObject obj)
        {
            return (int)obj.GetValue(PasswordLengthProperty);
        }

        public static void SetPasswordLength(DependencyObject obj, int value)
        {
            obj.SetValue(PasswordLengthProperty, value);
        }

        public static readonly DependencyProperty MonitorPasswordProperty =
            DependencyProperty.RegisterAttached("MonitorPassword", typeof(bool), typeof(PasswordBoxMonitor), new UIPropertyMetadata(false, OnMonitorPasswordChanged));

        public static bool GetMonitorPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(MonitorPasswordProperty);
        }

        public static void SetMonitorPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(MonitorPasswordProperty, value);
        }

        private static void OnMonitorPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = d as PasswordBox;
            if (passwordBox == null) return;

            passwordBox.PasswordChanged -= PasswordChanged;

            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            SetPasswordLength(passwordBox, passwordBox.Password.Length);
        }
    }

    // 추가: 바인딩 가능한 Password 속성 (VM과 양방향 바인딩 지원 시도)
    public static class BindablePasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(BindablePasswordHelper),
                new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null) return;

            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            if (!_isUpdating)
            {
                passwordBox.Password = (string)e.NewValue;
            }

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            _isUpdating = true;
            SetPassword(passwordBox, passwordBox.Password);
            _isUpdating = false;
        }

        private static bool _isUpdating;
    }
}
