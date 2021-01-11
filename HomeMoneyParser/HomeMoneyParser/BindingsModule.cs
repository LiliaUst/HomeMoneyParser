using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using HomeMoneyParser.Command;

namespace HomeMoneyParser
{
	internal class BindingsModule
	{
		public IContainer Register() {
			var containerBuilder = new ContainerBuilder();
			containerBuilder
				.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
				.AsImplementedInterfaces();
			
			containerBuilder.RegisterType<GetWeeklyExpensesCommand>();
			return containerBuilder.Build();
		}
	}
}
