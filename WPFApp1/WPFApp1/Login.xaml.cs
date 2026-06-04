using System.Windows;
using System.Windows.Controls;

namespace WPFApp1
{
    /// <summary>
    /// Code-behind cho cửa sổ đăng nhập Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        // Tạo đối tượng UserService để xử lý logic đăng nhập
        private readonly UserService _userService;

        public Login()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        /// <summary>
        /// Xử lý sự kiện khi người dùng click nút "Đăng nhập"
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Bước 1: Lấy dữ liệu từ giao diện
            string username = txtUsername.Text.Trim();       // Lấy text từ TextBox username
            string password = pwdPassword.Password;          // Lấy password từ PasswordBox

            // Bước 2: Kiểm tra người dùng không bỏ trống form
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.",
                    "Thiếu thông tin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Bước 3: Gọi AuthenticateUser để xác thực
            var user = _userService.AuthenticateUser(username, password);

            // Bước 4: Xử lý kết quả
            if (user != null)
            {
                // Đăng nhập thành công: mở MainWindow và đóng Login
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();   // Hiển thị cửa sổ chính
                this.Close();        // Đóng cửa sổ đăng nhập
            }
            else
            {
                // Đăng nhập thất bại: thông báo lỗi rõ ràng
                MessageBox.Show(
                    "Tên đăng nhập hoặc mật khẩu không đúng. Vui lòng thử lại.",
                    "Đăng nhập thất bại",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Xóa ô mật khẩu để người dùng nhập lại, giữ nguyên username
                pwdPassword.Clear();
                pwdPassword.Focus();
            }
        }

        /// <summary>
        /// Sự kiện TextChanged của txtUsername (giữ lại để tránh lỗi binding XAML)
        /// </summary>
        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Có thể thêm logic validate real-time ở đây nếu cần
        }
    }
}
