using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandLine;
using HomeMoneyParser.Csv;

namespace HomeMoneyParser.Command
{

	[Verb("get-week-exp", HelpText = "Get weekly expenses")]
	internal class GetWeeklyExpensesOptions
	{
		[Option('f', "FilePath", Required = true, HelpText = "Input file path.")]
		public string FilePath { get; set; }

		[Option('d', "Date", Required = false, HelpText = "Input date in week.")]
		public DateTime? Date { get; set; }

		[Option('s', "DateFrom", Required = false, HelpText = "Date from.")]
		public DateTime? DateFrom { get; set; }

		[Option('t', "DateTo", Required = false, HelpText = "Date to.")]
		public DateTime? DateTo { get; set; }

		[Option('w', "WeekNumber", Required = false, HelpText = "Input week number of the year.")]
		public int? WeekNumber { get; set; }

		[Option('c', "ShowCategoryName", Default = false, Required = false, HelpText = "Show category name.")]
		public bool ShowCategoryName { get; set; }
	}

	internal class GetWeeklyExpensesCommand : Command<GetWeeklyExpensesOptions>
	{
		private IList<Record> GetWeekRecords(IList<Record> records, DateTime monday) {
			var sunday = monday.AddDays(7);
			return records.Where(x => x.Date >= monday && x.Date <= sunday).ToList();
		}

		private DateTime GetMonday(GetWeeklyExpensesOptions options) {
			DateTime dn = options.Date.HasValue ? options.Date.Value : DateTime.Today;
			return dn.AddDays(-(dn.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)dn.DayOfWeek) + 1);
		}

		private IList<Record> GetExpenses(IList<Record> records) =>
			records.Where(x => x.Total < 0 && string.IsNullOrEmpty(x.Transfer)).ToList();

		private int GetWeekNumber(DateTime dateInWeek) {
			var cal = new GregorianCalendar();
			return cal.GetWeekOfYear(dateInWeek, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
		}

		public override int Execute(GetWeeklyExpensesOptions options) {
			var csvParser = new CsvParserWrapper();
			var records = csvParser.Parse<Record>(options.FilePath);
			IList<Record> recordsInDate = null;
			int? weekNumber = null;
			DateTime dateFrom = DateTime.Today, dateTo = DateTime.Today;
			if (options.DateFrom.HasValue) {
				dateFrom = options.DateFrom.Value;
				dateTo = options.DateTo.HasValue ? options.DateTo.Value : DateTime.Today;
				recordsInDate = records.Where(x => x.Date >= dateFrom && x.Date <= dateTo).ToList();
			} else {
				DateTime monday = GetMonday(options);
				recordsInDate = GetWeekRecords(records, monday);
				weekNumber = GetWeekNumber(monday);
			}
			IList<Record> expenses = GetExpenses(recordsInDate);

			ExpenseCategoryList.Default.Categories.ForEach(category => {
				var total = expenses.Where(x => x.Category == category.Name).Select(x => x.Total).Sum();
				category.ChildCategories.ForEach(childCategory => {
					var total = expenses.Where(x => x.Category == $"{category.Name}\\{childCategory.Name}").Sum(x => x.Total);
					childCategory.Amount = Math.Abs(total);
				});
				category.Amount = Math.Abs(total) + category.ChildCategories.Sum(x => x.Amount);
			});

			if (weekNumber.HasValue) {
				Console.WriteLine($"Week number:\t{weekNumber}");
			} else {
				Console.WriteLine($"Date from:\t{dateFrom}, date to:\t{dateTo}");
			}

			Console.WriteLine($"Categories:");
			ExpenseCategoryList.Default.Categories.ForEach(category => {
				Console.WriteLine($"{GetPrintCategoryName(category.Name, options)}{GetPrintAmount(category.Amount)}");
				category.ChildCategories.ForEach(childCategory => {
					Console.WriteLine($"{GetPrintCategoryName(childCategory.Name, options)}{GetPrintAmount(childCategory.Amount)}");
				});
			});
			return 0;
		}

		private string GetPrintCategoryName(string name, GetWeeklyExpensesOptions options) =>
			options.ShowCategoryName ? $"{name}\t" : string.Empty;

		private decimal GetPrintAmount(decimal amount) =>
			decimal.Round(amount);
	}
}
