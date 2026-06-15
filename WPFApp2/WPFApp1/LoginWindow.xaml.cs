using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFApp1
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {   UserService userService;
        private UserService service;

        public LoginWindow()
        {
            InitializeComponent();
            
        }

        //ham xu ly su kien click vao nut login
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            service = new UserService();
            string usename = txtUsername.Text;
            string password = pwdPassword.Password;

            var user = service.Login(usename, password);

            if(user != null) {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

            } else{
                MessageBox.Show("Invalid username or password!",
                                "Error Login",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }


        }
    }
}
