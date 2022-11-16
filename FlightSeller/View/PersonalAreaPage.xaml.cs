using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
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
    /// Interaction logic for PersonalAreaPage.xaml
    /// </summary>
    public partial class PersonalAreaPage : UserControl
    {
        private string m_userMail;
        private int m_countOfTickets;
        SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");

        public PersonalAreaPage()
        {
            InitializeComponent();
        }

        private void personalAreaPageIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow.m_curPage = mainWindow.personalAreaPage;
                mainWindow.mainPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.registrationPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.searchPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.buyPage.Visibility = System.Windows.Visibility.Hidden;

                m_userMail = mainWindow.m_user.m_email;
                updateCountOfTickets();
                showTickets(0, Math.Min(15, m_countOfTickets));
            }
        }

        private void updateCountOfTickets()
        {
            SqlCommand comm = new SqlCommand($"SELECT count(*) FROM dbo.tickets WHERE email = '{m_userMail}'", sqlConnection);
            string user = findUserNode();
            if (user != null)
            {
                sqlConnection.Open();
                m_countOfTickets = (int)comm.ExecuteScalar();
                sqlConnection.Close();
            }
            else
            {
                m_countOfTickets = 0;
            }
        }

        public string status;
        private void showTickets(int firstIndex, int count)
        {

            DateTime curDate = DateTime.Now;
            string user = findUserNode();
            if (user != null)
            {
                ticketList.Items.Clear();
                DataTable dt = Select($"SELECT * FROM tickets WHERE email = '{user}'");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow row = dt.Rows[i];
                    if ((DateTime)row["departure_time"] < curDate)
                    {
                        status = "Завершён";
                    }
                    else
                    {
                        status = "Активен";
                    }

                    ticketList.Items.Add(new ListViewRow()
                    {
                        m_departureCity = row["departure_city"].ToString(),
                        m_arrivalCity = row["arrival_city"].ToString(),
                        m_departureTime = Convert.ToDateTime(row["departure_time"]),
                        m_price = Convert.ToDouble(row["price"]),
                        m_seatType = row["seat_type"].ToString(),
                        m_status = status
                    });
                }
            }
                
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
            return dataTable;
        }

        private string findUserNode()
        {
            

            DataTable dt = Select("SELECT email FROM users");
            
            foreach (DataRow dr in dt.Rows)
            {
                var cells = dr.ItemArray;
                foreach (object cell in cells)
                {
                    if (cell.ToString() == m_userMail)
                    {
                        return cell.ToString();
                    }
                }
            }

            return null;
        }

        private class ListViewRow
        {
            public string m_departureCity { get; set; }
            public string m_arrivalCity { get; set; }
            public DateTime m_departureTime { get; set; }
            public double m_price { get; set; }
            public string m_seatType { get; set; }
            public string m_status { get; set; }

        }
    }
}