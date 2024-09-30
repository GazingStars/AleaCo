using BlockChain;
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
    /// Логика взаимодействия для HistoryOfTransPage.xaml
    /// </summary>
    public partial class HistoryOfTransPage : Page
    {
        Window parentWindow;
        SimpleAccount account;
        TransactionLogic transactionLogic;


        List<ListBoxData> listBoxDatas;

        public List<TransactionBlock> ShowCase;

        public HistoryOfTransPage(SimpleAccount account, TransactionLogic transactionLogic)
        {
            InitializeComponent();
            this.account = account;
            this.transactionLogic = transactionLogic;

            listBoxDatas = new List<ListBoxData>();
            ShowCase = new List<TransactionBlock>();

            account.Balance = transactionLogic.ReturnBalanceImMem(account.PublicKey);

            DataContext = account;

            var NoMemList = transactionLogic._blocks.ToList();

            foreach (var item in NoMemList)
            {
                if (item.Data != null)
                {
                    ShowCase?.AddRange(item.Data);
                }
            }

            ShowCase?.AddRange(transactionLogic._MemPoolOfTB);

            using (var fs = File.Open("AllSimpleAccounts", FileMode.Open))
            {
                var FromFile = JsonSerializer.Deserialize<List<SimpleAccount>>(fs);


                if (ShowCase?.Count > 0 && FromFile != null)
                {
                    foreach (var DTA in ShowCase)
                    {
                        foreach (var SimAcc in FromFile)
                        {
                            if (DTA.Data.To == SimAcc.PublicKey && DTA.Data.To != account.PublicKey)
                            {
                                listBoxDatas.Add(new ListBoxData { amount= DTA.Data.Amount, from = account.NickName, to = SimAcc.NickName });
                            }
                            if (DTA.Data.From == SimAcc.PublicKey && DTA.Data.To == account.PublicKey)
                            {

                                listBoxDatas.Add(new ListBoxData { amount = DTA.Data.Amount, from = SimAcc.NickName, to = account.NickName });
                            }

                        }
                    }
                }
            }
            //ListTransact.ItemsSource = this.transactionLogic._blocks.Skip(1);
            ListTransact.ItemsSource = listBoxDatas;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private void GoProfile(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProfilePage(account));
        }
    }
}
