using Sagede.OfficeLine.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.Importer
{
    internal class Program
    {

        static int GetTypBewegungsart(string bewegungsart)
        {
            switch (bewegungsart)
            {
                case "Anlieferung":
                    return 10;
                case "Abholung":
                    return 20;
                case "Behältertausch":
                    return 10; //40, aber es gibt keine Anlieferung
                default:
                    return 30;
            }
        }

        private static Mandant CreateMandant()
        {
            var user = System.Configuration.ConfigurationManager.AppSettings["user"];
            var password = System.Configuration.ConfigurationManager.AppSettings["password"];
            var database = System.Configuration.ConfigurationManager.AppSettings["database"];
            var mandantId = Convert.ToInt16(System.Configuration.ConfigurationManager.AppSettings["mandant"]);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Baue Datenbankverbindung mit Benutzer {user} zu Datenbank {database}-{mandantId} auf.");
            return Dal.MandantManager.CreateMandant(user, password, database, mandantId);
        }

        private static void ImportSerials(Mandant mandant)
        {
            var snDir = System.Configuration.ConfigurationManager.AppSettings["importdirSn"];
            var artikelgruppe = System.Configuration.ConfigurationManager.AppSettings["artikelgruppe"];
            var snImporter = new Dal.Seriennummern(mandant);
            foreach (var file in System.IO.Directory.GetFiles(snDir, "*.csv"))
            {
                var artikel = Dal.Artikel.CreateArtikel(System.IO.Path.GetFileNameWithoutExtension(file), artikelgruppe, mandant);
                if (string.IsNullOrEmpty(artikel))
                    continue;
                var serials = Csv.SerialNumberCsvReader.Read(file, artikel, 0);
                var dtoLagerbuchung = new ImportDto.Lagerbuchung();
                dtoLagerbuchung.Artikelnummer = artikel;
                dtoLagerbuchung.AuspraegungId = 0;
                dtoLagerbuchung.Buchungstyp = ImportDto.LagerbuchungartType.ZM;
                dtoLagerbuchung.LagerkennungZLP = System.Configuration.ConfigurationManager.AppSettings["ziellager"];
                foreach (var item in serials)
                {
                    dtoLagerbuchung.Seriennummern.Add(new ImportDto.SeriennummerItem()
                    {
                        Material = item.Material,
                        Schloss = item.Schloss,
                        Seriennummer = item.Serial
                    });
                }
                dtoLagerbuchung.Menge = dtoLagerbuchung.Seriennummern.Count();
                snImporter.Import(dtoLagerbuchung, artikel);
            }

        }

        static void ImportVorgaenge(Mandant mandant)
        {

            var importfile = System.Configuration.ConfigurationManager.AppSettings["importfile"];
            var vorgange = Csv.ContainerReader.Read(importfile);
            var dal = new Dal.RecyclingVorgang(mandant);
            var dtoVorgaenge = new List<ImportDto.Vorgang>();

            foreach (var csvVorgang in vorgange.GroupBy(x => new { x.Erfassungsnummer, x.Kunde, x.Name, x.Strasse, x.Typ, x.Datum }))
            {
                dtoVorgaenge.Add(new ImportDto.Vorgang()
                {
                    Adresse = dal.GetAdresse(csvVorgang.Key.Kunde, csvVorgang.Key.Name, csvVorgang.Key.Strasse),
                    Art = csvVorgang.Key.Typ,
                    Bearbeiter = "sage",
                    Bemerkung = "Importlauf",
                    Datum = csvVorgang.Key.Datum,
                    Erfassungsdatum = csvVorgang.Key.Datum,
                    Phase = "0",
                    Status = "10",
                    Container = new List<ImportDto.Container>()
                });
                foreach (var containerItem in csvVorgang)
                {
                    dtoVorgaenge.Last().Container.Add(

                         new ImportDto.Container()
                         {
                             VorgangId = 0,
                             Artikelnummer = containerItem.Container,
                             Me = "",
                             Fraktion = "",
                             Schutzklasse = "",
                             Seriennummer = containerItem.Seriennummer,
                             Sicherheitsstufe = 1,
                             Typ = GetTypBewegungsart(csvVorgang.Key.Typ)
                         }
                        );
                }
            }
            dal.Add(dtoVorgaenge);
        }

        static void Main(string[] args)
        {
            var mandant = CreateMandant();
            ImportVorgaenge(mandant);
            //ImportSerials(mandant);
            Console.WriteLine("Zum Beenden bitte Taste drücken.");
            Console.ReadLine();
        }
    }
}
