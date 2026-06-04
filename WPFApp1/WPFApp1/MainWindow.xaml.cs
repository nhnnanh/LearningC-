using System.Windows;

namespace WPFApp1
{
    /// <summary>
    /// Code-behind cho cửa sổ chính MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Lấy username từ biến static UserService.UserName
            // (được lưu tại thời điểm đăng nhập thành công trong Login.xaml.cs)
            string username = UserService.UserName;

            // Hiển thị lời chào đúng định dạng: "Welcome, username"
            // Nếu username trống (trường hợp bất thường), hiện mặc định
            if (!string.IsNullOrEmpty(username))
            {
                lblWelcome.Text = $"Welcome, {username}!";
            }
            else
            {
                lblWelcome.Text = "Welcome!";
            }
        }

        /// <summary>
        /// Xử lý sự kiện click nút Đăng xuất
        /// </summary>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Xác nhận trước khi đăng xuất
            var result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Mở lại cửa sổ đăng nhập
                Login loginWindow = new Login();
                loginWindow.Show();

                // Đóng cửa sổ hiện tại (MainWindow)
                this.Close();
            }
        }
    }
}