using CsvHelper;
using CsvHelper.Configuration;
using Haberling.ImportDto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haberling.Csv
{
    public static class ContainerReader
    {

        public static List<ContainerDto> Read(string file)
        {
            var result = new List<ContainerDto>();
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture);
            cfg.Encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            //cfg.ShouldSkipRecord = row => row.Record[2].StartsWith("Typ 7");
            cfg.Delimiter = @";";
            using (var reader = new StreamReader(file, Encoding.GetEncoding("ISO-8859-1")))
            using (var csv = new CsvReader(reader, cfg))
            {
                
                csv.Read();
                csv.ReadHeader();
                var lastKunde = "";
                var lastMatchcode = "";
                while (csv.Read())
                {

                    if (csv.GetField<string>(2).StartsWith("Typ 7"))
                    {
                        csv.Read();
                        continue;
                    }

                    if (!string.IsNullOrEmpty(csv.GetField<string>(0)))
                    {
                        //Neuer Datensatz
                        lastKunde = csv.GetField<string>(0);
                        lastMatchcode = csv.GetField<string>(1);
                        continue;
                    }

                    var record = new ContainerDto();
                    record.Kunde = lastKunde;
                    record.Name = lastMatchcode;
                    record.Jahr = csv.GetField<int>(3);
                    record.Datum = DateTime.Parse(csv.GetField<string>(7));
                    record.Ct2 = csv.GetField<string>(6);
                    record.Typ = csv.GetField<string>(5);
                    record.Container = string.Concat("Typ ", csv.GetField(8).Split('/')[1].Trim());
                    record.Seriennummer = csv.GetField(8).Replace("/","-").Replace("Cont: ", "").Trim();
                    record.Strasse = csv.GetField<string>(9);
                    record.Erfassungsnummer = Convert.ToInt32(csv.GetField<string>(4));
                    result.Add(record);
                }
            }
            return result;
        }

    }


}
