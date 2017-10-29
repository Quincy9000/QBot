//using System;
//using System.Collections.Generic;
//using System.Data.SQLite;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;
//using Discord;
//using Discord.Commands;
//using Discord.WebSocket;
//using QuincyBot.RPG;
//using QuincyBot.RPG.Monsters;
//
//namespace QBot.Commands
//{
//    [Group("rpg")]
//    public class RpgCommands : ModuleBase
//    {
//        public static Dictionary<string, DiscordPlayer> Players { get; } = new Dictionary<string, DiscordPlayer>();
//
//        public static Dictionary<string, Battle> CurrentBattles { get; } = new Dictionary<string, Battle>();
//
//        internal static void SaveAllPlayers()
//        {
//            using(var c = new SQLiteCommand("Data Source=Data/Players.sqlite;Version=3;"))
//            {
//                c.Open();

//                using(SQLiteCommand deleteCom = new SQLiteCommand("DELETE FROM DiscordPlayers;", c))
//                {
//                    deleteCom.Prepare();
//                    deleteCom.ExecuteNonQuery();
//                }
//
//                using(SQLiteCommand command = new SQLiteCommand(c))
//                {
//                    foreach(var p in Players.Values)
//                    {
//                        command.CommandText =
//                            $"INSERT INTO DiscordPlayers([CreatorName], [Avatar], [Name], [Class], [Race], [Cuteness], [Physique], [Technique], [Mystique], [ExperiencePoints], [Money])" +
//                            $"Values('{p.CreatorName}', '{p.Avatar}', '{p.Name}', '{p.Class}', '{p.Race}', '{p.Cuteness}', '{p.Stats.Physique}', '{p.Stats.Technique}', '{p.Stats.Mystique}', '{p.Exp.Points}', '{p.Money}');";
//                        command.Prepare();
//                        command.ExecuteNonQuery();
//                    }
//                }
//            }
//        }
//
//        internal static void LoadAllPlayers()
//        {
//            try
//            {
//                using(var c = new SQLiteConnection("Data Source=Data/Players.sqlite;Version=3;"))
//                {
//                    c.Open();
//
//                    using(SQLiteCommand command = new SQLiteCommand("SELECT * FROM DiscordPlayers", c))
//                    {
//                        using(SQLiteDataReader reader = command.ExecuteReader())
//                        {
//                            while(reader.Read())
//                            {
//                                DiscordPlayer p = new DiscordPlayer
//                                {
//                                    CreatorName = reader["CreatorName"].ToString(),
//                                    Name = reader["Name"].ToString(),
//                                    Avatar = reader["Avatar"].ToString(),
//                                    Class = reader["Class"].ToString(),
//                                    Race = reader["Race"].ToString(),
//                                    Exp =
//                                    {
//                                        Points = int.Parse(reader["ExperiencePoints"].ToString())
//                                    },
//                                    Stats =
//                                    {
//                                        Physique = int.Parse(reader["Physique"].ToString()),
//                                        Technique = int.Parse(reader["Technique"].ToString()),
//                                        Mystique = int.Parse(reader["Mystique"].ToString())
//                                    }
//                                };
//                                var money = reader["Money"].ToString();
//                                if(string.IsNullOrEmpty(money))
//                                {
//                                    money = "100";
//                                }
//                                p.Money = int.Parse(money);
//                                Players.Add(p.Name, p);
//                            }
//                        }
//                    }
//                }
//            }
//            catch(Exception e)
//            {
//                Console.WriteLine(e);
//            }
//        }
//
//        [Command("newchar")]
//        [Summary("Add a new character to the game")]
//        public async Task NewChar(string name, string _class, string race)
//        {
//            if(Players.ContainsKey(name))
//            {
//                await ReplyAsync("Character already exists!");
//            }
//            else
//            {
//                var p = new DiscordPlayer(name, _class, race)
//                {
//                    CreatorName = Context.User.Username,
//                    Avatar = Context.User.GetAvatarUrl()
//                };
//                Players.Add(name, p);
//                await ReplyAsync("Added new character!");
//            }
//        }
//
//        [Command("allchars")]
//        [Summary("Lists all characters")]
//        [Alias("viewchars", "viewall")]
//        public async Task AllChars()
//        {
//            if(Players.Count > 0)
//                await ReplyAsync(string.Join(", ", Players.Select(w => w.Value.Name)));
//            else
//                await ReplyAsync("No characters exist!");
//        }
//
//        [Command("viewchar")]
//        [Summary("Display existing character")]
//        [Alias("sheet", "charsheet")]
//        public async Task ViewChar(string name)
//        {
//            var u = Context.User as SocketGuildUser;
//            if(u == null) return;
//            if(Players.TryGetValue(name, out DiscordPlayer p))
//            {
//                EmbedBuilder eb = new EmbedBuilder();
//                eb.WithAuthor(b =>
//                {
//                    b.Name = p.CreatorName;
//                    b.IconUrl = p.Avatar;
//                });
//                eb.WithDescription(
//                    $"**Character:** {p.Name}\n" +
//                    $"**Class:** {p.Class}\n" +
//                    $"**Race:** {p.Race}\n" +
//                    $"**Level:** {p.Level}\n" +
//                    $"**Physique:** {p.Stats.Physique}\n" +
//                    $"**Technique:** {p.Stats.Technique}\n" +
//                    $"**Mystique:** {p.Stats.Mystique}\n" +
//                    $"**Skillpoints:** {p.Exp.SkillPointsToSpend}\n" +
//                    $"**PointsNeededToLevel:** {p.Exp.RequiredPointsToLevel()}\n" +
//                    $"**Money:** {p.Money}\n");
//                eb.WithFooter(b =>
//                {
//                    b.IconUrl = Context.Client.CurrentUser.GetAvatarUrl();
//                    b.Text = "Character Sheet Generator by Quincy";
//                });
//                await ReplyAsync("", false, eb);
//            }
//            else
//                await ReplyAsync("Characer does not exist!");
//        }
//
//        [Command("genmon")]
//        [Summary("Creates random monster")]
//        public async Task GenMon(string monster = "")
//        {
//            Monster mon = null;
//
//            if(monster == "")
//                mon = MonsterGenerator.RandomMonster();
//            else if(!MonsterGenerator.TryGetMonsterFromTable(monster, out mon))
//            {
//                await ReplyAsync("Monster does not exist!");
//                return;
//            }
//
//            EmbedBuilder eb = new EmbedBuilder();
//            eb.WithAuthor(b => { b.Name = mon.Name; });
//            eb.WithDescription(
//                $"**Physique:** {mon.Stats.Physique}\n" +
//                $"**Technique:** {mon.Stats.Technique}\n" +
//                $"**Mystique:** {mon.Stats.Mystique}\n" +
//                $"**ExpDrops:** {mon.ExpDrop}\n");
//            await ReplyAsync("", false, eb);
//        }
//
//        [Command("newbattle")]
//        [Summary("Start a new battle with an enemy")]
//        public async Task Battle(string fighter)
//        {
//            var u = Context.User as SocketGuildUser;
//            if(u == null) return;
//            if(!CurrentBattles.ContainsKey(u.ToString()))
//            {
//                var b = (CurrentBattles[u.ToString()] = new Battle());
//	            b.User = u; 
//	            b.BattleOwner = Players[fighter];
//	            b.FightingMonster = MonsterGenerator.RandomMonster(); 
//				var channel = await Context.User.CreateDMChannelAsync();
//	            var msg = await channel.SendMessageAsync($"Battle started with {b.FightingMonster.Name}");
//                await msg.AddReactionAsync(new Emoji(":one:"));
//            }
//            else
//            {
//                await ReplyAsync("You might already have a battle going on that you *should* finish.");
//            }
//        }
//        
//        
//
//        [Command("help")]
//        [Summary("I will tell you about all of my commands.")]
//        public async Task Help()
//        {
//            EmbedBuilder eb = new EmbedBuilder();
//
//            MethodInfo[] mi = GetType().GetMethods();
//            for(int o = 0; o < mi.Length; o++)
//            {
//                CommandAttribute myAttribute1 = mi[o].GetCustomAttributes(true).OfType<CommandAttribute>().FirstOrDefault();
//                SummaryAttribute myAttribute2 = mi[o].GetCustomAttributes(true).OfType<SummaryAttribute>().FirstOrDefault();
//                if(myAttribute1 != null && myAttribute2 != null)
//                    eb.AddField(myAttribute1.Text, myAttribute2.Text);
//            }
//
//            await ReplyAsync("", false, eb);
//        }
//    }
//}