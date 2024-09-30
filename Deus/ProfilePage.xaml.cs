using ProjectBlockchain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        Window parentWindow;
        SimpleAccount account;
        TransactionLogic transactionLogic;

        public Blockchain<TransactionBlock> _blocks;
        public Property propertyChain;


        public ProfilePage(SimpleAccount account)
        {
            InitializeComponent();
            var transaction = new TransactionLogic(new RSAEncryptor());

            transactionLogic = new TransactionLogic(new RSAEncryptor());

            PerformLoadFormFile();

            account.Balance = transactionLogic.ReturnBalanceImMem(account.PublicKey);

            propertyChain = new Property(new RSAEncryptor());

            PerformLoadFormFilePropChain();


            this.account = account;
            DataContext = account;
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



        private bool PerformLoadFormFilePropChain()
        {
            bool helpMeOne = false;
            bool helpMeTwo = false;

            if (File.Exists("MemPropChain"))
            {
                using (var fs = File.Open("MemPropChain", FileMode.Open))
                {
                    if (fs.Length != 0)
                    {
                        propertyChain._MemPoolOfPB = JsonSerializer.Deserialize<List<PropertyBlock>>(fs);
                        helpMeOne = true;
                    }
                }
            }
            if (File.Exists("NoMemPropChain"))
            {
                using (var fs = File.Open("NoMemPropChain", FileMode.Open))
                {
                    if (fs.Length != 0)
                    {
                        propertyChain._blocks._Blockchain.blocks = JsonSerializer.Deserialize<List<BasicBlock>>(fs)!;
                        helpMeTwo = true;
                    }
                }
            }

            if (helpMeOne || helpMeTwo)
                return true;
            return false;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                parentWindow = Window.GetWindow(this);
                parentWindow.DragMove();
            }
        }

        private List<string> CreateListIfExistingPublicKeys()
        {
            List<SimpleAccount> lst = new List<SimpleAccount>();
            if (File.Exists("AllSimpleAccounts"))
            {
                using (var fs = File.Open("AllSimpleAccounts", FileMode.Open))
                {
                    lst = JsonSerializer.Deserialize<List<SimpleAccount>>(fs);
                }
            }

            var PClst = lst.Select(x => x.PublicKey).ToList() ?? new List<string>();
            return PClst;
        }

        private List<SimpleAccount> CreateListOfExistingAccount()
        {
            List<SimpleAccount> lst = new List<SimpleAccount>();
            if (File.Exists("AllSimpleAccounts"))
            {
                using (var fs = File.Open("AllSimpleAccounts", FileMode.Open))
                {
                    lst = JsonSerializer.Deserialize<List<SimpleAccount>>(fs);
                    return lst;
                }
            }
            else
            {
                throw new Exception();
            }

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

        private void TagFunction(object sender, RoutedEventArgs e)
        {
            RBTNS.Tag = (sender as RadioButton)?.Tag;
        }

        private void GoToFunctionalPages(object sender, RoutedEventArgs e)
        {
            if (RBTNS.Tag != null)
            {
                var Tag = RBTNS.Tag.ToString();

                switch (Tag)
                {
                    case "RPT":
                        mainFrame.Navigate(new PerformTransactionPage(account, CreateListIfExistingPublicKeys()));
                        break;
                    case "RCH":
                        mainFrame.Navigate(new HistoryOfTransPage(account, transactionLogic));
                        break;
                    case "RSP":
                        mainFrame.Navigate(new PerformSellProp(account, propertyChain, CreateListIfExistingPublicKeys(), CreateListOfExistingAccount()));
                        break;
                    case "RRP":
                        mainFrame.Navigate(new RegisterPropPage(account, propertyChain));
                        break;
                    case "RCO":
                        mainFrame.Navigate(new OwnerShipPage(account, propertyChain, CreateListOfExistingAccount()));
                        break;
                    default:
                        break;
                }
            }
        }

        private void GoToLoginPage(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new Page1());
        }
    }
}
