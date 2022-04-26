using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.ImportDto
{
    public class SecLogin
    {

        public string User { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public short MandantId { get; set; }

        public bool NtLogin { get; set; }

    }
}
