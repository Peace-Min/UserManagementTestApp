using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UserManagementTestApp.Helpers;
using UserManagementTestApp.Models;
using UserManagementTestApp.Services;

namespace UserManagementTestApp.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        private readonly AuthenticationService _authService;

        // 데이터 목록
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        // 선택된 사용자
        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    OnUserSelected();
                }
            }
        }

        // 입력 폼 데이터 (별도 모델로 분리)
        private UserFormModel _userForm;
        public UserFormModel UserForm
        {
            get => _userForm;
            set => SetProperty(ref _userForm, value);
        }

        // 상태 Properties
        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (SetProperty(ref _isEditMode, value))
                {
                    OnPropertyChanged(nameof(IsIdEnabled));
                }
            }
        }

        private bool _isDeleteMode;
        public bool IsDeleteMode
        {
            get => _isDeleteMode;
            set
            {
                if (SetProperty(ref _isDeleteMode, value))
                {
                    UpdateUIState();
                }
            }
        }

        // UI 제어 Properties
        public bool IsIdEnabled => !_isEditMode;

        private bool _isFormEnabled = true;
        public bool IsFormEnabled
        {
            get => _isFormEnabled;
            set => SetProperty(ref _isFormEnabled, value);
        }

        private string _applyButtonText = "적용";
        public string ApplyButtonText
        {
            get => _applyButtonText;
            set => SetProperty(ref _applyButtonText, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        // Commands
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand ClearCommand { get; }

        public UserManagementViewModel()
        {
            _authService = new AuthenticationService();
            UserForm = new UserFormModel(); // 폼 모델 초기화
            LoadUsers();

            AddCommand = new RelayCommand(ExecuteAdd);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            ApplyCommand = new RelayCommand(ExecuteApply);
            ClearCommand = new RelayCommand(ExecuteClear);
        }

        private void LoadUsers()
        {
            var userList = _authService.GetUsers();
            Users = new ObservableCollection<User>(userList);
        }

        private void ExecuteAdd(object obj)
        {
            ClearForm();
        }

        private void ExecuteClear(object obj)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            UserForm.Clear(); // 폼 모델 초기화 위임
            ErrorMessage = "";

            IsEditMode = false;
            IsDeleteMode = false;
            SelectedUser = null;

            ApplyButtonText = "적용";
            IsFormEnabled = true;
        }

        private void OnUserSelected()
        {
            if (SelectedUser != null)
            {
                IsEditMode = true;
                IsDeleteMode = false;
                ApplyButtonText = "적용";
                IsFormEnabled = true;
                ErrorMessage = "";

                // 폼 모델에 데이터 매핑
                UserForm.Id = SelectedUser.Id;
                UserForm.Name = SelectedUser.Name;
                UserForm.EmployeeNumber = SelectedUser.EmployeeNumber;
                UserForm.Department = SelectedUser.Department;

                // 수정 모드에서는 비밀번호 필드를 비워둠 (암호화된 값을 표시하지 않음)
                // 사용자가 새 비밀번호를 입력하지 않으면 기존 비밀번호가 유지됨
                UserForm.Password = "";
                UserForm.ConfirmPassword = "";
            }
        }

        private void ExecuteDelete(object obj)
        {
            ErrorMessage = "";

            if (SelectedUser == null)
            {
                ErrorMessage = "삭제할 사용자를 선택해주세요.";
                return;
            }

            IsDeleteMode = true;
            ApplyButtonText = "삭제 확정";
            // 비밀번호 입력을 위해 폼은 활성화 상태 유지
            ErrorMessage = "삭제하시려면 하단에 비밀번호를 입력하고 '삭제 확정' 버튼을 눌러주세요.";

            // 비밀번호 초기화
            UserForm.Password = "";
            UserForm.ConfirmPassword = "";
        }

        private void ExecuteApply(object obj)
        {
            if (IsDeleteMode)
            {
                ProcessDelete();
            }
            else
            {
                ProcessAddOrUpdate();
            }
        }

        private void ProcessDelete()
        {
            ErrorMessage = "";
            if (SelectedUser == null) return;

            if (string.IsNullOrWhiteSpace(UserForm.Password))
            {
                ErrorMessage = "삭제하려면 비밀번호를 입력해주세요.";
                return;
            }

            if (UserForm.Password != UserForm.ConfirmPassword)
            {
                ErrorMessage = "비밀번호 확인이 일치하지 않습니다.";
                return;
            }

            string inputHash = _authService.HashPassword(UserForm.Password);
            if (SelectedUser.Password != inputHash)
            {
                ErrorMessage = "비밀번호가 올바르지 않아 삭제할 수 없습니다.";
                return;
            }

            try
            {
                _authService.DeleteUser(SelectedUser.Id);
                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"삭제 중 오류 발생: {ex.Message}";
            }
        }

        private void ProcessAddOrUpdate()
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(UserForm.Id) || string.IsNullOrWhiteSpace(UserForm.Name))
            {
                ErrorMessage = "필수 정보를 입력해주세요 (ID, 이름).";
                return;
            }

            bool isPasswordProvided = !string.IsNullOrWhiteSpace(UserForm.Password);

            if (!IsEditMode && !isPasswordProvided)
            {
                ErrorMessage = "비밀번호를 입력해주세요.";
                return;
            }

            if (isPasswordProvided)
            {
                if (UserForm.Password != UserForm.ConfirmPassword)
                {
                    ErrorMessage = "비밀번호 확인이 일치하지 않습니다.";
                    return;
                }

                if (!_authService.ValidatePasswordPolicy(UserForm.Password, out string policyMsg))
                {
                    ErrorMessage = policyMsg;
                    return;
                }
            }

            User user = new User
            {
                Id = UserForm.Id,
                Name = UserForm.Name,
                EmployeeNumber = UserForm.EmployeeNumber,
                Department = UserForm.Department,
                Password = isPasswordProvided ? UserForm.Password : SelectedUser?.Password
            };

            try
            {
                if (IsEditMode)
                {
                    _authService.UpdateUser(user);
                }
                else
                {
                    _authService.AddUser(user);
                }

                LoadUsers();
                ClearForm();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"오류 발생: {ex.Message}";
            }
        }

        private void UpdateUIState()
        {
            OnPropertyChanged(nameof(IsIdEnabled));
            OnPropertyChanged(nameof(IsFormEnabled));
            OnPropertyChanged(nameof(ApplyButtonText));
        }
    }
}
