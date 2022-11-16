using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace FlightSeller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            m_user = null;
            InitializeComponent();
            WriteFlights();
            Loaded += delegate
            {
                MinWidth = ActualWidth;
                MinHeight = ActualHeight;
            };
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


        public void WriteFlights()
        {
            SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");
            DataTable dt = Select("select * from dbo.flights");
            if (dt.Rows.Count == 0)
            {
                string Query = "INSERT into flights(code, company, departure_city,  arrival_city, departure_time, arrival_time, charter, economy_price, bussiness_price, sсhedule)  values('A-1', 'DepoStat', 'Минск', 'Москва', '7:00:00', '9:50:00', 'true', 300, 500, 'EVERY_DAY'), ('A-2', 'DepoStat', 'Москва', 'Минск', '18:00:00', '20:50:00', 'true', 400, 1000, 'EVERY_DAY'), ('A-3', 'Belavia', 'Гомель', 'Алания', '16:30:00', '20:00:00', 'true', 150, 400, 'EVERY_DAY'), ('A-4', 'Belavia', 'Алания', 'Гомель', '5:45:00', '9:15:00', 'true', 100, 300, 'EVERY_DAY'), ('A-6', 'CruteRedo', 'Минск', 'Лондон', '7:30:00', '14:00:00', 'true', 600, 1200, 'EVERY_DAY'), ('A-7', 'CruteRedo', 'Лондон', 'Минск', '20:30:00', '3:00:00', 'true', 800, 2000, 'EVERY_DAY'), ('A-8', 'SolfFly', 'Москва', 'Берлин', '13:20:00', '19:00:00', 'true', 550, 1000, 'EVERY_DAY'), ('A-9', 'SolfFly', 'Берлин', 'Москва', '7:20:00', '14:00:00', 'true', 620, 990, 'EVERY_DAY') ";
                SqlCommand com1 = new SqlCommand(Query, sqlConnection);
                SqlDataReader reader1;
                sqlConnection.Open();
                reader1 = com1.ExecuteReader();
                while (reader1.Read())
                { }
                sqlConnection.Close();

            }
        }

        private void mainLogInLabelMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (mainLogInLabel.Foreground == Brushes.Gray)
                return;
            if (mainLogInLabel.Content.Equals("Войти"))
            {
                Login.ShowLogInWindow();

            }
            else
            {
                userNameLabel.Content = "";
                logOutUser();
            }
        }

        public User m_user { get; private set; }
        public UserControl m_curPage { get; set; }

        public void logInUser(User user)
        {
            m_user = user;
            userNameLabel.Cursor = Cursors.Hand;
            userNameLabel.Content = m_user.m_name + " " + m_user.m_surname;
            mainLogInLabel.Content = "Выход";
        }

        public void logOutUser()
        {
            m_user = null;
            userNameLabel.Cursor = Cursors.Arrow;
            mainLogInLabel.Content = "Войти";
        }

        public bool isUserLogIn()
        {
            return m_user != null;
        }

        private void userNameLabelMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = App.Current.MainWindow as MainWindow;
            if (mainWindow.m_user == null)
                return;
            personalAreaPage.Visibility = System.Windows.Visibility.Visible;
        }

        private void goToMainPageMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.mainPage.Visibility = System.Windows.Visibility.Visible;

            mainWindow.buyPage.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.registrationPage.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.searchPage.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.personalAreaPage.Visibility = System.Windows.Visibility.Hidden;
            mainWindow.searchPage.borderInfoFromTo.Visibility = Visibility.Hidden;
            mainWindow.searchPage.borderInfoToFrom.Visibility = Visibility.Hidden;

        }

        private void mainWindowLoaded(object sender, RoutedEventArgs e)
        {
            mainPage.Visibility = System.Windows.Visibility.Visible;
        }
    }
}