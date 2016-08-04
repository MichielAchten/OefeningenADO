using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGemeenschap
{
    public class Soort
    {
        public Soort(string nSoortNaam, int nSoortNr)
        {
            SoortNaam = nSoortNaam;
            SoortNr = nSoortNr;
        }
        
        public int SoortNr { get; set; }
        public string SoortNaam { get; set; }

        public override string ToString()
        {
            return SoortNaam;
        }
    }
}
