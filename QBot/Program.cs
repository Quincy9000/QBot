using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;

namespace QBot
{
	class Program
	{
		static DiscordSocketClient Client;

		const string DatabaseName = "guilds.db";

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
					Console.WriteLine($"In {Client.Guilds.Count} guilds!");
					foreach(var guild in Client.Guilds)
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

		static async Task DeleteGuildFromDb(SocketGuild s)
		{
			try
			{
				using(var db = new LiteDatabase(DatabaseName))
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

		static async Task UserJoinedGuild(SocketGuildUser u)
		{
			try
			{
				using(var db = new LiteDatabase(DatabaseName))
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
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			await Task.CompletedTask;
		}

		static async Task UserLeftGuild(SocketGuildUser u)
		{
			try
			{
				using(var db = new LiteDatabase(DatabaseName))
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

		static async Task AddNewGuildToDb(SocketGuild s)
		{
			try
			{
				using(var db = new LiteDatabase(DatabaseName))
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
			foreach(var u in users)
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
				if(!message.HasCharPrefix('q', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos)
				   || message.Author.IsBot) return;

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