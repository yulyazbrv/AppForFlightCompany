using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace FlightSeller
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    /// 
    public partial class Login : Window
    {
        private Login()
        {
            InitializeComponent();
        }
        Guid GetHashString(string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);

            MD5CryptoServiceProvider CSP =
                new MD5CryptoServiceProvider();

            byte[] byteHash = CSP.ComputeHash(bytes);

            string hash = string.Empty;

            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return new Guid(hash);
        }
        public DataTable Select(string sqlQuery)
        {
            DataTable dataTable = new DataTable("dataBase");                // создаём таблицу в приложении 
                                                                            // подключаемся к базе данных 
            SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");
            sqlConnection.Open();                                           // открываем базу данных 
            SqlCommand sqlCommand = sqlConnection.CreateCommand();          // создаём команду 
            sqlCommand.CommandText = sqlQuery;                              // присваиваем команде текст 
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // создаём обработчик 
            sqlDataAdapter.Fill(dataTable);                                 // возращаем таблицу с результатом 
            //Перед return нужно закрыть соединение sqlConnect.Close(); 
            return dataTable;
        }

        public static Login ShowLogInWindow()
        {
            if (instance == null)
            {
                instance = new Login();
            }
            instance.Show();
            return instance;
        }

        private static Login instance;

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = System.Windows.Visibility.Hidden;
        }

        private void LogInButtonClick(object sender, RoutedEventArgs e)
        {
            string emailText = loginField.Text;
            var pass = GetHashString(passwordField.Password.Trim());

            DataTable user = Select(
                    "SELECT * FROM dbo.users WHERE email = '" +
                    emailText + "' AND password = '" + pass.ToString() + "'");

            SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");

            SqlCommand com = new SqlCommand($"SELECT email FROM dbo.users WHERE email = '{emailText}' AND password = '{pass}'", sqlConnection);
            sqlConnection.Open();
            string emailFromBD = (string)com.ExecuteScalar();

            if (user.Rows.Count == 1)
            {
                statusLabel.Content = "";

                SqlCommand comm = new SqlCommand($"SELECT name FROM dbo.users WHERE email = '{emailText}' AND password = '{pass}'", sqlConnection);
                SqlCommand comm1 = new SqlCommand($"SELECT surname FROM dbo.users WHERE email = '{emailText}' AND password = '{pass}'", sqlConnection);
                SqlCommand comm2 = new SqlCommand($"SELECT passport_number FROM dbo.users WHERE email = '{emailText}' AND password = '{pass}'", sqlConnection);
                SqlCommand comm3 = new SqlCommand($"SELECT passport_date FROM dbo.users WHERE email = '{emailText}' AND password = '{pass}'", sqlConnection);

                string userName = (string)comm.ExecuteScalar();
                string userSurname = (string)comm1.ExecuteScalar();

                var mainWindow = App.Current.MainWindow as MainWindow;

                string passportNo = (string)comm2.ExecuteScalar();
                string valDate = (string)comm3.ExecuteScalar();

                mainWindow.logInUser(new User(emailFromBD, userName, userSurname, passportNo, valDate));
                sqlConnection.Close();
                Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            else
            {
                const string status = "Неверный логин или пароль!";
                statusLabel.Content = status;
            }
        }

        private void registrationLabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.searchPage.Visibility = System.Windows.Visibility.Hidden;
            Visibility = System.Windows.Visibility.Hidden;
            mainWindow.registrationPage.Visibility = System.Windows.Visibility.Visible;
        }

        private void LoginBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb == null)
                return;

            tb.Text = "";
        }

        private void PasswordBoxGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox tb = (sender as PasswordBox);
            if (tb == null)
                return;

            tb.Password = "";
        }

        private void loginWindowIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            loginField.Text = "e-mail";
            passwordField.Password = "пароль";
        }
    }
}