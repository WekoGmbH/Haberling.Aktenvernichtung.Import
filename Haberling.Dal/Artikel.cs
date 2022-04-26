using Sagede.OfficeLine.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.Dal
{
    public class Artikel
    {

        public static string CreateArtikel(string artikelnummer, string artikelgruppe, Mandant mandant)
        {
            try
            {
                if (!string.IsNullOrEmpty(mandant.MainDevice.Lookup.GetString("Artikelnummer", "KHKArtikel", $"Artikelnummer = 'A{artikelnummer}' AND Mandant = {mandant.Id}", "")))
                {
                    return string.Concat("A", artikelnummer);
                }
                var artikel = mandant.MainDevice.Entities.Artikel.Create(string.Concat("A", artikelnummer), mandant.Id);
                artikel.Matchcode = string.Concat("A", artikelnummer);
                artikel.Basismengeneinheit = "Stk";
                artikel.Lagerfuehrung = true;
                artikel.Hauptartikelgruppe = artikelgruppe;
                artikel.Artikelgruppe = artikelgruppe;
                artikel.Bezeichnung1 = artikelnummer;
                artikel.Nachweispflicht = 3;
                artikel.Aktiv = true;
                artikel.Save(Sagede.OfficeLine.Data.Entities.EntitySaveOptions.FireAndForget);
                return artikel.Artikelnummer;
            }
            catch (Exception ex)
            {
                TraceLog.LogException(ex);
            }
            return string.Empty;
        }

    }
}
