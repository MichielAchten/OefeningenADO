using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoGemeenschap
{
    public class Plant
    {
        public Plant(string nPlantNaam, int nPlantNr, int NLevNr, decimal nPrijs, string nKleur)
        {
            PlantNaam = nPlantNaam;
            PlantNr = nPlantNr;
            LevNr = NLevNr;
            Prijs = nPrijs;
            Kleur = nKleur;
            Changed = false;
        }

        public Plant()
        {

        }

        private decimal prijsValue;
        private string kleurValue;
        
        public bool Changed { get; set; }
        public string PlantNaam { get; set; }
        public int PlantNr { get; set; }
        public int LevNr { get; set; }

        public decimal Prijs
        {
            get
            {
                return prijsValue;
            }
            set
            {
                prijsValue = value;
                Changed = true;
            }
        }

        public string Kleur 
        {
            get
            {
                return kleurValue;
            }
            set
            {
                kleurValue = value;
                Changed = true;
            }
        }
    }
}
