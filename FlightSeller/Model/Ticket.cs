using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSeller
{
    public class Ticket
    {
        public string m_code { get; private set; }
        public string m_departureCity { get; private set; }
        public string m_arrivalCity { get; private set; }
        public DateTime m_departureTime { get; private set; }
        public DateTime m_arrivalTime { get; private set; }
        public bool m_isBusiness { get; private set; }
        public double m_price { get; private set; }
        public bool m_status { get; private set; }//

        public Ticket(string code, string departureCity,
                      string arrivalCity,
                      DateTime departureTime,
                      DateTime arrivalTime,
                      bool isBusiness,
                      double price)
        {
            m_code = code;
            m_departureCity = departureCity;
            m_arrivalCity = arrivalCity;
            m_departureTime = departureTime;
            m_arrivalTime = arrivalTime;
            m_isBusiness = isBusiness;
            m_price = price;
        }
    }
}