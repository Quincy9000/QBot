using System;
using System.Data.SQLite;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace QBot
{
	class Program
	{
		static DiscordSocketClient Client;

		public static bool IsExit = false;

		/// <summary>
		/// Owner of the bot
		/// </summary>
		static SocketUser Quincy;

		CommandService Commands;

		public static Random R { get; } = new Random();

		public static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

		async Task Start()
		{
			using(Client = new DiscordSocketClient())
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
				while(!IsExit) await Task.Delay(TimeSpan.FromSeconds(1));

				await Client.LogoutAsync();
			}
		}

		static async Task DeleteGuildFromDb(SocketGuild socketGuild)
		{
			SQLiteCommand command = new SQLiteCommand()
			{
				CommandText = $"DROP TABLE IF EXISTS \"{socketGuild.Id}\";"
			};

			Console.WriteLine($"QBot left {socketGuild.Name}!");
			if(!await DataAccess.ExecuteCommand(DataAccess._con, command))
			{
				Console.WriteLine("uh oh");
			}
		}

		static async Task UserJoinedGuild(SocketGuildUser u)
		{
			SQLiteCommand c = new SQLiteCommand { CommandText = $"SELECT * FROM \"{u.Guild.Id}\" WHERE [UserID] = @id;"};
			c.Parameters.AddWithValue("@id", u.Id);
			var data = await DataAccess.FillDataSet(DataAccess._con, c);
			try
			{
				if (data.Tables[0].Rows.Count == 0)
				{
					SQLiteCommand command = new SQLiteCommand
					{
						CommandText = $"INSERT INTO \"{u.Guild.Id}\" ([UserID], [Name], [Nickname]) VALUES(@id, @name, @nickname)"
					};

					command.Parameters.AddWithValue("@id", u.Id);
					command.Parameters.AddWithValue("@name", u.Username);
					command.Parameters.AddWithValue("@nickname", u.Nickname);
					await DataAccess.ExecuteCommand(DataAccess._con, command);

					Console.WriteLine($"---- {u.Username} Joined {u.Guild.Name}!");
					command.Dispose();
				}
			}
			catch(Exception lol)
			{
				Console.WriteLine(lol.Message);
			}
		}

		static async Task UserLeftGuild(SocketGuildUser u)
		{
			SQLiteCommand command = new SQLiteCommand
			{
				CommandText = $"DELETE FROM \"{u.Guild.Id}\" WHERE [UserID] = @id;"
			};

			command.Parameters.AddWithValue("@id", u.Id);

			await DataAccess.ExecuteCommand(DataAccess._con, command);
			command.Dispose();

			Console.WriteLine($"Removed {u.Username} from {u.Guild.Name}!");
		}

		static async Task AddNewGuildToDb(SocketGuild s)
		{
			string dropLol = $"DROP TABLE IF EXISTS \"{s.Id}\"; ";
			SQLiteCommand command = new SQLiteCommand()
			{
				//Server Id as the table name
				CommandText =
					$"CREATE TABLE IF NOT EXISTS \"{s.Id}\" ( [UserID] INTEGER NOT NULL UNIQUE, [Name] TEXT NOT NULL, [Nickname] TEXT, PRIMARY KEY(`UserID`) );",
			};

			await DataAccess.ExecuteCommand(DataAccess._con, command);

			Console.WriteLine($"QBot joined new {s.Name}!");
			var users = s.Users;
			foreach (var u in users)
			{
				await UserJoinedGuild(u);
			}

			await Task.CompletedTask;
		}

		async Task InstallCommands()
		{
			Client.MessageReceived += MessageReceived;
			await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
		}

		async Task MessageReceived(SocketMessage msg)
		{
			if(msg is SocketUserMessage message)
			{
				int argPos = 0;
				if(!message.HasCharPrefix('q', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) ||
				   message.Author.IsBot) return;

				var context = new CommandContext(Client, message);

				if(message.HasStringPrefix("qre", ref argPos, StringComparison.CurrentCultureIgnoreCase))
				{
					string ree = "REE";
					foreach(var s in message.ToString())
					{
						if(char.ToLower(s) == 'e')
							ree += "EEEEEEEEEE";
					}
					await message.Channel.SendMessageAsync(ree);
				}

				var result = await Commands.ExecuteAsync(context, argPos);

				if(!result.IsSuccess)
					Console.WriteLine(result.ErrorReason);
			}
		}
	}
}