using ProjectBlockchain;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //MessageBox.Show("""
            //    Warning!
            //    Данное приложение хотя и находится в относительно функциональном состоянии,
            //    Однако есть ряд вещей, которые не рекомендуется делать при запуске, поскольку
            //    Они не были протестированы и/или в достаточной мере обработаны:
            //    1. Не пытайтесь проходя регистрацию указывать несуществующий E-Mail (ошибка не тестировалась, последствия неизвестны)
            //    2. Вы (пока что) можете создать пустой аккаунт и пользоваться им, в будущем будет испралвено
            //    3. Вы (скорее всего) можете создать много одинаковых аккаунтов, однако данный вариант не тестировался
            //    4. Вы можете пользоваться основными функциями программы (транзакция и трансфер имущества) в полной мере, однако, отрицательные значения транзакций могут обрабатываться некорректно(не тестировалось)
            //    5. Если Вы, укажете не свой PrivateKey, приложение упадет (не обрабатывается)
            //    6. Если Вы будете осуществлять трансфер несуществующего имущества самому себе это (возможно) сработает как эффект его регистрации через отдельное окно
            //    7.Часть элементов интерфейся не обработана, кнопки верхней панели, за исключением функциональных самого окна не работают, однако иконка профиля, в большинстве страниц работает как кнопка возврата в профильное окно
            //    8. Если Вы укажите не свое имущество приложение так же упадет
            //    9. Вы не сможете протестировать функцию E-Mail, так как для активации требуется код аккаунта, который является личным и раскрывать его не хочется никому(функционал будет продемонстрирован на презентации). 
            //    10. В приложении уже существует ряд зарегистрированных аккаунтов, которые можно найти по пути проекта "AllSimpleAccounts" содержит информацию о 3х тестовых аккаунтах, если хочется поиграться с ключами, то
            //    2й Акк PublicKey: AQABoUkw6EPIgqeh3wTm5tkVgQkmeGUzT6cfH5FKz4mBTWCcmbveUVO8l3Uq33r2o49h4SyN/ZymNd8BajUTsQ7hqtgbiSrhOueRGEyUaPw4VOHZUO+giSlbCApZhKx0AS0kd0O5jZAs+6BYUi/WW7qZbhxcIQqxx5yQ7KaeIXzYR00A,
            //    PrivateKey: bbpozqxG/zTi+OLR4N5yNd6u0JwxQ5C0qPplRjsLwEPpZuIcx4HLvdNV/sMeh3pmIhb450ErB12YDdTcd8pjFo6n3q+4c/TN14wwoiooyNQT7kLsZVSJ7+K98k880htIrKC3bB6HNbPPVEi9w16g15EA3IwlWfTzY2TZN62YxlVOL+8NyxEujIDW+bvt6xvF5QjW6w9fMyfzgb/JB6r3ZER7dsNiwI2oAbTYcxLjXg2wwbSKb4bglzrmVjYh9V/7iysDW3eOsDhkbAUEU4CgDYfTaInI883gV7/GwJzllvB3P5MzmAjRpre3wT+vENkaxThxAXmMzSaUaz7fh2G/XwEAAXr+GCeUlqi+rEBPVJxRh2xWiE6ddTQr/qIEhursPrvCjG8/p/nkgv6NHVOzn/NM22rzyPXlOsVNx4+4xzbjfTehSTDoQ8iCp6HfBObm2RWBCSZ4ZTNPpx8fkUrPiYFNYJyZu95RU7yXdSrfevajj2HhLI39nKY13wFqNROxDuGq2BuJKuE655EYTJRo/DhU4dlQ76CJKVsIClmErHQBLSR3Q7mNkCz7oFhSL9ZbupluHFwhCrHHnJDspp4hfNhHTcE505dqZnBTF07dV9pNugWqKyaGpjJHwW7FQHui3QknWx3K/c3SHIWPHub3wlt3R1ZTNLz6CYT6NnD5uFqLbxfVrvx1Tv8nKl2aRowMrEBbNlzh8+GvZVUD5Kpcck4yc/kE18S2z/+dBaiehyd2SzJpo4puuJAhCBmuXfPi29s7AA==
                
            //    3й акк PublicKey: AQAB0TKH54z39LuE2coEijSueX3uelLl+WElzt2Qk628av7oBjfbFNR8lyM8du+Ob44oQSkdwfzcS7KwoYHP55aTMjXWYSJfNWtY1pGis0vQWctx26ezif/wGQz8ou3D7YuAR9duGwFfXjoo0d4kmmeMoQjTQNscoabMekJ+ZohEB5EA,
            //    PrivateKey: XteZ5dBzWfDp+hMsm7vmng4J4EtVGAqDKZYpxcouchO+/ZK1MGxlT+AZuZJB8TFx7pjFdP0+otMQYSiJKWWKtZYdFk4M3n9zfx+bjv4/rOv2VdbB+SVSydFUNosy1n7HimCZsJUV8/6F2hODiz2rqZc+xZNDugExqb4j6eW880HS5BEdGJEUMkPCIC/4cmPwSZlNDyfJu/bpazw+G8J88cB2kNEzqWbDIGVv4i9QX2q8G9H0lNxm8nEFF8IJ5x73oiTiZrRXlK65qLo5r55oOnT/GYYVISL1VflgIFoJyuFFbR3Fg6XvhIYrN7Jx+map5yG8eCQqpUgmv886IJhKVQEAAUEAaC/2IcS6b9VRsmyANNEmEQi0grW30HHmPWXjYCWE1ZJ94yFS8Ol2B1J0suereA/wwObX83i4UiGSRKCbAAHRMofnjPf0u4TZygSKNK55fe56UuX5YSXO3ZCTrbxq/ugGN9sU1HyXIzx2745vjihBKR3B/NxLsrChgc/nlpMyNdZhIl81a1jWkaKzS9BZy3Hbp7OJ//AZDPyi7cPti4BH124bAV9eOijR3iSaZ4yhCNNA2xyhpsx6Qn5miEQHkf2Hh60iOGIa1R0Zv5V4IQjXHqm5LfkO5z6mQQbJIWymxPWRIJ2bxYJ5IU9KF/1TpRWGL+3GLbpyjY/kdXaJOrPTPGhRyB2aHMtjfI6UKuiQrBe6Fcs244unM/z/BhZGBJTfEi48AUc0ozVfCarL5093bwGbgi1MVj7/9YwTL+arAA==
            //    """);
            InitializeComponent();          
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Close_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Go_To_Page1LogIn(object sender, RoutedEventArgs e)
        {
            //SuccessWindow successWindow = new SuccessWindow();
            //successWindow.Owner = this;
            //successWindow.Show();
            MainFrame.Navigate(new Page1());
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegisterPage());
        }

        
    }
}