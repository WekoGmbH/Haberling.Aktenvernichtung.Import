using CsvHelper;
using CsvHelper.Configuration;
using Haberling.ImportDto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Haberling.Csv
{
    public static class SerialNumberCsvReader
    {

        public static List<SerialNumberCsvDto> Read(string file, string artikel, int auspraegung)
        {
            var result = new List<SerialNumberCsvDto>();
            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture);
            cfg.Encoding = Encoding.GetEncoding("ISO-8859-1");
            cfg.Delimiter = @";";
            using (var reader = new StreamReader(file, Encoding.GetEncoding("ISO-8859-1")))
            using (var csv = new CsvReader(reader, cfg))
            {              
                csv.Read();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new SerialNumberCsvDto();
                    record.Number = csv.GetField<int>(0);
                    record.Schloss = csv.GetField<string>(1);
                    record.Material = csv.GetField<string>(2);
                    record.Serial = csv.GetField<string>(3);
                    result.Add(record);
                }
            }
            return result;
        }

    }


}
