using Haberling.ImportDto;
using Sagede.Core.Tools;
using Sagede.OfficeLine.Engine;
using Sagede.OfficeLine.Shared;
using Sagede.OfficeLine.Wawi.LagerEngine;
using System;
using System.Collections.Generic;

namespace Haberling.Dal
{
    public class Seriennummern
    {

        private Mandant _Mandant;

        public Seriennummern(Mandant mandant)
        {
            _Mandant = mandant;
        }


        private void SaveAdditionalSeriennummerInfo(SeriennummerItem serialItem, string artikelnummer)
        {
            var query = @"INSERT INTO [dbo].[WekoRecyclingContainerSeriennummern]
           ([Seriennummer]
           ,[Mandant]
           ,[Artikelnummer]
           ,[Material]
           ,[Schloss])
            VALUES
           (@Seriennummer
           ,@Mandant
           ,@Artikelnummer
           ,@Material
           ,@Schloss)";
            var cmd = _Mandant.MainDevice.GenericConnection.CreateSqlStringCommand(query);
            cmd.AppendInParameter("Seriennummer", typeof(string), serialItem.Seriennummer);
            cmd.AppendInParameter("Mandant", typeof(short), _Mandant.Id);
            cmd.AppendInParameter("Artikelnummer", typeof(string), artikelnummer);
            cmd.AppendInParameter("Material", typeof(string), serialItem.Material);
            cmd.AppendInParameter("Schloss", typeof(string), serialItem.Schloss);
            cmd.ExecuteNonQuery();
        }

        public void Import(Lagerbuchung lagerbuchung, string jobname)
        {
            Import(new List<Lagerbuchung>() { lagerbuchung }, jobname);
        }

