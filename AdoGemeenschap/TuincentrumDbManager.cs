using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Collections.ObjectModel;

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

        public ObservableCollection<Leverancier> GetLeveranciers()
        {
            ObservableCollection<Leverancier> leveranciers = new ObservableCollection<Leverancier>();
            var manager = new TuincentrumDbManager();
            using (var conPlanten = manager.GetConnection())
            {
                using (var comLeveranciers = conPlanten.CreateCommand())
                {
                    comLeveranciers.CommandType = CommandType.Text;
                    comLeveranciers.CommandText = "select * from Leveranciers";
                    conPlanten.Open();
                    using (var rdrLeveranciers = comLeveranciers.ExecuteReader())
                    {
                        Int32 leverancierNrPos = rdrLeveranciers.GetOrdinal("LevNr");
                        Int32 naamPos = rdrLeveranciers.GetOrdinal("Naam");
                        Int32 adresPos = rdrLeveranciers.GetOrdinal("Adres");
                        Int32 postcodePos = rdrLeveranciers.GetOrdinal("PostNr");
                        Int32 gemeentePos = rdrLeveranciers.GetOrdinal("Woonplaats");
                        while (rdrLeveranciers.Read())
                        {
                            leveranciers.Add(
                            new Leverancier(rdrLeveranciers.GetInt32(leverancierNrPos),
                            rdrLeveranciers.GetString(naamPos),
                            rdrLeveranciers.GetString(adresPos),
                            rdrLeveranciers.GetString(postcodePos),
                            rdrLeveranciers.GetString(gemeentePos)));
                        }
                    }
                }
            }
            return leveranciers;
        }

        public void SchrijfVerwijderingen(List<Leverancier> leveranciers)
        {
            var manager = new TuincentrumDbManager();
            using (var conTuin = manager.GetConnection())
            {
                using (var comDelete = conTuin.CreateCommand())
                {
                    comDelete.CommandType = CommandType.Text;
                    comDelete.CommandText = "delete from leveranciers where LevNr=@levnr";

                    var parLevnr = comDelete.CreateParameter();
                    parLevnr.ParameterName = "@levnr";
                    comDelete.Parameters.Add(parLevnr);

                    conTuin.Open();
                    foreach (Leverancier eenLeverancier in leveranciers)
                    {
                        parLevnr.Value = eenLeverancier.LevNr;
                        comDelete.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SchrijfToevoegingen(List<Leverancier> leveranciers)
        {
            var manager = new TuincentrumDbManager();
            using (var conTuin = manager.GetConnection())
            {
                using (var comInsert = conTuin.CreateCommand())
                {
                    comInsert.CommandType = CommandType.Text;
                    comInsert.CommandText = @"Insert into leveranciers (Naam,Adres,PostNr,Woonplaats)
                        values (@naam,@adres,@postcode,@gemeente)";

                    var parNaam = comInsert.CreateParameter();
                    parNaam.ParameterName = "@naam";
                    comInsert.Parameters.Add(parNaam);

                    var parAdres = comInsert.CreateParameter();
                    parAdres.ParameterName = "@adres";
                    comInsert.Parameters.Add(parAdres);

                    var parPostcode = comInsert.CreateParameter();
                    parPostcode.ParameterName = "@postcode";
                    comInsert.Parameters.Add(parPostcode);

                    var parGemeente = comInsert.CreateParameter();
                    parGemeente.ParameterName = "@gemeente";
                    comInsert.Parameters.Add(parGemeente);

                    conTuin.Open();
                    foreach (Leverancier eenLeverancier in leveranciers)
                    {
                        parNaam.Value = eenLeverancier.Naam;
                        parAdres.Value = eenLeverancier.Adres;
                        parPostcode.Value = eenLeverancier.PostNr;
                        parGemeente.Value = eenLeverancier.Woonplaats;
                        comInsert.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SchrijfWijzigingen(List<Leverancier> leveranciers)
        {
            var manager = new TuincentrumDbManager();
            using (var conLeveranciers = manager.GetConnection())
            {
                using (var comUpdate = conLeveranciers.CreateCommand())
                {
                    comUpdate.CommandType = CommandType.Text;
                    comUpdate.CommandText = @"update leveranciers set Naam=@naam,Adres=@adres, PostNr=@postnr, 
                        Woonplaats=@woonplaats where LevNr=@levnr";

                    var parNaam = comUpdate.CreateParameter();
                    parNaam.ParameterName = "@naam";
                    comUpdate.Parameters.Add(parNaam);

                    var parAdres = comUpdate.CreateParameter();
                    parAdres.ParameterName = "@adres";
                    comUpdate.Parameters.Add(parAdres);

                    var parPostNr = comUpdate.CreateParameter();
                    parPostNr.ParameterName = "@postnr";
                    comUpdate.Parameters.Add(parPostNr);

                    var parWoonplaats = comUpdate.CreateParameter();
                    parWoonplaats.ParameterName = "@woonplaats";
                    comUpdate.Parameters.Add(parWoonplaats);

                    var parLevNr = comUpdate.CreateParameter();
                    parLevNr.ParameterName = "@levnr";
                    comUpdate.Parameters.Add(parLevNr);

                    conLeveranciers.Open();
                    foreach (var eenLeverancier in leveranciers)
                    {
                        parNaam.Value = eenLeverancier.Naam;
                        parAdres.Value = eenLeverancier.Adres;
                        parPostNr.Value = eenLeverancier.PostNr;
                        parWoonplaats.Value = eenLeverancier.Woonplaats;
                        parLevNr.Value = eenLeverancier.LevNr;
                        comUpdate.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<String> GetPostnummers()
        {
            var manager = new TuincentrumDbManager();
            List<string> postnummers = new List<string>();
            using (var conTuin = manager.GetConnection())
            {
                using (var comPostnummers = conTuin.CreateCommand())
                {
                    comPostnummers.CommandType = CommandType.StoredProcedure;
                    comPostnummers.CommandText = "PostNummers";
                    conTuin.Open();

                    using (var rdrPostnummers = comPostnummers.ExecuteReader())
                    {
                        Int32 postcodePos = rdrPostnummers.GetOrdinal("PostNr");
                        while (rdrPostnummers.Read())
                        {
                            postnummers.Add(rdrPostnummers.GetString(postcodePos));
                        }
                    }
                }
            }
            return postnummers;
        }
    }
}
