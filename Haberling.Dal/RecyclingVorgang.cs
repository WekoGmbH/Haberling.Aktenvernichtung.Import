using Sagede.Core.Tools;
using Sagede.OfficeLine.Engine;
using Sagede.OfficeLine.Shared;
using Sagede.OfficeLine.Wawi.LagerEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.Dal
{
    public class RecyclingVorgang
    {

        internal Mandant mandant;

        public RecyclingVorgang(Mandant mandant)
        {
            this.mandant = mandant;
        }

        private int AddContainer(ImportDto.Container container)
        {
            if (container.Artikelnummer == "0")
                return 0;
            var query = @"INSERT INTO [dbo].[WekoVorgangBehaelter]
                       ([Id]
                       ,[Mandant]
                       ,[VorgangId]
                       ,[Seriennummer]
                       ,[Artikelnummer]
                       ,[Fraktion]
                       ,[Sicherheitsstufe]
                       ,[Schutzklasse]
                       ,[FraktionMe]
                       ,[Gewicht]
                       ,[Typ]
                       ,[Gestellungsart]
                       ,[Referenz]
                       ,[Code])
                 VALUES
                       (@Id
                       ,@Mandant
                       ,@VorgangId
                       ,@Seriennummer
                       ,@Artikelnummer
                       ,@Fraktion
                       ,@Sicherheitsstufe
                       ,@Schutzklasse
                       ,@FraktionMe
                       ,@Gewicht
                       ,@Typ
                       ,@Gestellungsart
                       ,@Referenz
                       ,@Code)";
            var cmd = mandant.MainDevice.GenericConnection.CreateSqlStringCommand(query);
            var tan = mandant.GetTan("WekoVorgangBehaelter");
            cmd.AppendInParameter("Id", typeof(int), tan);
            cmd.AppendInParameter("Mandant", typeof(short), mandant.Id);
            cmd.AppendInParameter("VorgangId", typeof(int), container.VorgangId);
            cmd.AppendInParameter("Seriennummer", typeof(string), container.Seriennummer);
            cmd.AppendInParameter("Artikelnummer", typeof(string), container.Artikelnummer);
            cmd.AppendInParameter("Fraktion", typeof(string), container.Fraktion);
            cmd.AppendInParameter("Sicherheitsstufe", typeof(string), container.Sicherheitsstufe);
            cmd.AppendInParameter("Schutzklasse", typeof(string), "keine");
            cmd.AppendInParameter("FraktionMe", typeof(string), container.Me);
            cmd.AppendInParameter("Gewicht", typeof(decimal), 0);
            cmd.AppendInParameter("Typ", typeof(int), container.Typ);
            cmd.AppendInParameter("Gestellungsart", typeof(bool), false);
            cmd.AppendInParameter("Referenz", typeof(int), 0);
            cmd.AppendInParameter("Code", typeof(string), "");
            cmd.ExecuteNonQuery();
            cmd = null;
            return tan;
        }

        private string TypToArtikel(string typ)
        {
            var typDic = new Dictionary<string, string>();
            typDic.Add("Typ C10", "A10");
            typDic.Add("Typ K110", "A1100");
            typDic.Add("Typ 2", "A22");
            typDic.Add("Typ 24", "A240");
            typDic.Add("Typ BR24", "A240");
            typDic.Add("Typ 24AP", "A240");
            typDic.Add("Typ K24", "A240");
            typDic.Add("Typ M24", "A240");
            typDic.Add("Typ C34", "A34");
            typDic.Add("Typ 35", "A350");
            typDic.Add("Typ C38", "A38");
            typDic.Add("Typ BR42", "A416");
            typDic.Add("Typ 50", "A500");
            typDic.Add("Typ 7", "A70");
            if (typDic.ContainsKey(typ))
                return typDic[typ];
            return null;
        }

        private int AddVorgang(ImportDto.Vorgang vorgang)
        {
            var seriennummernUmbuchung = new List<ImportDto.ItemSerialsForLargerbuchung>();
            var query = @"INSERT INTO [dbo].[WekoRecyclingVorgang]
                        ([Id]
                        ,[Mandant]
                        ,[Status]
                        ,[Phase]
                        ,[Datum]
                        ,[Erfassungsdatum]               
                        ,[Adresse]
                        ,[Bearbeiter]
                        ,[Bemerkung]           
                        ,[Art]
                        ,[BelID])
                        VALUES
                            (@Id
                            ,@Mandant
                            ,@Status
                            ,@Phase
                            ,@Datum
                            ,@Erfassungsdatum                         
                            ,@Adresse
                            ,@Bearbeiter
                            ,@Bemerkung                       
                            ,@Art
                            ,@BelID)";
            var cmd = mandant.MainDevice.GenericConnection.CreateSqlStringCommand(query);
            var tan = mandant.GetTan("WekoRecyclingVorgang");
            cmd.AppendInParameter("Id", typeof(int), tan);
            cmd.AppendInParameter("Mandant", typeof(short), mandant.Id);
            cmd.AppendInParameter("Status", typeof(string), "30");
            cmd.AppendInParameter("Phase", typeof(int), 0);
            cmd.AppendInParameter("Datum", typeof(DateTime), vorgang.Datum);
            cmd.AppendInParameter("Erfassungsdatum", typeof(DateTime), vorgang.Datum);
            cmd.AppendInParameter("Adresse", typeof(int), vorgang.Adresse);
            cmd.AppendInParameter("Bearbeiter", typeof(string), vorgang.Bearbeiter);
            cmd.AppendInParameter("Bemerkung", typeof(string), vorgang.Bemerkung);
            cmd.AppendInParameter("Art", typeof(string), vorgang.Art);

            foreach (var item in vorgang.Container)
            {
                item.VorgangId = tan;
                var typ = item.Artikelnummer;
                item.Artikelnummer = TypToArtikel(item.Artikelnummer);
                if (item.Artikelnummer == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Artikelnummer für {typ} nicht vorhanden!");
                    continue;
                }
                item.Artikelnummer = mandant.MainDevice.Lookup.GetString("Artikelnummer", "KHKArtikel", $"Artikelnummer = '{item.Artikelnummer}' AND Mandant = {mandant.Id}", "");
                if (item.Artikelnummer == "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Artikelnummer für {typ} nicht vorhanden!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Artikelnummer {item.Artikelnummer} Für Typ {typ} ermittelt.");
                }
                AddContainer(item);
                seriennummernUmbuchung.Add(new ImportDto.ItemSerialsForLargerbuchung(item.Artikelnummer, item.Seriennummer));
            }
            Console.ForegroundColor = ConsoleColor.White;
            var belId = ExecuteUmbuchung(seriennummernUmbuchung, mandant);
            cmd.AppendInParameter("BelID", typeof(int), belId);
            cmd.ExecuteNonQuery();
            cmd = null;
            Console.WriteLine($"Vorgang {tan} gespeichert.");
            return tan;
        }

        private static string GetMaxKto(Mandant mandant)
        {
            var kto = string.Empty;
            var query = @"SELECT MAX(Kto) AS Kto FROM KHKKontokorrent WHERE KtoArt = 'D' AND Mandant = @Mandant";
            var command = mandant.MainDevice.GenericConnection.CreateSqlStringCommand(query);
            command.AppendInParameter("Mandant", typeof(short), mandant.Id);
            using (var reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    kto = reader.GetString(0);
                }
            }
            //ToDo Feldformatierungen berücksichtigen!!! Aber mit 1 Stunde Zeit für die gesamte Funktion der Kundenanlage - keine Chance. 
            // mandant.GetInputMask(101);
            if (kto.Length > 2 && kto.StartsWith("D"))
            {
                kto = kto.Substring(1);
                var ktoNummeric = 0;
                if (Int32.TryParse(kto, out ktoNummeric))
                {
                    ktoNummeric++;
                    kto = string.Concat("D", ktoNummeric.ToString());
                }
            }
            return kto;
        }

        private static void CloneKto(object src, object dest)
        {
            Reflection.CopyProperties(src, dest);
        }

        public int CreateKto(string kto, string matchcode, string strasse)
        {
            var vorlageKto = mandant.PropertyManager.PropertiesAllUsers.GetString(997006);
            if (string.IsNullOrEmpty(vorlageKto))
            {
                throw new Exception("Keine Kundenvorlage konfiguriert.");
            }
            var entVorlageKto = mandant.MainDevice.Entities.Kontokorrent.GetItem(vorlageKto, mandant.Id);
            var entKto = mandant.MainDevice.Entities.Kontokorrent.Create(kto, mandant.Id);
            var adr = mandant.MainDevice.Entities.Adressen.Create(mandant.MainDevice.GetTan("KHKAdressen", mandant.Id), mandant.Id);
            adr.LieferPLZ = "10000";
            adr.LieferOrt = "Berlin";
            adr.LieferStrasse = strasse;
            adr.Name1 = matchcode;
            adr.Matchcode = matchcode;
            adr.Save(Sagede.OfficeLine.Data.Entities.EntitySaveOptions.FireAndForget);
            CloneKto(entVorlageKto, entKto);
            try
            {
                entKto.State = Sagede.OfficeLine.Data.Entities.EntityDataState.New;
                entKto.Kto = string.Concat("D", kto);
                entKto.Adresse = adr.Adresse;
                entKto.Rechnungsempfaenger = null;
                entKto.Matchcode = matchcode;
                entKto.Save(Sagede.OfficeLine.Data.Entities.EntitySaveOptions.FireAndForget);
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return adr.Adresse;
        }

        public int GetAdresse(string kto, string matchcode, string strasse)
        {
            var adresse = mandant.MainDevice.Lookup.GetInt32("Adresse", "KHKKontokorrent", $"Kto = 'D{kto}' AND Mandant = {mandant.Id}", 0);
            if (adresse == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Kontokorrent {kto} nicht vorhanden!");
                //Kunde anlegen
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Lege {kto} an.");
                adresse = CreateKto(kto, matchcode, strasse);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Adresse {adresse} für Kontokorrent {kto} ermittelt.");
            }

            return adresse;
        }

        public void Add(List<ImportDto.Vorgang> vorgange)
        {
            foreach (var vorgang in vorgange)
            {
                AddVorgang(vorgang);
            }
            Console.WriteLine($"{vorgange.Count} Vorgänge bei {vorgange.GroupBy(x => x.Adresse).Count()} Adressen angelegt.");
        }


        internal static int ExecuteUmbuchung(List<ImportDto.ItemSerialsForLargerbuchung> artikelSerials, Mandant mandant)
        {
            const string defaultText = "Recycling-Umbuchungen";
            LagerJob job = null;
            var containerLager = ConversionHelper.ToInt32(mandant.PropertyManager.GetValue(997003, true));
            var auslieferungsLager = ConversionHelper.ToInt32(mandant.PropertyManager.GetValue(997004, true));

            var paramList = new Sagede.OfficeLine.Data.QueryParameterList();
            paramList.AddClauseParameter(new Sagede.OfficeLine.Data.ClauseParameter("Erfassungsdatum", DateTime.Today, Sagede.OfficeLine.Data.ClauseParameterComparisonType.Equals));
            paramList.AddClauseParameter(new Sagede.OfficeLine.Data.ClauseParameter("Benutzer", mandant.Benutzer.Name, Sagede.OfficeLine.Data.ClauseParameterComparisonType.Equals));
            paramList.AddClauseParameter(new Sagede.OfficeLine.Data.ClauseParameter("Standardtext", defaultText, Sagede.OfficeLine.Data.ClauseParameterComparisonType.Equals));
            paramList.AddClauseParameter(new Sagede.OfficeLine.Data.ClauseParameter("Mandant", mandant.Id, Sagede.OfficeLine.Data.ClauseParameterComparisonType.Equals));

            var efJob = mandant.MainDevice.Entities.LagerplatzbuchungenJobs.GetItem(paramList);
            job = new LagerJob(mandant, DateTime.Today, (short)mandant.PeriodenManager.Perioden.Date2Periode(DateTime.Today).Jahr);
            job.Erfassungsdatum = DateTime.Today;
            job.Benutzer = mandant.Benutzer.Name;
            job.Standardtext = defaultText;
            if (efJob != null)
            {
                job.JobHandle = efJob.Job;
            }

            // Initialisieren der Lagerplätze
            var lagerPlatzHerkunft = new Lagerplatz(mandant, DateTime.Today);
            lagerPlatzHerkunft.PlatzHandle = containerLager;
            var lagerPlatzZiel = new Lagerplatz(mandant, DateTime.Today);
            lagerPlatzZiel.PlatzHandle = auslieferungsLager;


            foreach (var item in artikelSerials)
            {

                // Initialisierung der Lagerbuchung
                var lagerBuchung = new LagerplatzBuchung(mandant, DateTime.Today, (short)mandant.PeriodenManager.Perioden.Date2Periode(DateTime.Today).Jahr);

                lagerBuchung.BuchungsHandle = 0;
                lagerBuchung.Status = 0;
                lagerBuchung.Artikelnummer = item.Artikelnummer;
                //Keine Varianten
                lagerBuchung.AuspraegungsHandle = 0;

                lagerBuchung.MengeLager = 1;
                //Interne Umbuchung
                lagerBuchung.Lagerbewegungsart.Bewegungsart = "IU";
                lagerBuchung.Bewegungsdatum = DateTime.Today;


                // lagerBuchung auf Seriennummernpflicht setzen
                lagerBuchung.SerienNummernPflichtigkeit = true;
                // Initialisierung der Seriennummern-Collection
                lagerBuchung.SerienNummernCollection = new SeriennummernEintragCollection();
                lagerBuchung.SerienNummernCollection.Add(new SeriennummernEintrag() { Seriennummer = item.Seriennummer });
                lagerBuchung.Save(ref lagerPlatzHerkunft, ref lagerPlatzZiel, job, false, false, true, true, false);
                lagerBuchung = null;
                lagerPlatzHerkunft = null;
                lagerPlatzZiel = null;
            }


            if (job != null)
                job.SaveJob();
            var lagerJobId = job.JobHandle;
            job = null;
            return lagerJobId;
        }

    }
}
