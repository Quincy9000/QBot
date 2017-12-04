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
					Console.WriteLine("Ready!");
					await Task.CompletedTask;
				};
				
				Client.LeftGuild += DeleteGuildFromDb;

				Client.JoinedGuild += AddNewGuildToDb;
				
				Client.UserJoined += async (s) => { await Task.CompletedTask; };

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

			Console.WriteLine($"Left Guild! {socketGuild.Name}");
			if(!await DataAccess.ExecuteCommand(DataAccess._con, command))
			{
				Console.WriteLine("uh oh");
			}
		}

		static async Task AddNewGuildToDb(SocketGuild s)
		{
			SQLiteCommand command = new SQLiteCommand()
			{
				//Server Id as the table name
				CommandText =
					$"DROP TABLE IF EXISTS \"{s.Id}\"; CREATE TABLE IF NOT EXISTS \"{s.Id}\" ( [UserID] INTEGER, [Name] TEXT, [Nickname] TEXT, PRIMARY KEY(`UserID`) );",
			};

			await DataAccess.ExecuteCommand(DataAccess._con, command);

			command.CommandText = $"INSERT INTO \"{s.Id}\" ([UserID], [Name], [Nickname]) VALUES(@id, @name, @nickname)";

			var users = s.Users;
			foreach(var u in users)
			{
				command.Parameters.AddWithValue("@id", u.Id);
				command.Parameters.AddWithValue("@name", u.Username);
				command.Parameters.AddWithValue("@nickname", u.Nickname);
				await DataAccess.ExecuteCommand(DataAccess._con, command);
				command.Parameters.Clear();
			}

			Console.WriteLine($"Joined new Server! {s.Name}");
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