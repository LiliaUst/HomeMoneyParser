using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace HomeMoneyParser.Command
{
	[Verb("get-exp", HelpText = "Get expenses by specified dates")]
	internal class GetExpensesOptions
	{
		[Option('f', "FilePath", Required = true, HelpText = "Input file path.")]
		public string FilePath { get; set; }

		[Option('d', "Date", Required = false, HelpText = "Date.")]
		public DateTime? Date { get; set; }

		[Option('t', "DateTo", Required = false, HelpText = "Date to.")]
		public DateTime? DateTo { get; set; }

		[Option('c', "ShowCategoryName", Default = false, Required = false, HelpText = "Show category name.")]
		public bool ShowCategoryName { get; set; }
	}

	internal class GetExpensesCommand : Command<GetExpensesOptions>
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
			var monday = GetMonday(options);
			var expenses = GetExpenses(GetWeekRecords(records, monday));

			ExpenseCategoryList.Default.Categories.ForEach(category => {
				var total = expenses.Where(x => x.Category == category.Name).Select(x => x.Total).Sum();
				category.ChildCategories.ForEach(childCategory => {
					var total = expenses.Where(x => x.Category == $"{category.Name}\\{childCategory.Name}").Sum(x => x.Total);
					childCategory.Amount = Math.Abs(total);
				});
				category.Amount = Math.Abs(total) + category.ChildCategories.Sum(x => x.Amount);
			});
			var weekNumber = GetWeekNumber(monday);

			Console.WriteLine($"Week number:\t{weekNumber}");
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
