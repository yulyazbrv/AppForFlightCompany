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
using System.Web;
using System.Xml;

namespace FlightSeller
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : UserControl
    {
        private bool m_isBothWayTicket;
        private bool m_isBusiness;

        public SearchPage()
        {
            InitializeComponent();
            m_isBothWayTicket = false;
            m_isBusiness = false;

            CalendarDateRange past = new CalendarDateRange(DateTime.MinValue, DateTime.Today.AddDays(-1));
            searchDatePickerFrom.BlackoutDates.Add(past);
            searchDatePickerTo.BlackoutDates.Add(past);
            borderInfoFromTo.Visibility = Visibility.Hidden;
            borderInfoToFrom.Visibility = Visibility.Hidden;
        }

        public void SendUserInfoToSearchPageControls(string cityFrom,
                                                      string cityTo,
                                                      DateTime selectedDateFrom,
                                                      DateTime selectedDateTo,
                                                      bool isBothWayTicket,
                                                      bool isBusiness)

        {
            searchCityFrom.Text = cityFrom;
            searchCityTo.Text = cityTo;
            searchDatePickerFrom.SelectedDate = selectedDateFrom;
            searchDatePickerTo.SelectedDate = selectedDateTo;
            searchIsBothWays.IsChecked = isBothWayTicket;
            m_isBothWayTicket = isBothWayTicket;
            m_isBusiness = isBusiness;

            SearchForListViewUtils utils = new SearchForListViewUtils();
            if (m_isBothWayTicket)
            {
                Grid.SetRowSpan(borderInfoFromTo, 1);
                Grid.SetRowSpan(GetListViewForFromToFlight(), 1);
                GetListViewForToFromFlight().Visibility = System.Windows.Visibility.Visible;
                //utils.AddFlightsToListView(cityTo, cityFrom, selectedDateTo, isBusiness, GetListViewForToFromFlight());
            }
            else
            {
                GetListViewForToFromFlight().Visibility = System.Windows.Visibility.Hidden;
                Grid.SetRowSpan(borderInfoFromTo, 2);
                Grid.SetRowSpan(GetListViewForFromToFlight(), 4);
            }

        }

        private void searchGoButtonClick(object sender, RoutedEventArgs e)
        {
            if (((searchCityFrom.Text == "") || (searchCityFrom.Text == " ")) || ((searchCityTo.Text == "") || (searchCityTo.Text == " ")))
                MessageBox.Show("Пожалуйста заполните все поля!", "Ууупс!");
            else
            {
                //очищаем все поля с информацией
                flightListViewToFrom.Items.Clear();
                flightListViewFromTo.Items.Clear();
                secondFlightInfo.Content = "";
                secondFlightDepartureTimeInfo.Content = "";
                secondFlightArrivalTimeInfo.Content = "";
                firstFlightInfo.Content = "";
                firstFlightDepartureTimeInfo.Content = "";
                firstFlightArrivalTimeInfo.Content = "";
                borderInfoFromTo.Visibility = Visibility.Hidden;
                borderInfoToFrom.Visibility = Visibility.Hidden;

                firstFlightArrivalTimeInfo.Content = "";
                firstFlightTransferInfoLabel.Visibility = Visibility.Hidden;
                firstFlightDepartureTimeInfoTransitLabel.Visibility = Visibility.Hidden;
                firstFlightArrivalTimeInfoTransitLabel.Visibility = Visibility.Hidden;

                firstFlightTransferInfo.Content = "";
                firstFlightDepartureTimeInfoTransit.Content = "";
                firstFlightArrivalTimeInfoTransit.Content = "";

                secondFlightArrivalTimeInfo.Content = "";
                secondFlightTransferInfoLabel.Visibility = Visibility.Hidden;
                secondFlightDepartureTimeInfoTransitLabel.Visibility = Visibility.Hidden;
                secondFlightArrivalTimeInfoTransitLabel.Visibility = Visibility.Hidden;

                secondFlightTransferInfo.Content = "";
                secondFlightDepartureTimeInfoTransit.Content = "";
                secondFlightArrivalTimeInfoTransit.Content = "";

                string cityFrom = searchCityFrom.Text;
                string cityTo = searchCityTo.Text;
                DateTime flightDateFrom = searchDatePickerFrom.SelectedDate.Value;
                DateTime flightDateTo = searchDatePickerTo.SelectedDate.Value;
                bool isBothWayTicket = searchIsBothWays.IsChecked.Value;
                bool isBusiness = searchIsBusiness.IsChecked.Value;

                SearchForListViewUtils utils = new SearchForListViewUtils();
                utils.AddFlightsToListView(cityFrom, cityTo, flightDateFrom, isBusiness, GetListViewForFromToFlight());

                if (isBothWayTicket == true)
                {
                    m_isBothWayTicket = true;
                    GetListViewForToFromFlight().Visibility = System.Windows.Visibility.Visible;
                    Grid.SetRowSpan(GetListViewForFromToFlight(), 1);
                    Grid.SetRowSpan(borderInfoFromTo, 1);
                    utils.AddFlightsToListView(cityTo, cityFrom, flightDateTo, isBusiness, GetListViewForToFromFlight());

                }
                else
                {
                    m_isBothWayTicket = false;

                    GetListViewForToFromFlight().Visibility = System.Windows.Visibility.Hidden;
                    Grid.SetRowSpan(GetListViewForFromToFlight(), 4);
                    Grid.SetRowSpan(borderInfoFromTo, 2);
                }
            }
        }

        private void BuyButtonClick(object sender, RoutedEventArgs e)
        {
            Search search = new Search();

            var mainWindow = App.Current.MainWindow as MainWindow;
            if (!mainWindow.isUserLogIn())
            {
                Login.ShowLogInWindow();
                return;
            }

            var selectedFromToFlight = flightListViewFromTo.SelectedItems[0] as ListViewRow;
            if (selectedFromToFlight == null)
            {
                return;
            }

            string selectedFromToCode = selectedFromToFlight.code;
            Ticket firstTicket;

            if (!selectedFromToFlight.isTransfer)
            {
                firstTicket = new Ticket(selectedFromToCode, selectedFromToFlight.departureCity, selectedFromToFlight.arrivalCity,
                                               selectedFromToFlight.departureTime, selectedFromToFlight.arrivalTime,
                                               m_isBusiness, selectedFromToFlight.price);
            }
            else
            {
                firstTicket = new Ticket(selectedFromToCode, selectedFromToFlight.departureCity, selectedFromToFlight.arrivalCity,
                                               selectedFromToFlight.departureTime, selectedFromToFlight.arrivalTimeTransit,
                                               m_isBusiness, selectedFromToFlight.price);
            }
               
            mainWindow.buyPage.addTicket(firstTicket);

            if (searchIsBothWays.IsChecked.Value == true)
            {
                var selectedToFromFlight = flightListViewToFrom.SelectedItems[0] as ListViewRow;
                if (selectedToFromFlight == null)
                    return;
                string selectedToFromCode = selectedToFromFlight.code;
                Ticket secondTicket;

                if (!selectedToFromFlight.isTransfer)
                    secondTicket = new Ticket(selectedToFromCode, selectedToFromFlight.departureCity, selectedToFromFlight.arrivalCity,
                                                    selectedToFromFlight.departureTime, selectedToFromFlight.arrivalTime,
                                                    m_isBusiness, selectedToFromFlight.price);
                else
                    secondTicket = new Ticket(selectedToFromCode, selectedToFromFlight.departureCity, selectedToFromFlight.arrivalCity,
                                                    selectedToFromFlight.departureTime, selectedToFromFlight.arrivalTimeTransit,
                                                    m_isBusiness, selectedToFromFlight.price);
                mainWindow.buyPage.addTicket(secondTicket);
            }

            Visibility = System.Windows.Visibility.Hidden;
            mainWindow.buyPage.Visibility = System.Windows.Visibility.Visible;
        }

        public ListView GetListViewForFromToFlight()
        {
            return flightListViewFromTo;
        }

        public ListView GetListViewForToFromFlight()
        {
            return flightListViewToFrom;
        }

        private void flightListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isFirstFlightSelected = false;
            bool isSecondFlightSelected = false;
            const string summary = "Итог: ";
            double priceFromTo = 0;
            double priceToFrom = 0;

            var selectedFromToFlight = flightListViewFromTo.SelectedItem as ListViewRow;
            if (selectedFromToFlight != null)
            {
                borderInfoFromTo.Visibility = Visibility.Visible;
                isFirstFlightSelected = true;
                priceFromTo = selectedFromToFlight.price;
                firstFlightInfo.Content = selectedFromToFlight.departureCity + "->" + selectedFromToFlight.arrivalCity;
                firstFlightDepartureTimeInfo.Content = selectedFromToFlight.departureTime.ToString();
                firstFlightArrivalTimeInfo.Content = selectedFromToFlight.arrivalTime.ToString();
                if (selectedFromToFlight.isTransfer)
                {
                    firstFlightTransferInfoLabel.Visibility = Visibility.Visible;
                    firstFlightDepartureTimeInfoTransitLabel.Visibility = Visibility.Visible;
                    firstFlightArrivalTimeInfoTransitLabel.Visibility = Visibility.Visible;

                    firstFlightTransferInfo.Content = selectedFromToFlight.transferCity.ToString();
                    firstFlightDepartureTimeInfoTransit.Content = selectedFromToFlight.departureTimeTransit.ToString();
                    firstFlightArrivalTimeInfoTransit.Content = selectedFromToFlight.arrivalTimeTransit.ToString();
                }
                else
                {
                    firstFlightTransferInfoLabel.Visibility = Visibility.Hidden;
                    firstFlightDepartureTimeInfoTransitLabel.Visibility = Visibility.Hidden;
                    firstFlightArrivalTimeInfoTransitLabel.Visibility = Visibility.Hidden;

                    firstFlightTransferInfo.Content = "";
                    firstFlightDepartureTimeInfoTransit.Content = "";
                    firstFlightArrivalTimeInfoTransit.Content = "";
                }
            }

            var selectedToFromFlight = flightListViewToFrom.SelectedItem as ListViewRow;
            if (selectedToFromFlight != null)
            {
                Grid.SetRowSpan(borderInfoFromTo, 1);
                borderInfoToFrom.Visibility = Visibility.Visible;
                isSecondFlightSelected = true;
                priceToFrom = selectedToFromFlight.price;
                secondFlightInfo.Content = selectedToFromFlight.departureCity + "->" + selectedToFromFlight.arrivalCity;
                secondFlightDepartureTimeInfo.Content = selectedToFromFlight.departureTime.ToString();
                secondFlightArrivalTimeInfo.Content = selectedToFromFlight.arrivalTime.ToString();
                if (selectedToFromFlight.isTransfer)
                {
                    secondFlightTransferInfoLabel.Visibility = Visibility.Visible;
                    secondFlightDepartureTimeInfoTransitLabel.Visibility = Visibility.Visible;
                    secondFlightArrivalTimeInfoTransitLabel.Visibility = Visibility.Visible;

                    secondFlightTransferInfo.Content = selectedToFromFlight.transferCity.ToString();
                    secondFlightDepartureTimeInfoTransit.Content = selectedToFromFlight.departureTimeTransit.ToString();
                    secondFlightArrivalTimeInfoTransit.Content = selectedToFromFlight.arrivalTimeTransit.ToString();
                }
                else
                {
                    secondFlightTransferInfoLabel.Visibility = Visibility.Hidden;
                    secondFlightDepartureTimeInfoTransitLabel.Visibility = Visibility.Hidden;
                    secondFlightArrivalTimeInfoTransitLabel.Visibility = Visibility.Hidden;

                    secondFlightTransferInfo.Content = "";
                    secondFlightDepartureTimeInfoTransit.Content = "";
                    secondFlightArrivalTimeInfoTransit.Content = "";
                }
            }

            double totalPrice = priceFromTo + priceToFrom;
            searchSummaryLabel.Content = summary + totalPrice.ToString() + " $";

            if (isFirstFlightSelected && (!m_isBothWayTicket || isSecondFlightSelected))
            {
                BuyButton.IsEnabled = true;
            }
            else
            {
                BuyButton.IsEnabled = false;
            }
        }

        private void searchIsBothWaysUnchecked(object sender, RoutedEventArgs e)
        {
            searchDatePickerTo.IsEnabled = false;
        }

        private void searchIsBothWaysChecked(object sender, RoutedEventArgs e)
        {
            searchDatePickerTo.IsEnabled = true;
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb == null)
                return;

            tb.Text = "";
        }

        private void searchDatePickerFromSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            CalendarDateRange impossibleDateRange = new CalendarDateRange(DateTime.MinValue, searchDatePickerFrom.SelectedDate.Value.AddDays(-1));
            if (searchDatePickerTo != null)
            {
                if (searchDatePickerTo.SelectedDate.Value.CompareTo(searchDatePickerFrom.SelectedDate.Value) < 0)
                {
                    searchDatePickerTo.SelectedDate = new DateTime(searchDatePickerFrom.SelectedDate.Value.Year,
                        searchDatePickerFrom.SelectedDate.Value.Month,
                        searchDatePickerFrom.SelectedDate.Value.Day);
                }
                searchDatePickerTo.BlackoutDates.Add(impossibleDateRange);
            }
        }

        private void searchPageIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var mainWindow = App.Current.MainWindow as MainWindow;
                mainWindow.m_curPage = mainWindow.searchPage;
                mainWindow.buyPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.mainPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.personalAreaPage.Visibility = System.Windows.Visibility.Hidden;
                mainWindow.registrationPage.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}