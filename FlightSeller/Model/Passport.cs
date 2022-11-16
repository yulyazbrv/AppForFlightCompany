using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSeller
{
    enum Gender { Male, Female }

    class Passport
    {
        public int m_passportNum { get; private set; }
        public string m_name { get; private set; }
        public string m_surname { get; private set; }
        public Gender m_gender { get; private set; }
        public DateTime m_birthday { get; private set; }
        public string m_passportInfo { get; private set; }
        public string m_citizenship { get; private set; }

        public Passport(int passportNum,
                       string name,
                       string surname,
                       Gender gender,
                       DateTime birthday,
                       string passportInfo,
                       string citizenship)
        {
            m_passportNum = passportNum;
            m_name = name;
            m_surname = surname;
            m_gender = gender;
            m_birthday = birthday;
            m_passportInfo = passportInfo;
            m_citizenship = citizenship;
        }
    }
}