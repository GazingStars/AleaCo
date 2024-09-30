using ProjectBlockchain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
    /// Логика взаимодействия для RegisterPropPage.xaml
    /// </summary>
    public partial class RegisterPropPage : Page
    {

        Window parentWindow;
        SimpleAccount account;
        Property propertyChain;


        public RegisterPropPage(SimpleAccount account, Property propertyChain)
        {
            InitializeComponent();
            this.account = account;

            var tool = new RSAEncryptor();

            this.propertyChain = propertyChain;
            DataContext = account;
        }


        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }
        private void ReturnToProfPageClick(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new ProfilePage(account));
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            parentWindow = Window.GetWindow(this);
            parentWindow.WindowState = WindowState.Minimized;
        }


        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                parentWindow = Window.GetWindow(this);
                parentWindow.DragMove();
            }
        }


        private void PublicKeyTextBoxLogIn_EnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterPropClick(sender, e);
            }
        }

        private void LoadInFileProp()
        {

            using (var fs = File.Open("MemPropChain", FileMode.Create))
            {
                JsonSerializer.Serialize(fs, propertyChain._MemPoolOfPB);
            }
            using (var fs = File.Open("NoMemPropChain", FileMode.Create))
            {
                JsonSerializer.Serialize(fs, propertyChain._blocks._Blockchain.blocks);
            }
        }

        private void RegisterPropClick(object sender, RoutedEventArgs e)
        {
            keyPair keyPair = new keyPair(account.PublicKey, PrivateKey.Text);
            bool BadThing = false;

            if (TextBoxPPToRegister.Text.Count() <= 0)
            {
                BadThing = true;
                var UW = new UnfortuneWindow("Error: You have to enter at least something \nto register.");
                UW.Owner = Window.GetWindow(this);
                UW.Show();
            }

            if (!BadThing)
            {
                try
                {
                    propertyChain.RegisterProperty(keyPair, TextBoxPPToRegister.Text);
                }
                catch (FormatException ex)
                {
                    BadThing = true;
                    var UW = new UnfortuneWindow("Error: The format of your private key does not \nmatch the required one.");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }
                catch (CryptographicException ex)
                {
                    BadThing = true;
                    var UW = new UnfortuneWindow("Error: You entered the wrong private key.");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }
                catch (ArgumentException ex)
                {
                    BadThing = true;
                    var UW = new UnfortuneWindow("Error: You entered the wrong private key.");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }
                catch (ApplicationException ex)
                {
                    BadThing = true;
                    var UW = new UnfortuneWindow("You're trying to register the work that is already registred");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }
                catch (Exception ex)
                {
                    BadThing = true;
                    var UW = new UnfortuneWindow("Error: Uknown ERROR.");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }
            }


            if (!BadThing)
            {
                LoadInFileProp();

                SuccessWindow successWindow = new SuccessWindow();
                successWindow.Owner = Window.GetWindow(this);
                successWindow.Show();
            }


        }
    }
}
