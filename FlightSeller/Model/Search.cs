using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSeller
{
    class Search
    {
        public DataTable Select(string sqlQuery)
        {
            DataTable dataTable = new DataTable("dataBase");

            SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = sqlQuery;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }
        public List<Flight> FindFlights(string cityFrom, string cityTo, DateTime flightDate)
        {
            List<Flight> flightList = new List<Flight>();

            SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");
            sqlConnection.Open();

            DataTable fl = Select("SELECT * FROM flights");

            for (int i = 1; i <= fl.Rows.Count; i++)
            {
                Flight currentFlight = GetFlight(i);
                bool isCitiesMatch = currentFlight.m_departureCity == cityTo &&
                                                             currentFlight.m_arrivalCity == cityFrom;

                string x = "EVERY_DAY";
                bool isScheduleMatch = (currentFlight.m_schedule == x ||
                           ConvertToWeekDay(currentFlight.m_schedule) == flightDate.DayOfWeek);

                if (isCitiesMatch && isScheduleMatch)
                {
                    flightList.Add(currentFlight);
                }
            }    
  
            return flightList;
        }

        private Flight GetFlight(int i)
        {
            SqlConnection sqlConnection = new SqlConnection("server=ZUBAREV-NB;Trusted_Connection=Yes;DataBase=Flight;");
            SqlCommand comm1 = new SqlCommand($"SELECT arrival_city FROM dbo.flights WHERE code like'%" + i + "%' ", sqlConnection);
            SqlCommand comm2 = new SqlCommand($"SELECT departure_city FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm3 = new SqlCommand($"SELECT charter FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm4 = new SqlCommand($"SELECT sсhedule FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm5 = new SqlCommand($"SELECT departure_time FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm6 = new SqlCommand($"SELECT arrival_time FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm7 = new SqlCommand($"SELECT code FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm8 = new SqlCommand($"SELECT company FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm9 = new SqlCommand($"SELECT economy_price FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);
            SqlCommand comm10 = new SqlCommand($"SELECT bussiness_price FROM dbo.flights WHERE code like'%{i}%'", sqlConnection);

            sqlConnection.Open();

            string from = (string)comm1.ExecuteScalar();
            string to = (string)comm2.ExecuteScalar();
            bool isCharter = Convert.ToBoolean(comm3.ExecuteScalar());
            string schedule = (string)comm4.ExecuteScalar();
            DateTime departureTime = Convert.ToDateTime(comm5.ExecuteScalar());
            DateTime arrivalTime = Convert.ToDateTime(comm6.ExecuteScalar());
            string code = (string)comm7.ExecuteScalar();
            string company = (string)comm8.ExecuteScalar();
            double economyPrice = Convert.ToDouble(comm9.ExecuteScalar());
            double businessPrice = Convert.ToDouble(comm10.ExecuteScalar());

            sqlConnection.Close();

            return new Flight(code, company, from, to, isCharter, schedule, departureTime, arrivalTime, economyPrice, businessPrice, false, null, new DateTime(), new DateTime());
        }
        private DayOfWeek ConvertToWeekDay(string scheduleStr)
        {
            if (scheduleStr == "EVERY_MONDAY")
                return DayOfWeek.Monday;
            if (scheduleStr == "EVERY_TUESDAY")
                return DayOfWeek.Tuesday;
            if (scheduleStr == "EVERY_WEDNESDAY")
                return DayOfWeek.Wednesday;
            if (scheduleStr == "EVERY_THURSDAY")
                return DayOfWeek.Thursday;
            if (scheduleStr == "EVERY_FRIDAY")
                return DayOfWeek.Friday;
            if (scheduleStr == "EVERY_SATURDAY")
                return DayOfWeek.Saturday;
            else
                return DayOfWeek.Sunday;
        }
    }
}