using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGemeenschap
{
    public class Leverancier
    {
        public Leverancier(int nLevNr, string nNaam, string nAdres, string nPostNr, string nWoonplaats)
        {
            LevNr = nLevNr;
            Naam = nNaam;
            Adres = nAdres;
            PostNr = nPostNr;
            Woonplaats = nWoonplaats;
            Changed = false;
        }

        public Leverancier()
        {

        }
        
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
                Changed = true;
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
                Changed = true;
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
                Changed = true;
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
                Changed = true;
            }
        }

        public bool Changed { get; set; }
    }
}