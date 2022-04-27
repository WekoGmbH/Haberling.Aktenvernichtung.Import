using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.ImportDto
{
    public class ItemSerialsForLargerbuchung
    {

        public ItemSerialsForLargerbuchung()
        {

        }

        public ItemSerialsForLargerbuchung(string artikelnummer, string seriennummer)
        {
            Artikelnummer = artikelnummer;
            Seriennummer = seriennummer;
        }

        public string Artikelnummer { get; set; }

        public string Seriennummer { get; set; }
    }
}
