using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace HomeMoneyParser
{
	internal class Record
	{
		[Name("date")]
		[Format("dd.mm.yyyy")]
		public DateTime Date { get; set; }
		[Name("account")]
		public string Account { get; set; }
		[Name("category")]
		public string Category { get; set; }
		[Name("total")]
		//[Format("G")]
		[CultureInfo("ru-RU")]
		public decimal Total { get; set; }
		[Name("currency")]
		public string Currency { get; set; }
		[Name("description")]
		[Optional]
		public string Description { get; set; }
		[Name("transfer")]
		[Optional]
		public string Transfer { get; set; }
	}

}
