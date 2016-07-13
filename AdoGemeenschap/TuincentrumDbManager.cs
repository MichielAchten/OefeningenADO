using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.Data;

namespace AdoGemeenschap
{
    public class TuincentrumDbManager
    {
        private static ConnectionStringSettings conTuincentrumSetting = ConfigurationManager.ConnectionStrings["Tuincentrum"];
        private static DbProviderFactory factory = DbProviderFactories.GetFactory(conTuincentrumSetting.ProviderName);

        public DbConnection GetConnection()
        {
            var conTuincentrum = factory.CreateConnection();
            conTuincentrum.ConnectionString = conTuincentrumSetting.ConnectionString;
            return conTuincentrum; 
        }

        public Boolean LeverancierToevoegen(String naam, String adres, String postcode, String plaats)
        {
            var dbManager = new TuincentrumDbManager();
            using (var conTuincentrum = dbManager.GetConnection())
            {
                using (var comLeverancierToevoegen = conTuincentrum.CreateCommand())
                {
                    comLeverancierToevoegen.CommandType = CommandType.Text;
                    comLeverancierToevoegen.CommandText = "LeverancierToevoegen";

                    DbParameter parNaam = comLeverancierToevoegen.CreateParameter();
                    parNaam.ParameterName = "@naam";
                    parNaam.Value = naam;
                    comLeverancierToevoegen.Parameters.Add(parNaam);

                    DbParameter parAdres = comLeverancierToevoegen.CreateParameter();
                    parAdres.ParameterName = "@adres";
                    parAdres.Value = adres;
                    comLeverancierToevoegen.Parameters.Add(parAdres);

                    DbParameter parPostcode = comLeverancierToevoegen.CreateParameter();
                    parPostcode.ParameterName = "@postcode";
                    parPostcode.Value = postcode;
                    comLeverancierToevoegen.Parameters.Add(parPostcode);

                    DbParameter parPlaats = comLeverancierToevoegen.CreateParameter();
                    parPlaats.ParameterName = "@plaats";
                    parPlaats.Value = plaats;
                    comLeverancierToevoegen.Parameters.Add(parPlaats);

                    conTuincentrum.Open();
                    return comLeverancierToevoegen.ExecuteNonQuery() != 0;
                }
            }
        }
    }
}
