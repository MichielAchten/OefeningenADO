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

                    using (var comAutoNumber = conTuincentrum.CreateCommand())
                    {
                        comAutoNumber.CommandType = CommandType.StoredProcedure;
                        comAutoNumber.CommandText = "AutoNumberOphalen";
                        conTuincentrum.Open();
                        comLeverancierToevoegen.ExecuteNonQuery();
                        eenLeverancier.LevNr = Convert.ToInt32(comAutoNumber.ExecuteScalar());
                    }
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
            return 0;
        }

        public Decimal GemiddeldePrijsVanEenSoort(String soort)
        {
            var manager = new TuincentrumDbManager();
            using (var conTuin = manager.GetConnection())
            {
                using (var comGemiddelde = conTuin.CreateCommand())
                {
                    comGemiddelde.CommandType = CommandType.StoredProcedure;
                    comGemiddelde.CommandText = "GemiddeldePrijsVanEenSoort";
                    var parSoort = comGemiddelde.CreateParameter();
                    parSoort.ParameterName = "@soort";
                    parSoort.Value = soort;
                    comGemiddelde.Parameters.Add(parSoort);
                    conTuin.Open();
                    var resultaat = comGemiddelde.ExecuteScalar();
                    if (resultaat == DBNull.Value)
                    {
                        throw new Exception("Soort bestaat niet");
                    }
                    else
                    {
                        return (Decimal)resultaat;
                    }
                }
            }
        }


        public PlantGegevens PlantGegevensOpzoeken(int plantNr)
        {
            var manager = new TuincentrumDbManager();
            using (var conTuin = manager.GetConnection())
            {
                using (var comPlantGegevens = conTuin.CreateCommand())
                {
                    comPlantGegevens.CommandType = CommandType.StoredProcedure;
                    comPlantGegevens.CommandText = "PlantenGegevens";

                    var parPlantNr = comPlantGegevens.CreateParameter();
                    parPlantNr.ParameterName = "@plantNr";
                    parPlantNr.Value = plantNr;
                    comPlantGegevens.Parameters.Add(parPlantNr);

                    var parPlantNaam = comPlantGegevens.CreateParameter();
                    parPlantNaam.ParameterName = "@plantnaam";
                    parPlantNaam.DbType = DbType.String;
                    parPlantNaam.Size = 30;
                    parPlantNaam.Direction = ParameterDirection.Output;
                    comPlantGegevens.Parameters.Add(parPlantNaam);

                    var parSoort = comPlantGegevens.CreateParameter();
                    parSoort.ParameterName = "@soort";
                    parSoort.DbType = DbType.String;
                    parSoort.Size = 10;
                    parSoort.Direction = ParameterDirection.Output;
                    comPlantGegevens.Parameters.Add(parSoort);

                    var parLeverancier = comPlantGegevens.CreateParameter();
                    parLeverancier.ParameterName = "@leverancier";
                    parLeverancier.DbType = DbType.String;
                    parLeverancier.Size = 30;
                    parLeverancier.Direction = ParameterDirection.Output;
                    comPlantGegevens.Parameters.Add(parLeverancier);

                    var parKleur = comPlantGegevens.CreateParameter();
                    parKleur.ParameterName = "@kleur";
                    parKleur.DbType = DbType.String;
                    parKleur.Size = 10;
                    parKleur.Direction = ParameterDirection.Output;
                    comPlantGegevens.Parameters.Add(parKleur);

                    var parKostprijs = comPlantGegevens.CreateParameter();
                    parKostprijs.ParameterName = "@kostprijs";
                    parKostprijs.DbType = DbType.Currency;
                    parKostprijs.Size = 10;
                    parKostprijs.Direction = ParameterDirection.Output;
                    comPlantGegevens.Parameters.Add(parKostprijs);

                    conTuin.Open();
                    comPlantGegevens.ExecuteNonQuery();
                    if (parPlantNaam.Value.Equals(DBNull.Value))
                    {
                        throw new Exception("Plantgegevens niet gevonden");
                    }
                    return new PlantGegevens(parPlantNaam.Value.ToString(),
                        parSoort.Value.ToString(), parLeverancier.Value.ToString(),
                        parKleur.Value.ToString(), (Decimal)parKostprijs.Value);
                }
            }
        }

        public List<Soort> GetSoorten()
        {
            var soorten = new List<Soort>();
            var manager = new TuincentrumDbManager();
            using (var conTuin = manager.GetConnection())
            {
                using (var comSoorten = conTuin.CreateCommand())
                {
                    comSoorten.CommandType = CommandType.Text;
                    comSoorten.CommandText = "select SoortNr, soort from Soorten order by Soort";
                    conTuin.Open();
                    using (var rdrSoorten = comSoorten.ExecuteReader())
                    {
                        var soortPos = rdrSoorten.GetOrdinal("soort");
                        var soortnrPos = rdrSoorten.GetOrdinal("soortnr");
                        while (rdrSoorten.Read())
                        {
                            soorten.Add(new Soort(rdrSoorten.GetString(soortPos), rdrSoorten.GetInt32(soortnrPos)));
                        }
                    }
                }
            }
            return soorten;
        }

        public List<Plant> GetPlanten(int soortnr)
        {
            var planten = new List<Plant>();
            var manager = new TuincentrumDbManager();
            using (var conTuin = manager.GetConnection())
            {
                using (var comPlanten = conTuin.CreateCommand())
                {
                    comPlanten.CommandType = CommandType.Text;
                    comPlanten.CommandText = "Select * from planten where soortnr=@soortnr order by naam";

                    var parSoortNr = comPlanten.CreateParameter();
                    parSoortNr.ParameterName = "@soortnr";
                    parSoortNr.Value = soortnr;
                    comPlanten.Parameters.Add(parSoortNr);
                    conTuin.Open();
                    using (var rdrPlanten = comPlanten.ExecuteReader())
                    {
                        var plantNaamPos = rdrPlanten.GetOrdinal("Naam");
                        var plantNrPos = rdrPlanten.GetOrdinal("plantnr");
                        var levnrPos = rdrPlanten.GetOrdinal("levnr");
                        var prijsPos = rdrPlanten.GetOrdinal("verkoopprijs");
                        var kleurPos = rdrPlanten.GetOrdinal("kleur");
                        var soortPos = rdrPlanten.GetOrdinal("soortnr");
                        while (rdrPlanten.Read())
                        {
                            var eenPlant = new Plant(
                                rdrPlanten.GetString(plantNaamPos),
                                rdrPlanten.GetInt32(plantNrPos),
                                rdrPlanten.GetInt32(levnrPos),
                                rdrPlanten.GetDecimal(prijsPos),
                                rdrPlanten.GetString(kleurPos));
                            planten.Add(eenPlant);
                        }
                    }
                }
            }
            return planten;
        }

        public void GewijzigdePlantenOpslaan(List<Plant> planten)
        {
            var manager = new TuincentrumDbManager();
            using (var conPlant = manager.GetConnection())
            {
                using (var comUpdate = conPlant.CreateCommand())
                {
                    comUpdate.CommandType = CommandType.Text;
                    comUpdate.CommandText = "update planten set Kleur=@kleur, VerkoopPrijs=@prijs where PlantNr=@plantenr";

                    var parKleur = comUpdate.CreateParameter();
                    parKleur.ParameterName = "@kleur";
                    comUpdate.Parameters.Add(parKleur);

                    var parPrijs = comUpdate.CreateParameter();
                    parPrijs.ParameterName = "@prijs";
                    comUpdate.Parameters.Add(parPrijs);

                    var parPlantNr = comUpdate.CreateParameter();
                    parPlantNr.ParameterName = "@plantnr";
                    comUpdate.Parameters.Add(parPlantNr);

                    conPlant.Open();
                    foreach (Plant p in planten)
                    {
                        parKleur.Value = p.Kleur;
                        parPrijs.Value = p.Prijs;
                        parPlantNr.Value = p.PlantNr;
                        if (comUpdate.ExecuteNonQuery() == 0)
                        {
                            throw new Exception(p.PlantNaam + " opslaan mislukt");
                        }
                    }
                }
            }
        }
    }
}
