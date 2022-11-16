using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
using System.Xml;

namespace FlightSeller
{
    /// <summary>
    /// Interaction logic for BuyPage.xaml
    /// </summary>
    public partial class BuyPage : UserControl
    {
        public List<Ticket> m_tickets { get; private set; }

        public BuyPage()
        {
            InitializeComponent();
            m_tickets = new List<Ticket>();
        }

        public void addTicket(Ticket ticket)
        {
            m_tickets.Add(ticket);
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

       

        private void confirmButtonClick(object sender, RoutedEventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");
            sqlConnection.Open();
            SqlCommand sqlcmd = sqlConnection.CreateCommand();
            
            DataTable dt = Select("SELECT email FROM users");
            sqlConnection.Close();
            foreach (DataRow dr in dt.Rows)
            {
                var cells = dr.ItemArray;
                foreach (object cell in cells)
                {
                    var mainWindow = App.Current.MainWindow as MainWindow;
                    if (cell.ToString() == mainWindow.m_user.m_email)
                    {
                        sqlcmd.CommandText = "INSERT into dbo.tickets (email, code, departure_city, arrival_city, departure_time, arrival_time, seat_type, price) Values (@emaill,@code,@dep_city,@arr_city,@dep_time,@arr_time,@seat,@price)";
                        sqlcmd.Parameters.AddWithValue("@emaill", mainWindow.m_user.m_email.Trim());
                        sqlcmd.Parameters.AddWithValue("@code", "");
                        sqlcmd.Parameters.AddWithValue("@dep_city", "");
                        sqlcmd.Parameters.AddWithValue("@arr_city", "");
                        sqlcmd.Parameters.AddWithValue("@dep_time", DateTime.MinValue);
                        sqlcmd.Parameters.AddWithValue("@arr_time", DateTime.MinValue);
                        sqlcmd.Parameters.AddWithValue("@seat", "");
                        sqlcmd.Parameters.AddWithValue("@price", 0.0m);
                        
                        foreach (Ticket ticket in m_tickets)
                        {
                            try
                            {
                                sqlcmd.Parameters["@code"].Value = ticket.m_code.Trim();
                                sqlcmd.Parameters["@dep_city"].Value = ticket.m_departureCity.Trim();
                                sqlcmd.Parameters["@arr_city"].Value = ticket.m_arrivalCity.Trim();
                                sqlcmd.Parameters["@dep_time"].Value = ticket.m_departureTime;
                                sqlcmd.Parameters["@arr_time"].Value = ticket.m_arrivalTime;
                                sqlcmd.Parameters["@seat"].Value = ticket.m_isBusiness ? "Бизнес класс" : "Эконом класс";
                                sqlcmd.Parameters["@price"].Value = ticket.m_price;

                                sqlConnection.Open();
                                sqlcmd.ExecuteNonQuery();
                                sqlConnection.Close();
                            }
                            catch (Exception ee)
                            {
                                MessageBox.Show(ee.Message.ToString());
                            }
                        }
                        SendMessToEmail();
                        m_tickets.Clear();
                        

                        mainWindow.mainPage.Visibility = System.Windows.Visibility.Visible;
                        mainWindow.buyPage.Visibility = System.Windows.Visibility.Hidden;
                        mainWindow.registrationPage.Visibility = System.Windows.Visibility.Hidden;
                        mainWindow.searchPage.Visibility = System.Windows.Visibility.Hidden;
                        mainWindow.personalAreaPage.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
            }

        }

        public void SendMessToEmail()
        {
            
            var mainwindow = App.Current.MainWindow as MainWindow;
            string toMail = mainwindow.m_user.m_email;
            string fromMail = "zzzFlights@gmail.com";
            try
            {
                MailAddress from = new MailAddress(fromMail, "ZZZFlights Company");
                MailAddress to = new MailAddress(toMail);
               
                foreach (Ticket ticket in m_tickets)
                {
                    MailMessage m = new MailMessage(from, to);
                    m.Subject = "Забронирован билет";
                    m.Body = "Добрый день, " + mainwindow.m_user.m_name + "\n" + "Билет \n" + "Вылет из города: " + ticket.m_departureCity + "\n" + "Прилёт в город: " + ticket.m_arrivalCity + "\n" + "Время вылета: " + ticket.m_departureTime + "\n" + "Цена: " + ticket.m_price + "$" + "\n" + "Спасибо что доверились нашей комании ZZZFlights!";
                    m.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential("zzzFlights@gmail.com", "juliazzz703");
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        smtp.Send(m);
                    }
                    
                }
                MessageBox.Show("Билет успешно забронирован и выслан вам на почту!", "Успех!");

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
                
        }

        private void buyPageIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow.m_curPage = mainWindow.buyPage;
                mainWindow.mainLogInLabel.Cursor = Cursors.Arrow;
                mainWindow.mainLogInLabel.Foreground = Brushes.Gray;
                mainWindow.mainPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.personalAreaPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.registrationPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.searchPage.Visibility = System.Windows.Visibility.Hidden;
                firstFlightInfo.Content = m_tickets[0].m_departureCity + "->" + m_tickets[0].m_arrivalCity + ": " + m_tickets[0].m_departureTime;
                if (m_tickets.Count == 2)
                {
                    secondFlightInfo.Content = m_tickets[1].m_departureCity + "->" + m_tickets[1].m_arrivalCity + ": " + m_tickets[1].m_departureTime;
                }
                else
                    secondFlightInfo.Content = "";

                double price = 0.0;
                foreach (Ticket ticket in m_tickets)
                {
                    price += ticket.m_price;
                }

                summaryInfo.Content = price.ToString() + " рублей";

                nameInfo.Content = "Пассажир: " + mainWindow.m_user.m_name + "   " + mainWindow.m_user.m_surname;
                string passportNo = mainWindow.m_user.m_passportNo;
                String valDate = mainWindow.m_user.m_validityDate.ToString();
                passportInfo.Content = "Номер паспорта: " + Convert.ToString(passportNo) + ".  Действителен до: " + valDate;
            }
            else
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow.mainLogInLabel.Cursor = Cursors.Hand;
                mainWindow.mainLogInLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xFF, 0x41, 0x1B, 0xEC));
            }
        }
    }
}