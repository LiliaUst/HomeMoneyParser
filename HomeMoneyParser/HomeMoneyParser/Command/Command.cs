namespace HomeMoneyParser.Command
{
	public abstract class Command<T>
	{
		public abstract int Execute(T options);
	}
}
