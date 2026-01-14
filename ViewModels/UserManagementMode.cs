namespace UserManagementTestApp.ViewModels
{
    public enum UserManagementMode
    {
        View,   // 기본 조회 상태 (아무것도 선택 안됨 또는 단순 조회)
        Add,    // 추가 모드 (신규 등록) -> 사실 View 상태에서 입력하면 Add로 간주할 수도 있으나, 명시적으로 구분 가능
        Edit,   // 수정 모드 (사용자 선택됨)
    }
}
