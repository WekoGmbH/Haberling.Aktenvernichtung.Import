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
    public static class MandantManager
    {
        public static Mandant CreateMandant(string user, string password, string database, short mandantId)
        {
            var pwc = new NamePasswordCredential(user, password);
            var session = ApplicationEngine.CreateSession(database, ApplicationToken.System, null, pwc);
            try
            {
               return session.CreateMandant(mandantId);
            }
            catch (Exception ex)
            {
                TraceLog.LogException(ex);
            }
            return null;
        }

    }
}