        public void Import(List<ImportDto.Lagerbuchung> lagerbuchungen, string jobname)
        {


            //Deklaration der Objekte
            Sagede.OfficeLine.Wawi.LagerEngine.LagerJob job = null;
            Sagede.OfficeLine.Wawi.LagerEngine.LagerplatzBuchung lagerbuchung = null;


            // Transaktion pro Datei
            _Mandant.MainDevice.GenericConnection.BeginTransaction();

            try
            {
                //ToDo: Implementierung der Lagerbuchungserstellung
                if (lagerbuchungen.Count > 0)
                {
                    //1. Schritt => Erzeugung des Lagerjobs
                    job = new LagerJob(_Mandant, DateTime.Today, (short)_Mandant.PeriodenManager.Perioden.Date2Periode(DateTime.Today).Jahr, true);
                    job.Erfassungsdatum = DateTime.Today;
                    job.Benutzer = _Mandant.Benutzer.Name;
                    job.Standardtext = $"Lagerbuchungen aus Datei {jobname}".Left(100);
                    job.Memo = jobname;
                    if (!job.SaveJob())
                    {
                        throw new LagerbuchungException($"Fehler beim Speichern des Lagerjobs {jobname}");
                    }
                }

                lagerbuchungen.ForEach(l =>
                {
                    //Lagerbuchung durchführen
                    //Schritt 1: Lagerplätze initialisieren
                    Lagerplatz lagerplatzHerkunft;
                    Lagerplatz lagerplatzZiel;
                    lagerplatzHerkunft = new Lagerplatz(_Mandant, job.Erfassungsdatum, true);
                    lagerplatzHerkunft.PlatzHandle = _Mandant.MainDevice.Lookup.GetInt32("PlatzID", "KHKLagerPlaetze", $"Mandant = {_Mandant.Id} AND Kurzbezeichnung={SqlStrings.ToSqlString(l.LagerkennungHLP)}");
                    lagerplatzZiel = new Lagerplatz(_Mandant, job.Erfassungsdatum, true);
                    lagerplatzZiel.PlatzHandle = _Mandant.MainDevice.Lookup.GetInt32("PlatzID", "KHKLagerPlaetze", $"Mandant = {_Mandant.Id} AND Kurzbezeichnung={SqlStrings.ToSqlString(l.LagerkennungZLP)}");

                    //Schritt 2: Lagerbuchung initialisieren
                    lagerbuchung = new LagerplatzBuchung(_Mandant, job.Erfassungsdatum, (short)_Mandant.PeriodenManager.Perioden.Date2Periode(job.Erfassungsdatum).Jahr, true);

                    // Schritt 3: Artikeldaten laden
                    var artikelItem = _Mandant.MainDevice.Entities.Artikel.GetItem(l.Artikelnummer, _Mandant.Id);
                    if (artikelItem == null) throw new LagerbuchungException($"Artikel {l.Artikelnummer} nicht in Datenbank.");

                    var artikelVariante = _Mandant.MainDevice.Entities.ArtikelVarianten.GetItem(l.Artikelnummer, _Mandant.Id, l.AuspraegungId);
                    if (artikelItem == null) throw new LagerbuchungException($"Artikelvariante {l.Artikelnummer}|{l.AuspraegungId} nicht in Datenbank.");

                    //Schritt 4: Lagerbuchungsobjekt besetzen
                    lagerbuchung.Artikelnummer = l.Artikelnummer;
                    lagerbuchung.AuspraegungsHandle = l.AuspraegungId;
                    lagerbuchung.MengeLager = l.Menge;
                    lagerbuchung.MengeBasis = ArtikelMengen.Transform(l.Menge, artikelItem.UmrechnungsFaktorLMEValue, false, artikelItem.DezimalstellenBasisValue, 0);
                    lagerbuchung.Bewegungsdatum = DateTime.Today;


                    switch (l.Buchungstyp)
                    {

                        case LagerbuchungartType.IU:
                            lagerbuchung.Bewegungsart = "IU";
                            break;
                        case LagerbuchungartType.ZM:
                            lagerbuchung.Bewegungsart = "ZM";
                            lagerplatzHerkunft = null;
                            break;
                        case LagerbuchungartType.EM:
                            lagerbuchung.Bewegungsart = "EM";
                            lagerplatzZiel = null;
                            break;
                        case LagerbuchungartType.Undefined:
                        default:
                            throw new LagerbuchungException("Falscher Buchungstyp");
                    }

                    // Einstandspreis
                    if (l.Einstandspreis == 0)
                    {
                        lagerbuchung.Einstandspreis = artikelVariante.MittlererEKValue * lagerbuchung.MengeBasis;
                    }
                    else
                    {
                        lagerbuchung.Einstandspreis = l.Einstandspreis;
                    }

                    // Chargen und Seriennummern
                    //0 ohne, 2 = nur Verkauf, 3 = Verkauf und Lager
                    lagerbuchung.ChargenPflichtigkeit = artikelItem.Chargenpflicht > 2;

                    if (lagerbuchung.ChargenPflichtigkeit)
                    {
                        // Achung => nur Instaziieren, wenn die Lagerbuchung chargenpflichtig ist
                        //lagerbuchung.ChargenCollection = new ChargenEintragCollection();
                        //l.Chargen.ForEach(c =>
                        //{
                        //    lagerbuchung.ChargenCollection.Add(new ChargenEintrag() { Charge = c.Chargenummer, Menge = c.Menge });
                        //});

                        //if (lagerbuchung.MengeLager != lagerbuchung.ChargenCollection.Sum(c => c.Menge))
                        //{
                        //    throw new LagerbuchungException("Fehlerhafte Angabe von Chargen");
                        //}

                    }


                    lagerbuchung.SerienNummernPflichtigkeit = artikelItem.Nachweispflicht > 2;
                    if (lagerbuchung.SerienNummernPflichtigkeit)
                    {
                        // Achung => nur Instaziieren, wenn die Lagerbuchung seriennummernpflichtig ist
                        lagerbuchung.SerienNummernCollection = new SeriennummernEintragCollection();
                        l.Seriennummern.ForEach(s =>
                        {
                            lagerbuchung.SerienNummernCollection.Add(new SeriennummernEintrag() { Seriennummer = s.Seriennummer });
                            SaveAdditionalSeriennummerInfo(s, l.Artikelnummer);
                        });

                        if (lagerbuchung.MengeLager != lagerbuchung.SerienNummernCollection.Count)
                        {
                            throw new LagerbuchungException("Fehlerhafte Angabe von Seriennummern");
                        }

                    }


                    // Validierung
                    var bestaendeAktualisieren = false;
                    if (!lagerbuchung.Validate(lagerplatzHerkunft, lagerplatzZiel, ref bestaendeAktualisieren, true, false))
                    {
                        throw new LagerbuchungException(lagerbuchung.Errors.GetDescriptionSummary());
                    }


                    // Speichern
                    if (!lagerbuchung.Save(ref lagerplatzHerkunft, ref lagerplatzZiel, job, false))
                    {
                        throw new LagerbuchungException(lagerbuchung.Errors.GetDescriptionSummary());
                    }
                    else
                    {
                        //Protokollierung:
                        //Im Fall einer erfolgreichen Lagerbuchungsspeicherung:
                        ProtokollHelper.AppendProtokollItem(_Mandant, ProtokollLogType.ImportLagerbuchungenLog, lagerbuchung.BuchungsHandle, string.Empty, lagerbuchung.MandantenJahr, string.Empty, string.Empty, string.Empty, lagerbuchung.Bewegungsdatum, string.Empty, 0, "Job " + job.JobHandle + "|Lagerbuchung " + lagerbuchung.BuchungsHandle, true, "Gebucht");

                    }
                });

                //ProtokollHelper.AppendProtokollItem(mandant, ProtokollLogType.ImportLagerbuchungenLog, lagerbuchung.BuchungsHandle, string.Empty, lagerbuchung.MandantenJahr, string.Empty, string.Empty, string.Empty, lagerbuchung.Bewegungsdatum, string.Empty, 0, "Job " + job.JobHandle + "|Lagerbuchung " + lagerbuchung.BuchungsHandle, true, "Gebucht");

                _Mandant.MainDevice.GenericConnection.CommitTransaction();
            }
            catch (LagerbuchungException ex)
            {
                _Mandant.MainDevice.GenericConnection.RollbackTransaction();
                ProtokollHelper.AppendProtokollItem(_Mandant, ProtokollLogType.ImportLagerbuchungenLog, 0, string.Empty, 0, string.Empty, string.Empty, string.Empty, null, string.Empty, 0, ex.Message, false, "Fehler");
                TraceLog.LogException(ex);
            }
            catch (Exception ex)
            {
                _Mandant.MainDevice.GenericConnection.RollbackTransaction();
                ProtokollHelper.AppendProtokollItem(_Mandant, ProtokollLogType.ImportLagerbuchungenLog, 0, string.Empty, 0, string.Empty, string.Empty, string.Empty, null, string.Empty, 0, ex.Message, false, "Fehler");
                TraceLog.LogException(ex);
            }
        }

    }
}
