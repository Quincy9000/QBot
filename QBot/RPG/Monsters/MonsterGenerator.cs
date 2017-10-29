using System.Linq;
using QuincyBot.RPG.Monsters;

namespace QBot.RPG.Monsters
{
	/// <summary>
	/// Monster functions 
	/// </summary>
	public static class MonsterGenerator
	{
		/// <summary>
		/// Gets a random monster from the database
		/// </summary>
		/// <returns></returns>
		public static Monster RandomMonster()
		{
//			var ds = DataAccess.FillDataSet("SELECT * FROM Enemies;", DataAccess.DataPath(@"Data/Enemies.db")).Result;
//			if(ds.Tables[0].Rows.Count > 0)
//				n = ds.Tables[0].Rows[Program.R.Next(0, ds.Tables[0].Rows.Count)]["Name"].ToString();
			var mList = Monster.Monsters.ToList();
			var mon = new Monster("");
			int r = Program.R.Next(0, Monster.Monsters.Count);
			mon.Name = mList[r].Value.Name;
			mon.Level = mList[r].Value.Level;
			mon.Stats.Physique = mList[r].Value.Physique;
			mon.Stats.Technique = mList[r].Value.Technique;
			mon.Stats.Mystique = mList[r].Value.Mystique;
			mon.ExpDrop = mList[r].Value.ExpDrop;
			return mon;
		}

		/// <summary>
		/// Returns true from the database if monster exists, if not then returns false and monster paramter is null
		/// </summary>
		/// <param name="name"></param>
		/// <param name="m"></param>
		/// <returns></returns>
		public static bool TryGetMonsterFromTable(string name, out Monster m)
		{
//			var ds = DataAccess.FillDataSet($"SELECT * FROM Enemies WHERE Name='{name}'", DataAccess.DataPath(@"Data/Enemies.db")).Result;
//			if(ds.Tables[0].Rows.Count > 0)
//			{
//				m = new Monster(ds.Tables[0].Rows[0]["Name"].ToString())
//				{
//					Level = int.Parse(ds.Tables[0].Rows[0]["Level"].ToString()),
//					Stats =
//					{
//						Physique = int.Parse(ds.Tables[0].Rows[0]["Physique"].ToString()),
//						Technique = int.Parse(ds.Tables[0].Rows[0]["Technique"].ToString()),
//						Mystique = int.Parse(ds.Tables[0].Rows[0]["Mystique"].ToString())
//					}
//				};
//				return true;
//			}
//			m = null;
			if(Monster.Monsters.TryGetValue(name, out MonsterTemplate mt))
			{
				m = new Monster(mt.Name);
				m.Level = mt.Level;
				m.ExpDrop = mt.ExpDrop;
				m.Stats.Physique = mt.Physique;
				m.Stats.Technique = mt.Technique;
				m.Stats.Mystique = mt.Mystique;
				return true;
			}
			m = null;
			return false;
		}
	}
}