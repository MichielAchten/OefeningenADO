using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGemeenschap
{
    public class Leverancier
    {
        private int levNrValue;
        private string naamValue;
        private string adresValue;
        private string postNrValue;
        private string woonplaatsValue;

        public int LevNr
        {
            get
            {
                return levNrValue;
            }
            set
            {
                levNrValue = value;
            }
        }
        public string Naam
        {
            get
            {
                return naamValue;
            }
            set
            {
                naamValue = value;
            }
        }
        public string Adres
        {
            get
            {
                return adresValue;
            }
            set
            {
                adresValue = value;
            }
        }
        public string PostNr
        {
            get
            {
                return postNrValue;
            }
            set
            {
                postNrValue = value;
            }
        }
        public string Woonplaats
        {
            get
            {
                return woonplaatsValue;
            }
            set
            {
                woonplaatsValue = value;
            }
        }
    }
}