using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;

namespace QBot
{
    internal class Program
    {
        static DiscordSocketClient Client;

		const string DatabaseName = "guilds.db";

        public static bool IsExit = false;

        /// <summary>
        /// Owner of the bot
        /// </summary>
        private static SocketUser Quincy;

        private CommandService Commands;

        public static Random R { get; } = new Random();

        public static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        private async Task Start()
        {
            using (Client = new DiscordSocketClient())
            {
                Commands = new CommandService();

                Client.Connected += async () =>
                {
                    Console.WriteLine("Connected!");
                    await Task.CompletedTask;
                };

                Client.Ready += async () =>
                {
                    Quincy = Client.GetUser(66162202129739776);
					Console.WriteLine($"In {Client.Guilds.Count} guilds!");
                    foreach (var guild in Client.Guilds)
                        await AddNewGuildToDb(guild);
                    Console.WriteLine("Ready!");
                    await Task.CompletedTask;
                };

                Client.LeftGuild += DeleteGuildFromDb;
                Client.JoinedGuild += AddNewGuildToDb;

                Client.UserJoined += UserJoinedGuild;
                Client.UserLeft += UserLeftGuild;

                await Client.LoginAsync(TokenType.Bot, Q.Token);
                await Client.StartAsync();

                await InstallCommands();

                //Wait in 1 second intervals to check if IsExit changed
                while (!IsExit) await Task.Delay(TimeSpan.FromSeconds(1));

                await Client.LogoutAsync();
            }
        }

        private static async Task DeleteGuildFromDb(SocketGuild s)
        {
			#region delete
			//SQLiteCommand command = new SQLiteCommand()
			//{
			//    CommandText = $"DROP TABLE IF EXISTS \"{socketGuild.Id}\";"
			//};

			//Console.WriteLine($"QBot left {socketGuild.Name}!");
			//if (!await DataAccess.ExecuteCommand(DataAccess._con, command))
			//{
			//    Console.WriteLine("uh oh");
			//}
			#endregion

			try
			{
				using (var db = new LiteDatabase(DatabaseName))
				{
					if(db.CollectionExists($"{s.Id}"))
					{
						db.DropCollection($"{s.Id}");
						Console.WriteLine($"QBot left {s.Name}!");
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			await Task.CompletedTask;
        }

        private static async Task UserJoinedGuild(SocketGuildUser u)
        {
			#region joined
			//SQLiteCommand c = new SQLiteCommand { CommandText = $"SELECT * FROM \"{u.Guild.Id}\" WHERE [UserID] = @id;" };
			//c.Parameters.AddWithValue("@id", u.Id);
			//var data = await DataAccess.FillDataSet(DataAccess._con, c);
			//try
			//{
			//    if (data.Tables[0].Rows.Count == 0)
			//    {
			//        SQLiteCommand command = new SQLiteCommand
			//        {
			//            CommandText = $"INSERT INTO \"{u.Guild.Id}\" ([UserID], [Name], [Nickname]) VALUES(@id, @name, @nickname)"
			//        };

			//        command.Parameters.AddWithValue("@id", u.Id);
			//        command.Parameters.AddWithValue("@name", u.Username);
			//        command.Parameters.AddWithValue("@nickname", u.Nickname);
			//        await DataAccess.ExecuteCommand(DataAccess._con, command);

			//        Console.WriteLine($"---- {u.Username} Joined {u.Guild.Name}!");
			//        command.Dispose();
			//    }
			//}
			//catch (Exception lol)
			//{
			//    Console.WriteLine(lol.Message);
			//}
			#endregion

			try
			{
				using (var db = new LiteDatabase(DatabaseName))
				{
					var collection = db.GetCollection<GuildMember>($"{u.Guild.Id}");

					//if we cant find the id in the database we add it
					if(!collection.Exists(x => x.UniqueId == u.Id))
					{
						var member = new GuildMember()
						{
							UserName = u.Username,
							NickName = u.Nickname,
							Server = u.Guild.Id,
							UniqueId = u.Id,
						};

						collection.Insert(member);

						Console.WriteLine($"---- {u.Username} Joined {u.Guild.Name}!");
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			await Task.CompletedTask;
        }

        private static async Task UserLeftGuild(SocketGuildUser u)
        {
			#region userLeft
			//SQLiteCommand command = new SQLiteCommand
			//{
			//    CommandText = $"DELETE FROM \"{u.Guild.Id}\" WHERE [UserID] = @id;"
			//};

			//command.Parameters.AddWithValue("@id", u.Id);

			//await DataAccess.ExecuteCommand(DataAccess._con, command);
			//command.Dispose();
			#endregion

			try
			{
				using (var db = new LiteDatabase(DatabaseName))
				{
					if(db.CollectionExists($"{u.Guild.Id}"))
					{
						var collection = db.GetCollection<GuildMember>($"{u.Guild.Id}");

						if(collection.Exists(x => x.UniqueId == u.Id))
						{
							collection.Delete(u.Id);
							Console.WriteLine($"Removed {u.Username} from {u.Guild.Name}!");
						}
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			await Task.CompletedTask;
        }

        private static async Task AddNewGuildToDb(SocketGuild s)
        {
			#region add
			//string dropLol = $"DROP TABLE IF EXISTS \"{s.Id}\"; ";
			//SQLiteCommand command = new SQLiteCommand()
			//{
			//    //Server Id as the table name
			//    CommandText =
			//        $"CREATE TABLE IF NOT EXISTS \"{s.Id}\" ( [UserID] INTEGER NOT NULL UNIQUE, [Name] TEXT NOT NULL, [Nickname] TEXT, PRIMARY KEY(`UserID`) );",
			//};

			//await DataAccess.ExecuteCommand(DataAccess._con, command);
			#endregion

			try
			{
				using (var db = new LiteDatabase(DatabaseName))
				{
					db.GetCollection<GuildMember>($"{s.Id}");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			Console.WriteLine($"QBot joined new {s.Name}!");
			var users = s.Users;
			foreach (var u in users)
			{
				await UserJoinedGuild(u);
			}

			await Task.CompletedTask;
        }

        private async Task InstallCommands()
        {
            Client.MessageReceived += MessageReceived;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task MessageReceived(SocketMessage msg)
        {
            if (msg is SocketUserMessage message)
            {
                int argPos = 0;
                if (!message.HasCharPrefix('q', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos)
                   || message.Author.IsBot) return;

                var context = new CommandContext(Client, message);

                if (message.HasStringPrefix("qre", ref argPos, StringComparison.CurrentCultureIgnoreCase))
                {
                    string ree = "REE";
                    foreach (var s in message.ToString())
                    {
                        if (char.ToLower(s) == 'e')
                            ree += "EEEEEEEEEE";
                    }
                    await message.Channel.SendMessageAsync(ree);
                }

                var result = await Commands.ExecuteAsync(context, argPos);

                if (!result.IsSuccess)
                    Console.WriteLine(result.ErrorReason);
            }
        }
    }
}