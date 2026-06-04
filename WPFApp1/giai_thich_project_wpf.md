# 📚 Giải Thích Project WPF C# — Dành Cho Sinh Viên Mới Học

---

## MỤC LỤC
1. [Tổng quan kiến trúc (3-Layer)](#1-tổng-quan-kiến-trúc)
2. [Vai trò từng file](#2-vai-trò-từng-file)
3. [Luồng chạy từ lúc bấm Run](#3-luồng-chạy-từ-lúc-bấm-run)
4. [Luồng đăng nhập từng bước](#4-luồng-đăng-nhập-từng-bước)
5. [Sơ đồ luồng hoạt động](#5-sơ-đồ-luồng-hoạt-động)
6. [Lỗi thường gặp trong AuthenticateUser](#6-lỗi-thường-gặp)
7. [Vấn đề trùng UserService.cs](#7-vấn-đề-trùng-userservicecs)
8. [Cấu trúc project chuẩn (3-Layer)](#8-cấu-trúc-project-chuẩn)
9. [Code mẫu chuẩn với giải thích](#9-code-mẫu-chuẩn)

---

## 1. Tổng Quan Kiến Trúc

Project này được chia thành **3 tầng** (3-Layer Architecture). Đây là mô hình rất phổ biến trong phát triển phần mềm.

> **Hãy tưởng tượng một nhà hàng:**
> - **Khách hàng** (UI) → gọi món với **nhân viên phục vụ** (Service) → nhân viên vào **nhà bếp** (Repository) → nhà bếp lấy nguyên liệu từ **kho** (Database)

```
┌─────────────────────────────────────────────────────────┐
│           TẦNG 1: UI / Presentation Layer               │
│                  Project: WPFApp1                       │
│   Login.xaml  │  Login.xaml.cs  │  MainWindow.xaml      │
│   App.xaml    │  App.xaml.cs    │  MainWindow.xaml.cs   │
│                                                         │
│   👉 Nhiệm vụ: Hiển thị giao diện, lắng nghe click     │
└───────────────────────┬─────────────────────────────────┘
                        │ gọi xuống
┌───────────────────────▼─────────────────────────────────┐
│           TẦNG 2: Service / Business Logic Layer        │
│                  Project: Service                       │
│                   UserService.cs                        │
│                                                         │
│   👉 Nhiệm vụ: Kiểm tra logic (password đủ dài chưa?)  │
└───────────────────────┬─────────────────────────────────┘
                        │ gọi xuống
┌───────────────────────▼─────────────────────────────────┐
│          TẦNG 3: Repository / Data Access Layer         │
│                  Project: Repository                    │
│   UserRepository.cs  │  Fpf1LoginContext.cs             │
│   Account.cs         │  appsettings.json                │
│                                                         │
│   👉 Nhiệm vụ: Truy vấn database, đọc ghi dữ liệu      │
└───────────────────────┬─────────────────────────────────┘
                        │ kết nối tới
┌───────────────────────▼─────────────────────────────────┐
│                    DATABASE                             │
│              SQL Server — FPF1_Login                    │
│                   Bảng: Account                         │
└─────────────────────────────────────────────────────────┘
```

**Quy tắc quan trọng:** Tầng trên chỉ được gọi xuống tầng dưới liền kề. UI **không được** gọi thẳng vào Repository, phải đi qua Service!

---

## 2. Vai Trò Từng File

### 📁 Project WPFApp1 (UI Layer)

---

#### `App.xaml` — File khởi động ứng dụng
```xml
<Application StartupUri="Login.xaml">
```
> **Hiểu đơn giản:** Đây là file cấu hình nói với Windows: *"Khi chạy app này, hãy mở cửa sổ Login.xaml đầu tiên"*. Giống như trang bìa của một cuốn sách.

---

#### `App.xaml.cs` — Code khởi động
```csharp
public partial class App : Application { }
```
> **Hiểu đơn giản:** File này thường rỗng trong project nhỏ. Nó là nơi để viết code chạy khi app vừa mở (ví dụ: kiểm tra license, load cấu hình...).

---

#### `Login.xaml` — Giao diện màn hình đăng nhập
```xml
<TextBox x:Name="txtUsername" />
<PasswordBox x:Name="pwdPassword" />
<Button x:Name="btnLogin" Click="Button_Click" />
```
> **Hiểu đơn giản:** File này vẽ ra màn hình mà người dùng nhìn thấy. Mỗi control có tên (`x:Name`) để code C# có thể đọc giá trị.
>
> **Quan trọng:** `x:Name` chính là tên biến trong code-behind. Viết sai tên ở đây → code-behind không tìm thấy → lỗi.

---

#### `Login.xaml.cs` — Xử lý sự kiện đăng nhập
```csharp
private void Button_Click(object sender, RoutedEventArgs e)
{
    string username = txtUsername.Text;      // Đọc từ TextBox
    string password = pwdPassword.Password;  // Đọc từ PasswordBox
    // ... gọi UserService
}
```
> **Hiểu đơn giản:** Đây là "bộ não" của màn hình Login. Khi người dùng click nút, hàm này chạy, đọc dữ liệu từ form, gọi service kiểm tra, rồi quyết định mở MainWindow hay báo lỗi.

---

#### `MainWindow.xaml` — Giao diện màn hình chính
```xml
<TextBlock x:Name="lblWelcome" Text="Welcome!" />
<Button x:Name="btnLogout" Click="btnLogout_Click" />
```
> **Hiểu đơn giản:** Màn hình này xuất hiện SAU KHI đăng nhập thành công. Nó hiển thị lời chào và nút đăng xuất.

---

#### `MainWindow.xaml.cs` — Xử lý sự kiện ở trang chính
```csharp
public MainWindow()
{
    InitializeComponent();
    lblWelcome.Text = $"Welcome, {UserService.UserName}!";
}
```
> **Hiểu đơn giản:** Code này chạy ngay khi MainWindow được tạo ra. Nó lấy tên username đã lưu và hiển thị lên màn hình.

---

#### `UserService.cs` trong WPFApp1 — ⚠️ File "bản nháp" chưa hoàn thiện
```csharp
internal class UserService
{
    internal object AuthenticateUser(string username, string password)
    {
        throw new NotImplementedException(); // ❌ LỖI! Chưa viết nội dung
    }
}
```
> **Hiểu đơn giản:** File này được tạo ra nhưng **chưa được implement**. Đây là nguyên nhân lỗi chính. Nó không gọi xuống Repository, không kiểm tra database — chỉ ném lỗi.
>
> **Nên làm gì?** Xóa file này đi và dùng `UserService.cs` từ project Service (đã viết đúng).

---

### 📁 Project Service (Service Layer)

#### `UserService.cs` trong Service — ✅ File đúng, nên dùng cái này
```csharp
public class UserService
{
    UserRepository userRepository = new UserRepository();

    public bool AuthenticateUser(string username, string password)
    {
        if (username.Length < 3 || password.Length < 6)
            return false;  // Validate input trước

        var user = userRepository.GetUserByUsername(username, password);
        return user != null;
    }
}
```
> **Hiểu đơn giản:** Đây là "nhân viên phục vụ" thực sự. Nó:
> 1. Kiểm tra input có hợp lệ không (username >= 3 ký tự, password >= 6 ký tự)
> 2. Gọi xuống Repository để kiểm tra trong database
> 3. Trả về `true`/`false`

---

### 📁 Project Repository (Data Access Layer)

#### `UserRepository.cs` — Truy vấn database
```csharp
public Account? GetUserByUsername(string username, string password)
{
    return Context.Accounts
        .FirstOrDefault(u => u.Username == username && u.Password == password);
}
```
> **Hiểu đơn giản:** Đây là "đầu bếp" trực tiếp vào database lấy thông tin. Câu lệnh LINQ `FirstOrDefault(...)` tương đương với SQL:
> ```sql
> SELECT TOP 1 * FROM Account WHERE Username = @username AND Password = @password
> ```
> Nếu tìm thấy → trả về Account object. Không tìm thấy → trả về `null`.

---

#### `Account.cs` — Model/Entity (ánh xạ bảng database)
```csharp
public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
```
> **Hiểu đơn giản:** File này mô tả "hình dạng" của 1 tài khoản trong database. Mỗi property tương ứng với 1 cột trong bảng `Account`.
>
> | C# Property | Cột SQL |
> |-------------|---------|
> | `Id` | `Id INT` |
> | `Username` | `Username NVARCHAR(50)` |
> | `Password` | `Password NVARCHAR(100)` |

---

#### `Fpf1LoginContext.cs` — Cầu nối C# ↔ Database
```csharp
public class Fpf1LoginContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }  // Đại diện cho bảng Account

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Đọc connection string từ appsettings.json
        var connectionString = configuration.GetConnectionString("DBDefault");
        optionsBuilder.UseSqlServer(connectionString);
    }
}
```
> **Hiểu đơn giản:** Đây là "thông dịch viên" giữa C# và SQL Server. Khi bạn viết `Context.Accounts.FirstOrDefault(...)` trong C#, Entity Framework Core tự động dịch thành câu SQL và chạy trong database.

---

#### `appsettings.json` — File cấu hình kết nối database
```json
{
  "ConnectionStrings": {
    "DBDefault": "Data Source=NgocAnh;Initial Catalog=FPF1_Login;User ID=sa;Password=ngocanh1203;..."
  }
}
```
> **Hiểu đơn giản:** File này chứa "địa chỉ" của database, giống như địa chỉ nhà. Các thành phần:
>
> | Thành phần | Ý nghĩa |
> |------------|---------|
> | `Data Source=NgocAnh` | Tên máy tính / SQL Server instance |
> | `Initial Catalog=FPF1_Login` | Tên database |
> | `User ID=sa` | Tên đăng nhập SQL |
> | `Password=ngocanh1203` | Mật khẩu SQL |

> [!WARNING]
> **Không nên** để password trong file này khi deploy lên môi trường thực tế. Với project học tập thì chấp nhận được.

---

## 3. Luồng Chạy Từ Lúc Bấm Run

```
Bấm Run (F5)
    │
    ▼
Chương trình đọc App.xaml
    │  StartupUri="Login.xaml"
    ▼
WPF tạo ra cửa sổ Login (new Login())
    │  InitializeComponent() → vẽ giao diện
    ▼
Màn hình Login hiện ra
    │  Người dùng nhìn thấy form nhập liệu
    ▼
Người dùng gõ Username + Password
    │
    ▼
Người dùng click nút "Đăng nhập"
    │  WPF tự gọi hàm Button_Click()
    ▼
Button_Click() trong Login.xaml.cs chạy
    │
    ▼
... (xem chi tiết ở phần 4)
```

**Vì sao màn hình Login được mở?**
→ Vì `App.xaml` có dòng `StartupUri="Login.xaml"`. WPF đọc dòng này và tự động tạo + hiển thị cửa sổ Login khi app khởi động.

---

## 4. Luồng Đăng Nhập Từng Bước

### Bước 1️⃣ — Người dùng nhập thông tin
```
[Màn hình Login.xaml]
┌─────────────────────┐
│ Username: [admin   ]│  ← TextBox txtUsername
│ Password: [••••••••]│  ← PasswordBox pwdPassword
│ [   Đăng nhập     ]│  ← Button btnLogin
└─────────────────────┘
```

### Bước 2️⃣ — `Login.xaml.cs` đọc dữ liệu từ form
```csharp
// Đọc text từ TextBox
string username = txtUsername.Text;       // → "admin"

// Đọc password từ PasswordBox (không dùng .Text, dùng .Password)
string password = pwdPassword.Password;  // → "123456"
```
> **Lưu ý:** `PasswordBox` dùng `.Password` (không phải `.Text`) vì lý do bảo mật.

### Bước 3️⃣ — Gọi hàm `AuthenticateUser`
```csharp
// _userService đã được khởi tạo ở constructor
var user = _userService.AuthenticateUser(username, password);
//                      ↑ "admin"          ↑ "123456"
```

### Bước 4️⃣ — Trong `UserService`: kiểm tra input
```csharp
// Service/UserService.cs
public bool AuthenticateUser(string username, string password)
{
    // Kiểm tra cơ bản: không được quá ngắn
    if (username.Length < 3 || password.Length < 6)
        return false;  // Trả về false ngay, không cần hỏi database

    // Nếu OK → tiếp tục gọi xuống Repository
    var user = userRepository.GetUserByUsername(username, password);
    return user != null;
}
```

### Bước 5️⃣ — `UserService` gọi `UserRepository`
```csharp
// UserRepository được tạo trong constructor của UserService
UserRepository userRepository = new UserRepository();

// Gọi hàm GetUserByUsername
var user = userRepository.GetUserByUsername("admin", "123456");
```

### Bước 6️⃣ — `UserRepository` truy vấn database
```csharp
// Repository/UserRepository.cs
public Account? GetUserByUsername(string username, string password)
{
    // Context là Fpf1LoginContext (kết nối database)
    return Context.Accounts
        .FirstOrDefault(u => u.Username == username && u.Password == password);
    // SQL tương đương:
    // SELECT TOP 1 * FROM Account WHERE Username='admin' AND Password='123456'
}
```

### Bước 7️⃣ — Database trả kết quả về
```
Nếu TÌM THẤY:
    Repository → trả về Account { Id=1, Username="admin", Password="123456" }
    Service    → trả về true
    Login.xaml.cs → nhận được true/user != null

Nếu KHÔNG TÌM THẤY:
    Repository → trả về null
    Service    → trả về false
    Login.xaml.cs → nhận được false/user == null
```

### Bước 8️⃣ — Xử lý kết quả và mở MainWindow
```csharp
if (user != null)  // Đăng nhập đúng
{
    MainWindow mainWindow = new MainWindow();  // Tạo cửa sổ chính
    mainWindow.Show();                         // Hiển thị lên
    this.Close();                              // Đóng cửa sổ Login
}
else               // Đăng nhập sai
{
    MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.");
    pwdPassword.Clear();  // Xóa password, để người dùng nhập lại
}
```

### Bước 9️⃣ — `MainWindow` hiển thị "Welcome, admin"
```csharp
// MainWindow.xaml.cs — chạy ngay khi cửa sổ được tạo
public MainWindow()
{
    InitializeComponent();

    // Lấy username đã lưu trong biến static
    string username = UserService.UserName;  // → "admin"

    // Gán vào TextBlock trên giao diện
    lblWelcome.Text = $"Welcome, {username}!";  // → "Welcome, admin!"
}
```

---

## 5. Sơ Đồ Luồng Hoạt Động

```
┌─────────────────────────────────────────────────────────────────┐
│                      Login.xaml (UI)                            │
│  [txtUsername] → "admin"                                        │
│  [pwdPassword] → "123456"                                       │
│  [btnLogin]    → Click                                          │
└────────────────────────┬────────────────────────────────────────┘
                         │ 1. Người dùng nhập + click
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Login.xaml.cs (Code-Behind)                   │
│  Button_Click() được gọi tự động                                │
│  username = txtUsername.Text        → "admin"                   │
│  password = pwdPassword.Password    → "123456"                  │
│  _userService.AuthenticateUser(username, password)              │
└────────────────────────┬────────────────────────────────────────┘
                         │ 2. Gọi Service
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│               Service/UserService.cs (Business Logic)           │
│  AuthenticateUser("admin", "123456")                            │
│  ├─ Validate: username >= 3 ký tự? ✅                           │
│  ├─ Validate: password >= 6 ký tự? ✅                           │
│  └─ userRepository.GetUserByUsername("admin", "123456")         │
└────────────────────────┬────────────────────────────────────────┘
                         │ 3. Gọi Repository
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│             Repository/UserRepository.cs (Data Access)          │
│  GetUserByUsername("admin", "123456")                           │
│  Context.Accounts.FirstOrDefault(u =>                           │
│      u.Username == "admin" && u.Password == "123456")           │
└────────────────────────┬────────────────────────────────────────┘
                         │ 4. Truy vấn EF Core
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│                 Fpf1LoginContext (DbContext)                     │
│  OnConfiguring() đọc appsettings.json                           │
│  UseSqlServer("Data Source=NgocAnh;...")                        │
└────────────────────────┬────────────────────────────────────────┘
                         │ 5. Kết nối SQL Server
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│              Database: SQL Server — FPF1_Login                  │
│  SELECT * FROM Account                                          │
│  WHERE Username='admin' AND Password='123456'                   │
│  → Tìm thấy: Id=1, Username='admin', Password='123456'          │
└────────────────────────┬────────────────────────────────────────┘
                         │ 6. Trả về Account object
         ┌───────────────┘
         │ Kết quả đi ngược lên...
         ▼
Repository → trả Account { Id=1, Username="admin" }
         ▼
Service    → trả true (user != null)
         ▼
Login.xaml.cs → user != null → ĐỦ ĐIỀU KIỆN
         │
         ├─ UserService.UserName = "admin"  (lưu lại)
         ├─ new MainWindow().Show()         (mở trang chính)
         └─ this.Close()                    (đóng Login)
         ▼
┌─────────────────────────────────────────────────────────────────┐
│                   MainWindow.xaml.cs                            │
│  InitializeComponent()                                          │
│  lblWelcome.Text = $"Welcome, {UserService.UserName}!"          │
│                  = "Welcome, admin!"                            │
└─────────────────────────────────────────────────────────────────┘
         ▼
┌─────────────────────────────────────────────────────────────────┐
│                    MainWindow.xaml (UI)                         │
│  Hiển thị: "Welcome, admin! 👋"                                 │
│  Nút: [🔓 Đăng xuất]                                            │
└─────────────────────────────────────────────────────────────────┘
```

---

## 6. Lỗi Thường Gặp

### ❌ Lỗi 1: `throw new NotImplementedException()`
```csharp
// ❌ Code gốc của bạn trong WPFApp1/UserService.cs
internal object AuthenticateUser(string username, string password)
{
    throw new NotImplementedException();  // App crash ngay!
}
```
**Nguyên nhân:** Hàm chưa được viết nội dung.
**Sửa:** Implement đầy đủ hoặc xóa file này, dùng Service/UserService.cs.

---

### ❌ Lỗi 2: `NullReferenceException` — Chưa khởi tạo UserService
```csharp
// ❌ Sai
UserService _userService;  // = null!

private void Button_Click(...)
{
    _userService.AuthenticateUser(...);  // NullReferenceException!
}

// ✅ Đúng — khởi tạo trong constructor
public Login()
{
    InitializeComponent();
    _userService = new UserService();  // Phải có dòng này!
}
```

---

### ❌ Lỗi 3: Sai tên control trong XAML
```xml
<!-- XAML đặt tên là txtUsername -->
<TextBox x:Name="txtUsername" />
```
```csharp
// ❌ Gõ sai tên → lỗi compile
string username = txtUser.Text;    // "txtUser" không tồn tại!

// ✅ Phải dùng đúng tên
string username = txtUsername.Text;
```

---

### ❌ Lỗi 4: Dùng `.Text` cho PasswordBox
```csharp
// ❌ PasswordBox không có .Text
string password = pwdPassword.Text;  // Lỗi compile!

// ✅ PasswordBox dùng .Password
string password = pwdPassword.Password;
```

---

### ❌ Lỗi 5: Không có `x:Name` trên Label ở MainWindow
```xml
<!-- ❌ Không có tên → không thể truy cập từ code -->
<Label Content="Hello, UserName"></Label>

<!-- ✅ Đặt tên để code-behind dùng được -->
<TextBlock x:Name="lblWelcome" Text="Welcome!" />
```
```csharp
lblWelcome.Text = "Welcome, admin!";  // ✅ Hoạt động
```

---

### ❌ Lỗi 6: Không đóng cửa sổ Login
```csharp
// ❌ Quên this.Close() → cả 2 cửa sổ cùng mở!
MainWindow main = new MainWindow();
main.Show();
// this.Close();  ← quên dòng này

// ✅ Luôn đóng Login sau khi mở MainWindow
MainWindow main = new MainWindow();
main.Show();
this.Close();
```

---

### ❌ Lỗi 7: Sai connection string
```json
// ❌ Sai tên server
"Data Source=WrongServerName;..."

// ❌ Sai tên database
"Initial Catalog=WrongDB;..."
```
**Cách debug:** Chạy SQL Server Management Studio, kiểm tra tên server bằng:
```sql
SELECT @@SERVERNAME AS ServerName;
SELECT name FROM sys.databases WHERE name = 'FPF1_Login';
```

---

### ❌ Lỗi 8: DbContext không đọc được `appsettings.json`
```csharp
// Nguyên nhân: appsettings.json không được copy ra thư mục bin/
// Sửa: Trong Repository.csproj, đảm bảo có:
```
```xml
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

---

### ❌ Lỗi 9: Không reference đúng project
```
WPFApp1 cần tham chiếu đến Repository project.
Nếu thiếu → không tìm thấy UserRepository, Account, Fpf1LoginContext.
```
Trong `WPFApp1.csproj`:
```xml
<ItemGroup>
  <ProjectReference Include="..\Repository\Repository.csproj" />
  <!-- Và nếu dùng Service project: -->
  <ProjectReference Include="..\Service\Service.csproj" />
</ItemGroup>
```

---

## 7. Vấn Đề Trùng `UserService.cs`

Hiện tại bạn có **2 file UserService.cs**:

| File | Namespace | Tình trạng |
|------|-----------|-----------|
| `WPFApp1/UserService.cs` | `WPFApp1` | ❌ Chưa implement, chỉ throw NotImplementedException |
| `Service/UserService.cs` | `Service` | ✅ Đã implement đúng, gọi được UserRepository |

**Nên làm gì?**

> **Khuyến nghị:** Xóa `WPFApp1/UserService.cs`, chỉ dùng `Service/UserService.cs`.

**Tại sao không nên có 2?**
- Gây nhầm lẫn: code gọi cái nào? 
- Namespace khác nhau (`WPFApp1` vs `Service`) → dễ dùng sai
- Vi phạm nguyên tắc DRY (Don't Repeat Yourself)

**Cách sửa:**
1. Xóa `WPFApp1/UserService.cs`
2. Thêm project reference `Service` vào `WPFApp1.csproj`:
   ```xml
   <ProjectReference Include="..\Service\Service.csproj" />
   ```
3. Thêm `using Service;` ở đầu `Login.xaml.cs`

---

## 8. Cấu Trúc Project Chuẩn (3-Layer)

```
WPFApp1Solution/
│
├── WPFApp1/                    ← UI Layer (chỉ chứa giao diện)
│   ├── App.xaml
│   ├── App.xaml.cs
│   ├── Login.xaml
│   ├── Login.xaml.cs
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs
│   └── WPFApp1.csproj          ← Reference đến Service project
│       └── <ProjectReference Include="..\Service\Service.csproj" />
│
├── Service/                    ← Business Logic Layer
│   ├── UserService.cs          ← Logic đăng nhập, validate input
│   └── Service.csproj          ← Reference đến Repository project
│       └── <ProjectReference Include="..\Repository\Repository.csproj" />
│
└── Repository/                 ← Data Access Layer
    ├── UserRepository.cs       ← Truy vấn database
    ├── appsettings.json        ← Connection string
    └── Entities/
        ├── Account.cs          ← Model ánh xạ bảng Account
        └── Fpf1LoginContext.cs ← DbContext kết nối database
```

**Quy tắc tham chiếu:**
```
WPFApp1 → tham chiếu → Service
Service  → tham chiếu → Repository
Repository → dùng EF Core → SQL Server
```
**WPFApp1 KHÔNG tham chiếu trực tiếp đến Repository** (trừ trường hợp cần dùng Entity class `Account` để lấy thông tin).

---

## 9. Code Mẫu Chuẩn

### `Login.xaml.cs` — Hoàn chỉnh và có giải thích

```csharp
using System.Windows;
using System.Windows.Controls;
using Service; // Dùng UserService từ project Service

namespace WPFApp1
{
    public partial class Login : Window
    {
        // Khai báo biến UserService ở đây để dùng trong cả class
        private readonly UserService _userService;

        // Constructor: chạy đầu tiên khi cửa sổ Login được tạo
        public Login()
        {
            InitializeComponent();  // Vẽ giao diện từ Login.xaml

            // Khởi tạo UserService — PHẢI có dòng này!
            _userService = new UserService();
        }

        // Hàm này tự động được gọi khi người dùng click nút "Đăng nhập"
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // BƯỚC 1: Lấy dữ liệu từ giao diện
            string username = txtUsername.Text.Trim();  // Trim() xóa khoảng trắng thừa
            string password = pwdPassword.Password;

            // BƯỚC 2: Kiểm tra người dùng có nhập đủ chưa
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Cảnh báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Dừng lại, không xử lý tiếp
            }

            // BƯỚC 3: Gọi Service để kiểm tra đăng nhập
            bool isSuccess = _userService.AuthenticateUser(username, password);

            // BƯỚC 4: Xử lý kết quả
            if (isSuccess)
            {
                // Đăng nhập thành công!
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();  // Mở cửa sổ chính
                this.Close();       // Đóng cửa sổ đăng nhập
            }
            else
            {
                // Đăng nhập thất bại
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng.", "Đăng nhập thất bại",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                pwdPassword.Clear();   // Xóa ô password
                pwdPassword.Focus();   // Di chuyển con trỏ vào ô password
            }
        }

        // Giữ lại event handler này vì XAML đã khai báo TextChanged="txtUsername_TextChanged"
        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Có thể để trống, hoặc thêm logic validate real-time
        }
    }
}
```

---

### `MainWindow.xaml.cs` — Hoàn chỉnh và có giải thích

```csharp
using System.Windows;
using WPFApp1; // Truy cập UserService.UserName (biến static lưu username)

namespace WPFApp1
{
    public partial class MainWindow : Window
    {
        // Constructor: chạy ngay khi cửa sổ MainWindow được tạo
        public MainWindow()
        {
            InitializeComponent();  // Vẽ giao diện từ MainWindow.xaml

            // Lấy username từ biến static đã được lưu lúc đăng nhập
            string username = UserService.UserName;

            // Hiển thị lời chào trên giao diện
            // lblWelcome phải có x:Name="lblWelcome" trong MainWindow.xaml
            if (!string.IsNullOrEmpty(username))
                lblWelcome.Text = $"Welcome, {username}!";
            else
                lblWelcome.Text = "Welcome!"; // Trường hợp bất thường
        }

        // Hàm này chạy khi người dùng click nút "Đăng xuất"
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Hỏi xác nhận trước khi đăng xuất
            var answer = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer == MessageBoxResult.Yes)
            {
                Login loginWindow = new Login();  // Tạo lại cửa sổ đăng nhập
                loginWindow.Show();               // Mở lên
                this.Close();                     // Đóng MainWindow
            }
        }
    }
}
```

---

### `UserService.cs` — Chuẩn (dùng cái trong project Service)

```csharp
using Repository;           // Dùng UserRepository
using Repository.Entities;  // Dùng Account entity

namespace Service
{
    public class UserService
    {
        // Biến static để lưu username sau đăng nhập thành công
        // Static = dùng chung cho toàn bộ app, không cần truyền qua constructor
        public static string UserName { get; private set; } = string.Empty;

        // Repository để truy vấn database
        private readonly UserRepository _userRepository;

        // Constructor
        public UserService()
        {
            _userRepository = new UserRepository();
        }

        /// <summary>
        /// Kiểm tra đăng nhập.
        /// Trả về true nếu username/password đúng, false nếu sai.
        /// </summary>
        public bool AuthenticateUser(string username, string password)
        {
            // Validate input cơ bản
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            if (username.Length < 3)
                return false;  // Username quá ngắn

            if (password.Length < 6)
                return false;  // Password quá ngắn

            // Gọi Repository để kiểm tra trong database
            Account? user = _userRepository.GetUserByUsername(username, password);

            // Nếu tìm thấy user → lưu username lại, trả về true
            if (user != null)
            {
                UserName = user.Username;  // Lưu vào biến static
                return true;
            }

            return false;  // Không tìm thấy → đăng nhập sai
        }
    }
}
```

---

### `UserRepository.cs` — Chuẩn

```csharp
using Repository.Entities;

namespace Repository
{
    public class UserRepository
    {
        // Context là cầu nối đến database (Entity Framework Core)
        private readonly Fpf1LoginContext _context;

        public UserRepository()
        {
            _context = new Fpf1LoginContext();
        }

        /// <summary>
        /// Tìm tài khoản trong database theo username và password.
        /// Dấu ? sau Account nghĩa là có thể trả về null (không tìm thấy).
        /// </summary>
        public Account? GetUserByUsername(string username, string password)
        {
            // FirstOrDefault:
            // - Tìm phần tử ĐẦU TIÊN thỏa điều kiện
            // - Nếu không có → trả về null (Default của object là null)
            return _context.Accounts
                .FirstOrDefault(u => u.Username == username 
                                  && u.Password == password);

            // Câu SQL tương đương:
            // SELECT TOP 1 * FROM Account
            // WHERE Username = 'admin' AND Password = '123456'
        }
    }
}
```

---

## Tóm Tắt Nhanh — "File nào gọi file nào"

```
App.xaml
  └─ Mở Login.xaml (qua StartupUri)

Login.xaml
  └─ Khi click → Login.xaml.cs (Button_Click)

Login.xaml.cs
  └─ Gọi → UserService.AuthenticateUser()

UserService.cs (Service project)
  └─ Gọi → UserRepository.GetUserByUsername()

UserRepository.cs
  └─ Dùng → Fpf1LoginContext (DbContext)
  └─ Đọc → appsettings.json (connection string)
  └─ Kết nối → SQL Server Database

Kết quả trả ngược về:
  Database → UserRepository → UserService → Login.xaml.cs

Login.xaml.cs (kết quả đúng):
  └─ Mở → MainWindow.xaml.cs
  └─ Đóng → Login.xaml

MainWindow.xaml.cs
  └─ Đọc → UserService.UserName (biến static)
  └─ Hiển thị → lblWelcome.Text = "Welcome, admin!"
```

---

*Tài liệu được tạo tự động dựa trên code hiện tại của project WPFApp1 — 04/06/2026*
