namespace QuincyBot.RPG
{
	public class DiscordPlayer
	{
		public string CreatorName { get; set; }

		public string Avatar { get; set; }

		public string Name { get; set; }

		public string Class { get; set; }

		public string Race { get; set; }

		public int Level => Exp.Level;

		public int Cuteness { get; set; }

		public int Money { get; set; }

		public Experience Exp { get; set; }

		public Attributes Stats { get; set; }

		public DiscordPlayer(string name, string _class, string race, int exp = 0)
		{
			Name = name;
			Class = _class;
			Race = race;
			Money = 100;
			Exp = new Experience(exp);
			Stats = new Attributes();
		}

		public DiscordPlayer()
		{
			Name = "";
			Class = "";
			Race = "";
			Money = 100;
			Exp = new Experience(0);
			Stats = new Attributes();
		}
	}
}
