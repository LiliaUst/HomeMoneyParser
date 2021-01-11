using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace HomeMoneyParser.Csv
{
	internal class CsvParserWrapper
	{
		public string Delimiter { get; set; } = ";";

		public IList<T> Parse<T>(string path) {
			using var reader = new StreamReader(path);
			using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
			csv.Configuration.Delimiter = Delimiter;
			var records = new List<T>();
			var isHeader = true;
			while (csv.Read()) {
				if (isHeader) {
					csv.ReadHeader();
					isHeader = false;
					continue;
				}
				var record = csv.GetRecord<T>();
				records.Add(record);
			}
			return records;
		}
	}
}
