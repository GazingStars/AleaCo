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

namespace Deus
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        Window parentWindow;
        MainWindow mainWindow;


        public Page1()
        {
            InitializeComponent();           
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                parentWindow = Window.GetWindow(this);
                parentWindow.DragMove();
            }
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegisterPage());
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            parentWindow = Window.GetWindow(this);
            parentWindow.WindowState = WindowState.Minimized;
        }

        private void ApprovalCheck(object sender, RoutedEventArgs e)
        {
            AccountLogic logic = new AccountLogic(NickNameTextBoxLogIn.Text, PassWordBoxLogIn.Password);

            if (logic.IsImOkay())
            {
                MainFrame.Navigate(new CongratulationsPage("Congratulations, you have successfully logged into your account!", logic.OkayAccount()));
            }
            else
            {
                UnfortuneWindow unfortune = new UnfortuneWindow("Invalid username or password");
                unfortune.Owner = Window.GetWindow(this);
                unfortune.Show();

            }
        }

        private void PublicKeyTextBoxLogIn_EnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApprovalCheck(sender, e);
            }
        }
    }


}

