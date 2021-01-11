using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HomeMoneyParser.Dto;

namespace HomeMoneyParser
{
	internal class ExpenseCategoryList
	{
		public List<ExpenseCategory> Categories { get; set; } = new List<ExpenseCategory>();

		public static ExpenseCategoryList Default { get; private set; }

		static ExpenseCategoryList() {
			InitializeCategories();
		}

		private static void InitializeCategories() {
			Default = new ExpenseCategoryList();
			using var reader = new StreamReader("DB/categories.txt");
			ExpenseCategory parentCategory = null;
			while (reader.Peek() >= 0) {
				var name = reader.ReadLine();
				if (!name.StartsWith('\t')) {
					parentCategory = new ExpenseCategory {
						Name = name
					};
					Default.Categories.Add(parentCategory);
				} else {
					name = name.TrimStart('\t');
					var category = new ExpenseCategory {
						Name = name
					};
					parentCategory?.ChildCategories.Add(category);
				}
			}

		}

	}
}
