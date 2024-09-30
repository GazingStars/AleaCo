using ProjectBlockchain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для PerformTransactionPage.xaml
    /// </summary>
    public partial class PerformTransactionPage : Page
    {
        Window parentWindow;
        SimpleAccount account;
        TransactionLogic transactionLogic;
        List<string> ExistingPublicKeys;

        public PerformTransactionPage(SimpleAccount account, List<string> ExistingPublicKeys)
        {
            InitializeComponent();
            this.account = account;

            transactionLogic = new TransactionLogic(new RSAEncryptor());

            PerformLoadFormFile();

            account.Balance = transactionLogic.ReturnBalanceImMem(account.PublicKey);

            this.ExistingPublicKeys = ExistingPublicKeys;

            DataContext = account;
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            parentWindow = Window.GetWindow(this);
            parentWindow.Close();
        }

        private void PerformWriteInFile()
        {
            using (var fs = File.Open("MemChain", FileMode.Create))
            {
                JsonSerializer.Serialize(fs, transactionLogic._MemPoolOfTB);
            }
            using (var fs = File.Open("NoMemChain", FileMode.Create))
            {
                JsonSerializer.Serialize(fs, transactionLogic._blocks._Blockchain.blocks);
            }
        }
        private bool PerformLoadFormFile()
        {
            bool helpMeOne = false;
            bool helpMeTwo = false;

            if (File.Exists("MemChain"))
            {
                using (var fs = File.Open("MemChain", FileMode.Open))
                {
                    transactionLogic._MemPoolOfTB = JsonSerializer.Deserialize<List<TransactionBlock>>(fs)!;
                    helpMeOne = true;
                }
            }
            if (File.Exists("NoMemChain"))
            {
                using (var fs = File.Open("NoMemChain", FileMode.Open))
                {
                    transactionLogic._blocks._Blockchain.blocks = JsonSerializer.Deserialize<List<BasicBlock>>(fs)!;
                    helpMeTwo = true;
                }
            }

            if (helpMeOne || helpMeTwo)
                return true;
            return false;
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

        private void PerformTract(object sender, RoutedEventArgs e)
        {
            bool BadThing = false;

            var amount = TextBoxAmmountCoTransfer;

            var PublicKey = PublicKeyYouWnatToSendTo;

            var YorPrivate = PrivateKey.Text;

            keyPair pair = new keyPair(account.PublicKey, PrivateKey.Text);

            if (ExistingPublicKeys.Any(x => x == PublicKeyYouWnatToSendTo.Text))
            {
                if (!(PublicKeyYouWnatToSendTo.Text == account.PublicKey))
                {
                    try
                    {
                        transactionLogic.PerformTransactionInMem(pair, PublicKeyYouWnatToSendTo.Text, Convert.ToInt64(TextBoxAmmountCoTransfer.Text));
                    }
                    catch (ArgumentException ex)
                    {
                        BadThing = true;
                        var UW = new UnfortuneWindow("Error: Sorry, but you don't have enough money.");
                        UW.Owner = Window.GetWindow(this);
                        UW.Show();
                    }
                    catch (FormatException ex)
                    {
                        BadThing = true;
                        var UW = new UnfortuneWindow("Error: The format of your private key does not match the required one.");
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
                    catch (Exception ex)
                    {
                        BadThing = true;
                        var UW = new UnfortuneWindow("Error: Uknown ERROR.");
                        UW.Owner = Window.GetWindow(this);
                        UW.Show();
                    }

                    if (!BadThing)
                    {
                        account.Balance = transactionLogic.ReturnBalanceImMem(account.PublicKey);
                        PerformWriteInFile();

                        SuccessWindow successWindow = new SuccessWindow();
                        successWindow.Owner = Window.GetWindow(this);
                        successWindow.Show();
                    }
                }
                else
                {
                    BadThing = true;
                    var UW = new UnfortuneWindow("Error: Youre trying to transfer to yourself. Please, try again.");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }

            }
            else
            {
                BadThing = true;
                var UW = new UnfortuneWindow("Error: Youre trying to sent Coins to the user that not \nexist in system. Please, try again.");
                UW.Owner = Window.GetWindow(this);
                UW.Show();
            }
        }

        private void ReturnToProfile(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage(account));
        }

        private void TextBoxAmmountCoTransfer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); // Разрешаем только цифры
            e.Handled = regex.IsMatch(e.Text);

        }

        private void TextBoxAmmountCoTransfer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformTract(sender, e);
            }
        }
    }
}
