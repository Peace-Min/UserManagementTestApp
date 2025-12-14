using UserManagementTestApp.Helpers;

namespace UserManagementTestApp.ViewModels
{
    /// <summary>
    /// 사용자 입력 폼 데이터 모델
    /// - ViewModel에서 개별 필드를 직접 관리하지 않고 별도 모델로 분리
    /// - UI 바인딩 및 데이터 초기화 관리 용이
    /// </summary>
    public class UserFormModel : ViewModelBase
    {
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _employeeNumber; // 사번 (조직 내 고유 번호)
        public string EmployeeNumber
        {
            get => _employeeNumber;
            set => SetProperty(ref _employeeNumber, value);
        }

        private string _department; // 부서 (소속 조직)
        public string Department
        {
            get => _department;
            set => SetProperty(ref _department, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        /// <summary>
        /// 폼 데이터 전체 초기화
        /// </summary>
        public void Clear()
        {
            Id = "";
            Name = "";
            EmployeeNumber = "";
            Department = "";
            Password = "";
            ConfirmPassword = "";
        }
    }
}
