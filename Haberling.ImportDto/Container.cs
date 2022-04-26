using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.ImportDto
{
    public class Container
    {

        public int Id { get; set; }

        public int VorgangId { get; set; }

        public string Artikelnummer { get; set; }

        public string Seriennummer { get; set; }

        public int Sicherheitsstufe { get; set; }

        public string Fraktion { get; set; }

        public string Schutzklasse { get; set; }

        public string Me { get; set; }


        public int Typ { get; set; }
    }
}
