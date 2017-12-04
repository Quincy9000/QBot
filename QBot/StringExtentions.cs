namespace QBot
{
	public static class StringExtentions
	{
		public static string ToLower(this string msg)
		{
			string newMsg = "";
			foreach(var c in msg)
				newMsg += char.ToLower(c);
			return newMsg;
		}
	}
}