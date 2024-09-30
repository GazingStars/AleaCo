using ProjectBlockchain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    /// Логика взаимодействия для OwnerShipPage.xaml
    /// </summary>
    public partial class OwnerShipPage : Page
    {
        Window parentWindow;
        SimpleAccount account;
        Property propertyChain;

        List<SimpleAccount> ExistingAccounts;

        public ObservableCollection<Doc> docs;

        public OwnerShipPage(SimpleAccount account, Property propertyChain, List<SimpleAccount> ExistingAccounts)
        {
            InitializeComponent();
            this.account = account;
            //doc = new Doc();
            docs = new ObservableCollection<Doc>();

            this.propertyChain = propertyChain;

            string PublicKey = account.PublicKey;

            ListTransact.ItemsSource = docs;

            this.ExistingAccounts = ExistingAccounts;

            fillDocs();
            DataContext = account;
        }


        public void fillDocs()
        {
            var PublicKey = account.PublicKey;

            foreach (var item in propertyChain._blocks.Skip(1))
                for (int i = 0; i < item.Data.Count; i++)
                    if (PublicKey == item.Data[i]!.Data.To && PublicKey == item.Data[i]!.Data.From)
                    {
                        Doc doc = new Doc();
                        doc.PublicKey = account.PublicKey;
                        doc.Document = item.Data[i]!.Data.Document;
                        doc.isReceived = false;
                        doc.isRegistered = true;
                        doc.isAvailable = true;
                        docs.Add(doc);
                        Console.WriteLine($"Собственник {PublicKey} зарегистрировал: {item.Data[i]!.Data.Document}");
                    }
                    else if (PublicKey == item.Data[i]!.Data.To)
                    {
                        Doc doc = new Doc();
                        doc.PublicKey = account.PublicKey;
                        doc.Document = item.Data[i]!.Data.Document;
                        doc.isReceived = true;
                        doc.isRegistered = false;
                        doc.isAvailable = true;
                        docs.Add(doc);
                        Console.WriteLine($"Собственник {PublicKey} получил: {item.Data[i]!.Data.Document}");
                    }
                    else if (PublicKey == item.Data[i]!.Data.From)
                    {
                        Doc doc = new Doc();
                        doc.PublicKey = account.PublicKey;
                        doc.Document = item.Data[i]!.Data.Document;
                        doc.isReceived = false;
                        doc.isRegistered = false;
                        doc.isAvailable = false;
                        doc.To = TellMeWhoItIs(item.Data[i]!.Data.To);
                        docs.Add(doc);
                        Console.WriteLine($"Собственник {PublicKey} передал: {item.Data[i]!.Data.Document}, собственнику {item.Data[i]!.Data.To}");
                    }

            foreach (var item in propertyChain._MemPoolOfPB)
                if (PublicKey == item.Data.To && PublicKey == item.Data.From)
                {
                    Doc doc = new Doc();
                    doc.PublicKey = account.PublicKey;
                    doc.Document = item.Data.Document;
                    doc.isReceived = false;
                    doc.isRegistered = true;
                    doc.isAvailable = true;
                    docs.Add(doc);
                    Console.WriteLine($"Собственник {PublicKey} зарегистрировал: {item.Data.Document}");
                }
                else if (PublicKey == item.Data.To && PublicKey != item.Data.From)
                {
                    Doc doc = new Doc();
                    doc.PublicKey = account.PublicKey;
                    doc.Document = item.Data.Document;
                    doc.isReceived = true;
                    doc.isRegistered = false;
                    doc.isAvailable = true;
                    docs.Add(doc);
                    Console.WriteLine($"Собственник {PublicKey} получил: {item.Data.Document}");
                }
                else if (PublicKey == item.Data.From && PublicKey != item.Data.To)
                {
                    Doc doc = new Doc();
                    doc.PublicKey = account.PublicKey;
                    doc.Document = item.Data.Document;
                    doc.isReceived = false;
                    doc.isRegistered = false;
                    doc.isAvailable = false;
                    doc.To = TellMeWhoItIs(item.Data.To);
                    docs.Add(doc);
                    Console.WriteLine($"Собственник {PublicKey} передал: {item.Data.Document}, собственнику {item.Data.To}");
                }
        }

        public string TellMeWhoItIs(string Who)
        {

            if (!ExistingAccounts.Any(x => x.PublicKey == Who))
            {
                var UW = new UnfortuneWindow("Error: Youre trying to sent Coins to the user that not \nexist in system. Please, try again.");
                UW.Owner = Window.GetWindow(this);
                UW.Show();
            }
            
            

            return ExistingAccounts.FirstOrDefault(x => x.PublicKey == Who).NickName ?? "Unknown";
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ReturnToProfPage(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new ProfilePage(account));
        }
    }
}
