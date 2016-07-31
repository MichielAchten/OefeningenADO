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

        public int VervangLeverancier(int oudeLevNr, int nieuweLevNr)
        {
            var manager = new TuincentrumDbManager();
            using (var conTuin = manager.GetConnection())
            {
                conTuin.Open();
                using (var traVervangen = conTuin.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    using (var comWijzigen = conTuin.CreateCommand())
                    {
                        comWijzigen.Transaction = traVervangen;
                        comWijzigen.CommandType = CommandType.StoredProcedure;
                        comWijzigen.CommandText = "LeverancierWijzigen";

                        var parOudeLevNr = comWijzigen.CreateParameter();
                        parOudeLevNr.ParameterName = "@OudeLevNr";
                        parOudeLevNr.Value = oudeLevNr;
                        comWijzigen.Parameters.Add(parOudeLevNr);

                        var parNieuweLevNr = comWijzigen.CreateParameter();
                        parNieuweLevNr.ParameterName = "@NieuweLevNr";
                        parNieuweLevNr.Value = nieuweLevNr;
                        comWijzigen.Parameters.Add(parNieuweLevNr);
                        if (comWijzigen.ExecuteNonQuery() == 0)
                        {
                            traVervangen.Rollback();
                            throw new Exception("Leverancier " + oudeLevNr + 
                                " kon niet vervangen worden door " + nieuweLevNr);
                        }
                    }
                    using (var comVerwijderen = conTuin.CreateCommand())
                    {
                        comVerwijderen.Transaction = traVervangen;
                        comVerwijderen.CommandType = CommandType.StoredProcedure;
                        comVerwijderen.CommandText = "LeveranciersVerwijderen";
                        var parLevNr = comVerwijderen.CreateParameter();
                        parLevNr.ParameterName = "@LevNr";
                        parLevNr.Value = oudeLevNr;
                        comVerwijderen.Parameters.Add(parLevNr);
                        if (comVerwijderen.ExecuteNonQuery() == 0)
                        {
                            traVervangen.Rollback();
                            throw new Exception("Leverancier " + oudeLevNr + " kon niet verwijderd worden");
                        }
                        traVervangen.Commit();
                    }
                }
            }



            //Hier verder afwerken
            return 0;
        }
    }
}
