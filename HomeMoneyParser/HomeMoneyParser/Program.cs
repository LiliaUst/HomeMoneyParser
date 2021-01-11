using System;
using Autofac;
using CommandLine;
using HomeMoneyParser.Command;

namespace HomeMoneyParser
{
	class Program
	{
		static int Main(string[] args) {
			var result = Parser.Default.ParseArguments<GetWeeklyExpensesOptions>(args)
				.MapResult(
					(GetWeeklyExpensesOptions opts) => Resolve<GetWeeklyExpensesCommand>().Execute(opts),
					errs => 1);
			Console.WriteLine("Press any key to close this window...");
			Console.ReadLine();
			return result;
		}

		private static T Resolve<T>() {
			var container = new BindingsModule().Register();
			return container.Resolve<T>();
		}

	}
}
