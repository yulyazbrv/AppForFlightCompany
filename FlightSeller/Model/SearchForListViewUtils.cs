using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FlightSeller
{
    class ListViewRow
    {
        public string company { get; set; }
        public string code { get; set; }
        public string departureCity { get; set; }
        public string arrivalCity { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
        public DateTime arrivalTimeWrite { get; set; }
        public double price { get; set; }
        public bool isTransfer { get; set; }
        public string isTransferStr { get; set; }
        public string transferCity { get; set; }
        public DateTime departureTimeTransit { get; set; }
        public DateTime arrivalTimeTransit { get; set; }
    }

    class SearchForListViewUtils
    {
        public void AddFlightsToListView(string cityFrom, string cityTo, DateTime flightDate, bool isBusiness, ListView listView)
        {
            Search flightSearch = new Search();
            List<Flight> flightList = flightSearch.FindFlights(cityFrom, cityTo, flightDate);
            if (flightList.Count() == 0)
            {
                return;
            }

            foreach (Flight flight in flightList)
                AddFlightsToListView(flight, listView, flightDate, isBusiness);
        }

        private void AddFlightsToListView(Flight flight, ListView listView, DateTime flightDate, bool isBusiness)
        {
            flight.m_departureTime = new DateTime(flightDate.Year, flightDate.Month, flightDate.Day, flight.m_departureTime.Hour, flight.m_departureTime.Minute, 0);
            double price = -1;
            if (isBusiness)
                price = flight.m_businessPrice;
            else
                price = flight.m_economyPrice;

            listView.Items.Add(new ListViewRow
            {
                company = flight.m_company,
                code = flight.m_code,
                departureCity = flight.m_departureCity,
                arrivalCity = flight.m_arrivalCity,
                departureTime = flight.m_departureTime,
                arrivalTime = flight.m_arrivalTime,
                arrivalTimeWrite = flight.m_isTranfer ? flight.m_arrivalTimeTransit : flight.m_arrivalTime,
                price = price,
                isTransfer = flight.m_isTranfer,
                isTransferStr = flight.m_isTranfer ? "Да" : "Нет",
                transferCity = flight.m_transferCity,
                departureTimeTransit = flight.m_departureTimeTransit,
                arrivalTimeTransit = flight.m_arrivalTimeTransit
            });
        }
    }
}