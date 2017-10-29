using Discord.WebSocket;
using QuincyBot.RPG.Monsters;

namespace QuincyBot.RPG
{
    /// <summary>
    /// Contains all the current battle info for a DiscordPlayer and a Monster
    /// </summary>
    public class Battle
    {
		public SocketUser User { get; set; }

        public DiscordPlayer BattleOwner { get; set; }
        
        public Monster FightingMonster { get; set; }
    }
}