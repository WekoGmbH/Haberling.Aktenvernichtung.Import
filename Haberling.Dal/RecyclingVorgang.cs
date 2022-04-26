using Sagede.Core.Tools;
using Sagede.OfficeLine.Engine;
using Sagede.OfficeLine.Shared;
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

        private string CreateArtikel(string artikelnummer)
        {
            try
            {
                var artikel = mandant.MainDevice.Entities.Artikel.Create(artikelnummer, mandant.Id);
                artikel.Matchcode = artikelnummer;
                artikel.Basismengeneinheit = "Stk";
                artikel.Lagerfuehrung = true;
                artikel.Hauptartikelgruppe = "700";
                artikel.Bezeichnung1 = artikelnummer;
                artikel.Aktiv = true;
                artikel.Save(Sagede.OfficeLine.Data.Entities.EntitySaveOptions.FireAndForget);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return artikelnummer;
        }

        private int AddVorgang(ImportDto.Vorgang vorgang)
        {
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
                        ,[Art])
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
                            ,@Art)";
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
            cmd.ExecuteNonQuery();
            cmd = null;
            foreach (var item in vorgang.Container)
            {
                item.VorgangId = tan;
                var typ = item.Artikelnummer;
                item.Artikelnummer = mandant.MainDevice.Lookup.GetString("Artikelnummer", "KHKArtikel", $"Matchcode = '{item.Artikelnummer}' AND Mandant = {mandant.Id}", "");
                if (item.Artikelnummer == "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Artikelnummer für {typ} nicht vorhanden!");
                    item.Artikelnummer = CreateArtikel(typ);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Artikelnummer {item.Artikelnummer} Für Typ {typ} ermittelt.");
                }
                AddContainer(item);
            }
            Console.ForegroundColor = ConsoleColor.White;
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
                entKto.Kto = string.Concat("D",kto);
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

    }
}
