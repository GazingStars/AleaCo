using ProjectBlockchain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для PerformSellProp.xaml
    /// </summary>
    public partial class PerformSellProp : Page
    {
        Window parentWindow;
        SimpleAccount account;
        Property propertyChain;
        OwnerShipPage page;
        ObservableCollection<Doc> AvailableProp;
        List<string> ExistingPublicKeys;
        List<SimpleAccount> ExistingAccounts;


        public PerformSellProp(SimpleAccount account, Property propertyChain, List<string> ExistingPublicKeys, List<SimpleAccount> existingAccounts)
        {
            InitializeComponent();
            var transaction = new TransactionLogic(new RSAEncryptor());
            account.Balance = transaction.ReturnBalanceImMem(account.PublicKey);
            this.propertyChain = propertyChain;
            this.account = account;

            this.ExistingPublicKeys = ExistingPublicKeys;



            page = new OwnerShipPage(account, propertyChain, existingAccounts);


            DataContext = account;
            ExistingAccounts = existingAccounts;

            IsImAvailable();
        }

        private void IsImAvailable()
        {
            AvailableProp = new ObservableCollection<Doc>();

            page = new OwnerShipPage(account, propertyChain, ExistingAccounts);

            foreach (var item in page.docs)
            {
                bool isReallyAvailable = true;
                foreach (var item2 in page.docs)
                {
                    if (item.Document == item2.Document && !(item2.isAvailable))
                    {
                        isReallyAvailable = false;
                    }

                }

                if (isReallyAvailable)
                {
                    AvailableProp.Add(item);
                }

            }

            ListTransact.ItemsSource = AvailableProp;
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

        private void GoToCongratulationsPage(object sender, RoutedEventArgs e)
        {

        }

        private void PublicKeyTextBoxLogIn_EnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GoToCongratulationsPage(sender, e);
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

        private void PerfornTrans(object sender, RoutedEventArgs e)
        {
            if (TranProp.Text.Count() > 0)
            {
                bool BadThing = false;
                keyPair keyPair = new keyPair(account.PublicKey, PrivateKey.Text);

                if (ExistingPublicKeys.Any(x => x == PublicKeyYouWnatToSendTo.Text))
                {
                    if (!(PublicKeyYouWnatToSendTo.Text == account.PublicKey))
                    {
                        try
                        {
                            propertyChain.PerformSellPropertyInMem(TranProp.Text, keyPair, PublicKeyYouWnatToSendTo.Text);
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
                        catch (Exception ex)
                        {
                            BadThing = true;
                            var UW = new UnfortuneWindow("Error: Uknown ERROR.");
                            UW.Owner = Window.GetWindow(this);
                            UW.Show();
                        }

                        if (!BadThing)
                        {
                            LoadInFileProp();
                            IsImAvailable();
                        }
                    }
                    else
                    {
                        BadThing = true;
                        var UW = new UnfortuneWindow("Error: Youre trying to sell to yourself. Please, try again.");
                        UW.Owner = Window.GetWindow(this);
                        UW.Show();
                    }
                    
                }
                else
                {
                    BadThing = true;
                    var UW = new UnfortuneWindow("Error: Youre trying to sell to the user that not \nexist in system. Please, try again.");
                    UW.Owner = Window.GetWindow(this);
                    UW.Show();
                }

            }
            else
            {
                var UW = new UnfortuneWindow("Error: You have to select your property.");
                UW.Owner = Window.GetWindow(this);
                UW.Show();
            }
        }

        private void ProfReturn(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new ProfilePage(account));
        }

        private void ListTransact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TranProp.Text = (ListTransact.SelectedValue as Doc)?.Document ?? string.Empty;
        }
    }
}
