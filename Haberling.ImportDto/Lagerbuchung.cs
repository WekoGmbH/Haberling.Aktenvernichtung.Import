using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.ImportDto
{
    public class Lagerbuchung
    {

        public Lagerbuchung()
        {
            Seriennummern = new List<SeriennummerItem>();
        }

        /// <summary>
        /// Artikelnummer Sage
        /// </summary>
        public string Artikelnummer { get; set; }

        /// <summary>
        /// Variante
        /// </summary>
        public int AuspraegungId { get; set; }

        public decimal Menge { get; set; }


        public LagerbuchungartType Buchungstyp { get; set; }

        /// <summary>
        /// Hauptlagerplatz
        /// </summary>
        public string LagerkennungHLP { get; set; }

        /// <summary>
        /// Ziellagerplatz
        /// </summary>
        public string LagerkennungZLP { get; set; }

        public decimal Einstandspreis { get; set; }

        /// <summary>
        /// Zu buchende Seriennummern
        /// </summary>
        public List<SeriennummerItem> Seriennummern { get; set; }
    }
}
