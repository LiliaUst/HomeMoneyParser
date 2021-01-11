using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMoneyParser.Dto
{
	internal class ExpenseCategory
	{
		public string Name { get; set; }

		public decimal Amount { get; set; } = 0;

		public List<ExpenseCategory> ChildCategories { get; set; } = new List<ExpenseCategory>();

	}

}
