using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.ImportDto
{
    public class Vorgang
    {

        public Vorgang()
        {
            Container = new List<Container>();
        }

        public int Adresse { get; set; }

        public int Id { get; set; }

        public string Status { get; set; }

        public string Phase { get; set; }

        public string Bemerkung { get; set; }

        public DateTime Datum { get; set; }

        public DateTime Erfassungsdatum { get; set; }

        public string Art { get; set; }

        public string Bearbeiter { get; set; }

        public List<Container> Container { get; set; }
    }
}
