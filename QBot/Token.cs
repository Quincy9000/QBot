namespace QBot
{
    internal static class Q
    {
        static Q() => Token = System.IO.File.ReadAllText("BotToken");

        public static string Token { get; }
    }
}