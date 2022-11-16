using System;
using System.Collections.Generic;
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
    /// Interaction logic for mainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            CalendarDateRange past = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1));
            mainDatePickerFrom.BlackoutDates.Add(past);
            mainDatePickerTo.BlackoutDates.Add(past);
        }

        private void mainIsBothWaysUnchecked(object sender, RoutedEventArgs e)
        {
            mainDatePickerTo.IsEnabled = false;
        }

        private void mainIsBothWaysChecked(object sender, RoutedEventArgs e)
        {
            mainDatePickerTo.IsEnabled = true;
        }

        private void MainGoButtonClick(object sender, RoutedEventArgs e)
        {
            if (((mainCityFrom.Text == "") || (mainCityFrom.Text == " ")) || ((mainCityTo.Text == "") || (mainCityTo.Text == " ")))
                MessageBox.Show("Пожалуйста заполните все поля!", "Ууупс!");
            else
            {
                var mainWindow = App.Current.MainWindow;
                var searchPage = (SearchPage)mainWindow.FindName("searchPage");
                searchPage.Visibility = System.Windows.Visibility.Visible;
                this.Visibility = System.Windows.Visibility.Hidden;


                searchPage.GetListViewForFromToFlight().Items.Clear();
                searchPage.GetListViewForToFromFlight().Items.Clear();
                searchPage.firstFlightInfo.Content = "";
                searchPage.firstFlightDepartureTimeInfo.Content = "";
                searchPage.firstFlightArrivalTimeInfo.Content = "";
                searchPage.secondFlightInfo.Content = "";
                searchPage.secondFlightDepartureTimeInfo.Content = "";
                searchPage.secondFlightArrivalTimeInfo.Content = "";

                string cityFrom = mainCityFrom.Text;
                string cityTo = mainCityTo.Text;
                DateTime flightDateFrom = mainDatePickerFrom.SelectedDate.Value;
                DateTime flightDateTo = mainDatePickerTo.SelectedDate.Value;
                bool isBothWayTicket = mainIsBothWays.IsChecked.Value;
                bool isBusiness = mainIsBusiness.IsChecked.Value;

                searchPage.SendUserInfoToSearchPageControls(cityFrom,
                                                             cityTo,
                                                             flightDateFrom,
                                                             flightDateTo,
                                                             isBothWayTicket,
                                                             isBusiness);

                SearchForListViewUtils utils = new SearchForListViewUtils();
                utils.AddFlightsToListView(cityFrom, cityTo, flightDateFrom, isBusiness, searchPage.GetListViewForFromToFlight());

                if (isBothWayTicket == true)
                {
                    utils.AddFlightsToListView(cityTo, cityFrom, flightDateTo, isBusiness, searchPage.GetListViewForToFromFlight());
                }
            }
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb == null)
                return;

            tb.Text = "";
        }

        private void mainDatePickerFromSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalendarDateRange impossibleDateRange = new CalendarDateRange(DateTime.MinValue, mainDatePickerFrom.SelectedDate.Value.AddDays(-1));
            if (mainDatePickerTo != null)
            {
                if (mainDatePickerTo.SelectedDate.Value.CompareTo(mainDatePickerFrom.SelectedDate.Value) < 0)
                {
                    mainDatePickerTo.SelectedDate = new DateTime(mainDatePickerFrom.SelectedDate.Value.Year,
                        mainDatePickerFrom.SelectedDate.Value.Month,
                        mainDatePickerFrom.SelectedDate.Value.Day);
                }
                mainDatePickerTo.BlackoutDates.Add(impossibleDateRange);
            }
        }

        private void mainPageIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow.m_curPage = mainWindow.mainPage;
                mainWindow.buyPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.personalAreaPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.registrationPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.searchPage.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}