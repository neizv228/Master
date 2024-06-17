using Master.BDModel;
using Master.StrWindow;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Master
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> userObj;
        private string UserFIO;
        public MainWindow()
        {
            InitializeComponent();
            userObj = KompEntities.GetContext().User.ToList();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            var CurrentUsers = userObj.Where(user => user.UserLogin == log.Text && user.UserPassword == pass.Password).FirstOrDefault();

            if (CurrentUsers != null)
            {
                UserFIO = $"Добро пожаловать, {CurrentUsers.UserSurname} {CurrentUsers.UserName} {CurrentUsers.UserPatronymic}!";
                if (CurrentUsers.UserRole == 1)
                {
                    AdminMaster a = new AdminMaster(UserFIO);
                    a.Show();
                    this.Close();
                }
                else if (CurrentUsers.UserRole == 2)
                {
                    ManagerMaster m = new ManagerMaster(UserFIO);
                    m.Show();
                    this.Close();
                }
                else if (CurrentUsers.UserRole == 3)
                {
                    ClientMaster c = new ClientMaster(UserFIO);
                    c.Show();
                    this.Close();
                }
            }
            else MessageBox.Show("Вы ввели неправильный логин или пароль!", "Внимание!", MessageBoxButton.OK);
        }

        private void GuestEnter_Click(object sender, RoutedEventArgs e)
        {
            ClientMaster a = new ClientMaster(null);
            a.Show();
            this.Close();
        }
    }
}

