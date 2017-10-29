using System;

namespace QuincyBot.RPG
{
	public class Experience
	{
		/// <summary>
		/// Experience Points that the player currently has
		/// </summary>
		public int Points
		{
			get { return points; }
			set
			{
				points += value;
				//check to see if theres a level up after receiveing points
				int tryL = TryLevel(points);
				if(tryL >= Level)
				{
					for(int i = tryL - Level; i > 0; i--)
						SkillPointsToSpend += 2;
					Level = tryL;
				}
			}
		}

		public int RequiredPointsToLevel()
		{
			try
			{
				return levels[Level] - points;
			}
			catch(Exception e)
			{
				return 0;
			}
		}

		public bool IsLevelUpAvailable => true;

		public int SkillPointsToSpend { get; private set; }

		int points;

		public int Level { get; private set; }

		/// <summary>
		/// Checks if the exp earned is enough to get receive a level up
		/// </summary>
		/// <param name="exp"></param>
		/// <returns></returns>
		int TryLevel(int exp)
		{
			int l = 0;
			foreach(var i in levels)
			{
				if(exp >= i)
					l++;
				//Console.WriteLine($"Level: {l}");
			}
			return l;
		}

		int maxLevel => levels.Length;

		readonly int[] levels =
		{
			0,
			100,
			300,
			600,
			1000,
			2000,
			5000,
			10000,
			20000,
			50000,
			120000
		};

		public Experience(int exp)
		{
			Level = 1;
			Points = exp;
		}
	}
}