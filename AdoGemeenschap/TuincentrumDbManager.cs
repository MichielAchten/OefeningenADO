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

        public void LeverancierToevoegen(Leverancier eenLeverancier)
        {
            var dbManager = new TuincentrumDbManager();

            using (var conTuincentrum = dbManager.GetConnection())
            {
                using (var comLeverancierToevoegen = conTuincentrum.CreateCommand())
                {
                    comLeverancierToevoegen.CommandType = CommandType.StoredProcedure;
                    comLeverancierToevoegen.CommandText = "LeverancierToevoegen";

                    DbParameter parNaam = comLeverancierToevoegen.CreateParameter();
                    parNaam.ParameterName = "@Naam";
                    parNaam.Value = eenLeverancier.Naam;
                    comLeverancierToevoegen.Parameters.Add(parNaam);

                    DbParameter parAdres = comLeverancierToevoegen.CreateParameter();
                    parAdres.ParameterName = "@Adres";
                    parAdres.Value = eenLeverancier.Adres;
                    comLeverancierToevoegen.Parameters.Add(parAdres);

                    DbParameter parPostNr = comLeverancierToevoegen.CreateParameter();
                    parPostNr.ParameterName = "@PostNr";
                    parPostNr.Value = eenLeverancier.PostNr;
                    comLeverancierToevoegen.Parameters.Add(parPostNr);

                    DbParameter parWoonPlaats = comLeverancierToevoegen.CreateParameter();
                    parWoonPlaats.ParameterName = "@Woonplaats";
                    parWoonPlaats.Value = eenLeverancier.Woonplaats;
                    comLeverancierToevoegen.Parameters.Add(parWoonPlaats);

                    conTuincentrum.Open();
                    comLeverancierToevoegen.ExecuteNonQuery();
                }
            }
        }

        public int EindejaarsKorting()
        {
            var dbManager = new TuincentrumDbManager();
            using (var conTuincentrum = dbManager.GetConnection())
            {
                using (var comEindejaarsKorting = conTuincentrum.CreateCommand())
                {
                    comEindejaarsKorting.CommandType = CommandType.StoredProcedure;
                    comEindejaarsKorting.CommandText = "EindejaarsKorting";

                    conTuincentrum.Open();
                    return comEindejaarsKorting.ExecuteNonQuery();
                }
            }
        }
    }
}
