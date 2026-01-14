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
        private readonly UserService _userService;

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

        // 상태 관리 (Enum)
        private UserManagementMode _currentMode;
        public UserManagementMode CurrentMode
        {
            get => _currentMode;
            set
            {
                if (SetProperty(ref _currentMode, value))
                {
                    UpdateUIState();
                }
            }
        }

        // UI 제어 Properties (Mode에 따라 자동 계산)
        
        // 아이디 입력 가능 여부: View(신규입력)일 때만 가능, Edit/Delete 때는 불가능
        public bool IsIdEnabled => _currentMode == UserManagementMode.View;

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
            _userService = new UserService();
            UserForm = new UserFormModel(); 
            LoadUsers();

            AddCommand = new RelayCommand(ExecuteAdd);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            ApplyCommand = new RelayCommand(ExecuteApply);
            ClearCommand = new RelayCommand(ExecuteClear);

            // 초기 상태
            CurrentMode = UserManagementMode.View;
        }

        private void LoadUsers()
        {
            var userList = _userService.GetUsers();
            Users = new ObservableCollection<User>(userList);
        }

        private void ExecuteAdd(object obj)
        {
            ClearForm();
            CurrentMode = UserManagementMode.View;
        }

        private void ExecuteClear(object obj)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            UserForm.Clear(); 
            ErrorMessage = "";
            SelectedUser = null;
            
            // 초기화 시 View 모드로 복귀
            CurrentMode = UserManagementMode.View;
        }

        private void OnUserSelected()
        {
            if (SelectedUser != null)
            {
                // 사용자 선택 시 수정 모드로 진입
                // 폼 모델에 데이터 매핑
                UserForm.Id = SelectedUser.Id;
                UserForm.Name = SelectedUser.Name;
                UserForm.EmployeeNumber = SelectedUser.EmployeeNumber;
                UserForm.Department = SelectedUser.Department;
                
                UserForm.Password = "";
                UserForm.ConfirmPassword = "";

                ErrorMessage = "";
                
                // 모드 전환
                CurrentMode = UserManagementMode.Edit;
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

            // 관리자 확인 등의 절차 없이 즉시 삭제 (확인 팝업만 제공)
            var result = MessageBox.Show($"'{SelectedUser.Name}' 사용자를 삭제하시겠습니까?", 
                                         "삭제 확인", 
                                         MessageBoxButton.YesNo, 
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _userService.DeleteUser(SelectedUser.Id);
                    LoadUsers();
                    ClearForm(); // 삭제 후 초기화
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"삭제 중 오류 발생: {ex.Message}";
                }
            }
        }

        private void ExecuteApply(object obj)
        {
            switch (CurrentMode)
            {
                case UserManagementMode.View:
                    CreateUser();
                    break;
                case UserManagementMode.Edit: 
                    ModifyUser();
                    break;
            }
        }

        private bool ValidateUserInput(bool isEditMode)
        {
            if (string.IsNullOrWhiteSpace(UserForm.Id) || string.IsNullOrWhiteSpace(UserForm.Name))
            {
                ErrorMessage = "필수 정보를 입력해주세요 (ID, 이름).";
                return false;
            }

            bool isPasswordProvided = !string.IsNullOrWhiteSpace(UserForm.Password);

            // 신규 생성 시 비밀번호 필수
            if (!isEditMode && !isPasswordProvided)
            {
                ErrorMessage = "비밀번호를 입력해주세요.";
                return false;
            }

            if (isPasswordProvided)
            {
                if (UserForm.Password != UserForm.ConfirmPassword)
                {
                    ErrorMessage = "비밀번호 확인이 일치하지 않습니다.";
                    return false;
                }

                if (!_authService.ValidatePasswordPolicy(UserForm.Password, out string policyMsg))
                {
                    ErrorMessage = policyMsg;
                    return false;
                }
            }
            return true;
        }

        private void CreateUser()
        {
            ErrorMessage = "";
            if (!ValidateUserInput(isEditMode: false)) return;

            User newUser = new User
            {
                Id = UserForm.Id,
                Name = UserForm.Name,
                EmployeeNumber = UserForm.EmployeeNumber,
                Department = UserForm.Department,
                Password = UserForm.Password
            };

            try
            {
                _userService.AddUser(newUser);
                LoadUsers();
                ClearForm();
                ErrorMessage = "사용자가 등록되었습니다.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"등록 오류: {ex.Message}";
            }
        }

        private void ModifyUser()
        {
            ErrorMessage = "";
            if (!ValidateUserInput(isEditMode: true)) return;

            // 수정 시 비밀번호가 비어있으면 기존 비밀번호 유지
            string finalPassword = string.IsNullOrWhiteSpace(UserForm.Password) 
                ? SelectedUser?.Password 
                : UserForm.Password;

            User updatedUser = new User
            {
                Id = UserForm.Id, // ID는 변경 불가 (또는 정책에 따라)
                Name = UserForm.Name,
                EmployeeNumber = UserForm.EmployeeNumber,
                Department = UserForm.Department,
                Password = finalPassword
            };

            try
            {
                _userService.UpdateUser(updatedUser);
                LoadUsers();
                ClearForm();
                ErrorMessage = "사용자 정보가 수정되었습니다.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"수정 오류: {ex.Message}";
            }
        }

        private void UpdateUIState()
        {
            // 모든 UI 상태를 CurrentMode 하나를 기준으로 결정
            OnPropertyChanged(nameof(IsIdEnabled));
            
            switch (CurrentMode)
            {
                case UserManagementMode.View:
                    ApplyButtonText = "추가"; // 신규 등록 의미
                    IsFormEnabled = true;
                    break;
                    
                case UserManagementMode.Edit:
                    ApplyButtonText = "수정";
                    IsFormEnabled = true;
                    break;
            }
            
            OnPropertyChanged(nameof(ApplyButtonText));
            OnPropertyChanged(nameof(IsFormEnabled));
        }
    }
}
