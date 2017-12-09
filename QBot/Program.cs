using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Newtonsoft.Json.Bson;

namespace QBot
{
	static class Program
	{
		/// <summary>
		/// Client for discord
		/// </summary>
		static DiscordSocketClient _client;

		/// <summary>
		/// File used for database interactions
		/// </summary>
		public const string DatabaseConnectionString = "guilds.db";

		/// <summary>
		/// Change this to true when you want the bot to stop
		/// </summary>
		public static bool IsExit = false;

		/// <summary>
		/// Change this to true when you want to reboot the bot
		/// </summary>
		public static bool IsReboot = false;
		
		/// <summary>
		/// Owner of the bot
		/// </summary>
		public static SocketUser Owner { get; private set; }
		
		/// <summary>
		/// The bot itself
		/// </summary>
		public static SocketUser Bot { get; private set; }

		static CommandService _commands;

		public static int Latency => _client.Latency;

		public static IEnumerable<string> Split(string str, int chunkSize) => Enumerable.Range(0, str.Length / chunkSize)
			.Select(i => str.Substring(i * chunkSize, chunkSize));

		public static void Main(string[] args) => Start().GetAwaiter().GetResult();

		public static async Task Start()
		{						
			do
			{
				IsReboot = false;
				IsExit = false;
				using(_client = new DiscordSocketClient())				
				{
					_commands = new CommandService();

					_client.Connected += async () =>
					{
						Console.WriteLine("Connected!");
						await Task.CompletedTask;
					};

					_client.Ready += async () =>
					{
						//replace with your id
						Owner = _client.GetUser(66162202129739776);
						//replace with your bot id
						Bot = _client.GetUser(315370917825871872);
						Console.WriteLine($"In {_client.Guilds.Count} guilds!");
						//Check for users that could have joined when the bot was offline
						foreach(var guild in _client.Guilds)
							await AddNewGuildToDb(guild);
						Console.WriteLine("Ready!");
						await Task.CompletedTask;
					};

					_client.LeftGuild += DeleteGuildFromDb;
					_client.JoinedGuild += AddNewGuildToDb;

					_client.UserJoined += UserJoinedGuild;
					_client.UserLeft += UserLeftGuild;

					await _client.LoginAsync(TokenType.Bot, Q.Token);
					await _client.StartAsync();

					await InstallCommands();

					//Wait in 1 second intervals to check if IsExit changed
					while(!IsExit) await Task.Delay(TimeSpan.FromSeconds(1));

					await _client.LogoutAsync();
				}
			}
			while(IsReboot);
		}

		static async Task DeleteGuildFromDb(SocketGuild s)
		{
			try
			{
				using(var db = new LiteDatabase(DatabaseConnectionString))
				{
					if(db.CollectionExists($"{s.Id}"))
					{
						db.DropCollection($"{s.Id}");
						Console.WriteLine($"Bot left {s.Name}!");
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
				using(var db = new LiteDatabase(DatabaseConnectionString))
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
				using(var db = new LiteDatabase(DatabaseConnectionString))
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
				using(var db = new LiteDatabase(DatabaseConnectionString))
				{
					db.GetCollection<GuildMember>($"{s.Id}");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			Console.WriteLine($"Bot joined {s.Name}!");
			var users = s.Users;
			foreach(var u in users)
			{
				await UserJoinedGuild(u);
			}

			await Task.CompletedTask;
		}

		static async Task InstallCommands()
		{
			_client.MessageReceived += MessageReceived;
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
		}

		static async Task MessageReceived(SocketMessage msg)
		{
			if(msg is SocketUserMessage message)
			{
				int argPos = 0;
				if(!message.HasCharPrefix('q', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)
				   || message.Author.IsBot) return;

				var context = new CommandContext(_client, message);

				if(message.HasStringPrefix("qree", ref argPos, StringComparison.CurrentCultureIgnoreCase))
				{
					string ree = "REE";
					foreach(var s in message.ToString())
					{
						if(char.ToLower(s) == 'e')
							ree += "EEEEEEEEEE";
					}
					await message.Channel.SendMessageAsync(ree);
				}

				var result = await _commands.ExecuteAsync(context, argPos);

				if(!result.IsSuccess)
					Console.WriteLine(result.ErrorReason);
			}
		}
	}
}