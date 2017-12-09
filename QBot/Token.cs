namespace QBot
{
	static class Q
	{
		static Q() => Token = System.IO.File.ReadAllText("BotToken.txt");

		public static string Token { get; }
	}
}