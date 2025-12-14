# User Management Test App

## 📋 프로젝트 개요

**User Management Test App**은 WPF 기반의 사용자 인증 및 관리 시스템 테스트 애플리케이션입니다. 
조직 내 사용자 정보를 관리하고, 로그인 인증, 비밀번호 암호화, MVVM 패턴 구현 등 실무 환경에서 필요한 핵심 기능들을 학습하고 테스트하기 위한 목적으로 개발되었습니다.

## 🎯 주요 기능

### 1. 사용자 인증 시스템
- **로그인 기능**: 사용자 ID와 비밀번호 기반 인증
- **관리자 모드**: App.config를 통한 관리자 계정 설정
- **로그인 우회 옵션**: 개발/테스트 시 로그인 스킵 가능 (`RequireLogin` 설정)

### 2. 사용자 관리 (CRUD)
- **추가**: 새로운 사용자 등록 (ID, 이름, 사번, 부서, 비밀번호)
- **조회**: 등록된 사용자 목록 표시
- **수정**: 기존 사용자 정보 업데이트
- **삭제**: 비밀번호 확인 후 안전한 사용자 삭제

### 3. 보안 기능
- **비밀번호 암호화**: 
  - Plain (평문)
  - SHA256
  - SHA512
  - PBKDF2
  - BCrypt (준비됨)
- **비밀번호 정책 검증**: 확장 가능한 정책 프레임워크
- **삭제 시 비밀번호 재확인**: 중요 작업 전 2차 인증

### 4. UI/UX
- **다크 테마**: 모던한 다크 모드 UI
- **인라인 에러 메시지**: 팝업 대신 폼 내부에 에러 표시
- **반응형 상태 관리**: 추가/수정/삭제 모드별 UI 자동 전환

## 🏗️ 기술 스택

### 프레임워크 & 언어
- **.NET Framework 4.7.2**
- **WPF (Windows Presentation Foundation)**
- **C#**

### 아키텍처 패턴
- **MVVM (Model-View-ViewModel)**
  - `ViewModelBase`: INotifyPropertyChanged 구현
  - `RelayCommand`: ICommand 구현
  - 데이터 바인딩을 통한 UI-로직 분리

### 데이터 저장
- **JSON 파일 기반**: `users.json`으로 사용자 정보 영구 저장
- **JavaScriptSerializer**: .NET 기본 JSON 직렬화

### 보안
- **System.Security.Cryptography**: 비밀번호 해싱
- **다중 암호화 알고리즘 지원**

## 📁 프로젝트 구조

```
UserManagementTestApp/
├── Models/
│   ├── User.cs                    # 사용자 데이터 모델
│   └── EncryptionType.cs          # 암호화 타입 열거형
├── ViewModels/
│   ├── LoginViewModel.cs          # 로그인 화면 뷰모델
│   ├── UserManagementViewModel.cs # 사용자 관리 화면 뷰모델
│   └── UserFormModel.cs           # 입력 폼 바인딩 모델
├── Views/
│   ├── LoginWindow.xaml           # 로그인 화면
│   ├── UserManagementWindow.xaml  # 사용자 관리 화면
│   └── MainWindow.xaml            # 메인 화면
├── Services/
│   └── AuthenticationService.cs   # 인증 및 사용자 관리 서비스
├── Helpers/
│   ├── ViewModelBase.cs           # MVVM 기본 클래스
│   ├── RelayCommand.cs            # 커맨드 헬퍼
│   └── PasswordBoxMonitor.cs      # PasswordBox 바인딩 헬퍼
└── App.config                     # 애플리케이션 설정
```

## 🚀 시작하기

### 필수 요구사항
- Windows 10 이상
- .NET Framework 4.7.2 이상
- Visual Studio 2019 이상 (권장)

### 설치 및 실행

1. **저장소 클론**
   ```bash
   git clone https://github.com/yourusername/UserManagementTestApp.git
   cd UserManagementTestApp
   ```

2. **Visual Studio에서 열기**
   - `UserManagementTestApp.sln` 파일을 Visual Studio로 엽니다.

3. **빌드 및 실행**
   - `F5` 키를 눌러 디버그 모드로 실행
   - 또는 `Ctrl+F5`로 디버그 없이 실행

### 기본 설정 (App.config)

```xml
<appSettings>
  <add key="AdminId" value="admin" />
  <add key="AdminPassword" value="1111" />
  <add key="RequireLogin" value="true" />
</appSettings>
```

- **AdminId**: 관리자 계정 ID
- **AdminPassword**: 관리자 비밀번호
- **RequireLogin**: `true`면 로그인 필수, `false`면 로그인 스킵

## 💡 사용 방법

### 1. 로그인
- 기본 관리자 계정: `admin` / `1111`
- 또는 등록된 사용자 계정으로 로그인

### 2. 사용자 추가
1. "추가" 버튼 클릭
2. 사용자 정보 입력 (ID, 이름, 사번, 부서, 비밀번호)
3. "적용" 버튼 클릭

### 3. 사용자 수정
1. 목록에서 수정할 사용자 선택
2. 정보 수정
3. "적용" 버튼 클릭

### 4. 사용자 삭제
1. 목록에서 삭제할 사용자 선택
2. "삭제" 버튼 클릭
3. 비밀번호 입력
4. "삭제 확정" 버튼 클릭

## 🔐 보안 고려사항

### 현재 구현
- ✅ 비밀번호 해싱 (SHA256 기본)
- ✅ 삭제 시 비밀번호 재확인
- ✅ 비밀번호 정책 프레임워크

### 프로덕션 환경 권장사항
- ⚠️ **Admin 비밀번호 암호화**: 현재 App.config에 평문 저장
- ⚠️ **Salt 사용**: PBKDF2 사용 시 사용자별 고유 Salt 생성
- ⚠️ **HTTPS 통신**: 네트워크 전송 시 암호화
- ⚠️ **세션 관리**: 로그인 세션 타임아웃 구현

## 📝 개발 히스토리

### v1.0 - MVVM 리팩토링 완료
- ✅ MVVM 패턴 전면 적용
- ✅ `UserFormModel` 도입으로 뷰모델 간소화
- ✅ 인라인 에러 메시지 구현
- ✅ 조직 중심 네이밍 (ServiceNumber → EmployeeNumber, Unit → Department)
- ✅ 다크 테마 UI 개선
- ✅ 비밀번호 암호화 다중 알고리즘 지원

## 🤝 기여

이 프로젝트는 학습 및 테스트 목적으로 개발되었습니다. 
개선 사항이나 버그 리포트는 이슈로 등록해주세요.

## 📄 라이선스

MIT License

---

**Note**: 이 애플리케이션은 교육 및 테스트 목적으로 제작되었습니다. 
프로덕션 환경에서 사용 시 추가적인 보안 강화가 필요합니다.
