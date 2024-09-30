using ProjectBlockchain;
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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        Window parentWindow;
        public RegisterPage()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

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

        private void RegisterationButtonClick(object sender, RoutedEventArgs e)
        {
            bool correctName = false;
            bool correctPassWord = false;
            bool correctEmail = true;

            var tool = new RSAEncryptor();
            var newUser = tool.GenerateKeys();

            EmailService emailService = new EmailService();

            if (NicknameTextBoxRegister.Text.Count() > 3 && NicknameTextBoxRegister.Text.Count() < 20 && !NicknameTextBoxRegister.Text.Any(x => x == ' '))
            {
                correctName = true;
            }
            if (PassWordTextBoxRegister.Text.Count() > 5)
            {
                correctPassWord = true;
            }

            if (correctName && correctPassWord)
            {
                AccountLogic logic = new AccountLogic(NicknameTextBoxRegister.Text, PassWordTextBoxRegister.Text, newUser.PublicKey);

                try
                {
                    emailService.SendAccessCodeEmail(EmailTextBoxRegister.Text, "Hello, here is your data required for making transactions.\n\n" + $"PublicKey: {newUser.PublicKey}, \n PrivateKey: {newUser.PrivateKey}");
                }
                catch (FormatException ex)
                {
                    correctEmail = false;
                    var UW = new UnfortuneWindow("Error: The email was entered incorrectly.");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }

                if (correctEmail)
                {
                    logic.AddAccount();
                    logic.LoadKeyPairToFile(newUser.PublicKey, newUser.PrivateKey);

                    MainFrame.Navigate(new CongratulationsPage($"Congratulations, you have successfully registered a new account!\n\n                 Your KeyPair were sent on your Email adress.", logic.OkayAccount()));
                }
            }
            else if (!correctPassWord && !correctName)
            {
                var UW = new UnfortuneWindow("Error: The login must be between 3 and 20 characters \nlong and must not contain spaces." +
                    "\nError: The PassWord must be between 5 and 15 \ncharacters long.");
                UW.Owner = Window.GetWindow(this);
                UW.Show();
            }
            else if (!correctPassWord && correctName)
            {
                var UW = new UnfortuneWindow("Error: The PassWord must be between 5 and 15 characters long.");
                UW.Owner = Window.GetWindow(this);
                UW.Show();
            }
            else if (correctPassWord && !correctName)
            {
                var UW = new UnfortuneWindow("Error: The login must be between 3 and 20 characters \nlong and must not contain spaces.");
                UW.Owner = Window.GetWindow(this);
                UW.Show();
            }

        }

        private void PublicKeyTextBoxLogIn_EnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterationButtonClick(sender, e);
            }
        }

        private void GoToLogIn(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Page1());
        }
    }
}
