using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.ImportDto
{
        /// <summary>
        /// Art des auszuführenden Tasks
        /// </summary>
        public enum ProcessingTaskType
        {
            /// <summary>
            /// Undefined
            /// </summary>
            Undefined = 0,

            /// <summary>
            /// Export von Verkaufsbelegen
            /// </summary>
            ExportVKBelege = 1,

            /// <summary>
            /// Import von Verkaufsbelegen
            /// </summary>
            ImportVKBelege = 2,

            /// <summary>
            /// Verabeitung offene Aufträge zu Lieferscheinen (vorgangsbezogen)
            /// </summary>
            ProcessOffeneVVAs = 3,

            /// <summary>
            /// Import von Lagerbewegungen
            /// </summary>
            ImportLagerbuchungen = 4,

            /// <summary>
            /// Import von Buchungen im Rechnungswesen (Sachkonto an Sachkonto)
            /// </summary>
            ImportBuchungen = 5,

            /// <summary>
            /// Import von Rechnungen in der Finanzbuchhaltung über die Klasse Rechnung
            /// </summary>
            ImportFibuRechnungen = 6
        }

        /// <summary>
        /// LogTypes für tKHKProtokoll
        /// </summary>
        public enum ProtokollLogType
        {
            /// <summary>
            /// ExportVKBelegeLog
            /// </summary>
            ExportVKBelegeLog = 10050,

            /// <summary>
            /// ImportVKBelegeLog
            /// </summary>
            ImportVKBelegeLog = 10051,

            /// <summary>
            /// ProcessOffeneVVAsLog
            /// </summary>
            ProcessOffeneVVAsLog = 10052,

            /// <summary>
            /// ImportLagerbuchungenLog
            /// </summary>
            ImportLagerbuchungenLog = 10053,

            /// <summary>
            ///ImportBuchungenLog
            /// </summary>
            ImportBuchungenLog = 10054,

            /// <summary>
            /// ImportFibuRechnungenLog
            /// </summary>
            ImportFibuRechnungenLog = 10055
        }

        /// <summary>
        /// Lagerbewegungsart für Lagerbuchungen
        /// </summary>
        public enum LagerbuchungartType
        {
            /// <summary>
            /// Undefined
            /// </summary>
            Undefined = 0,

            /// <summary>
            /// Interne Umbuchung
            /// </summary>
            IU = 1,

            /// <summary>
            /// Manueller Zugang
            /// </summary>
            ZM = 2,

            /// <summary>
            /// Manuelle Entnahme
            /// </summary>
            EM = 3
        }
}
