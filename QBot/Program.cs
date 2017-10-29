using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace QBot
{
	class Program
	{
		public static DiscordSocketClient Client;

		public static bool IsExit = false;

		CommandService Commands;

		public static Random R { get; } = new Random();

		/// <summary>
		/// Owner of the bot
		/// </summary>
		public static SocketUser Quincy;

		public static void Main(string[] args) => new Program().Start().Wait();

		public async Task Start()
		{
			using (Client = new DiscordSocketClient())
			{
				Commands = new CommandService();

				Client.Ready += async () =>
				{
					Quincy = Client.GetUser(66162202129739776);
					Console.WriteLine("Bot fully operational!");
					await Task.CompletedTask;
				};

				Client.Disconnected += async c =>
				{
					Console.WriteLine("Bot shutting down");
					await Task.CompletedTask;
				};

				await Client.LoginAsync(TokenType.Bot, Q.Token);
				await Client.StartAsync();

				await InstallCommands();

				//Wait in 1 second intervals to check if IsExit changed
				while (!IsExit) await Task.Delay(TimeSpan.FromSeconds(1));

				await Client.LogoutAsync();
			}
		}

		async Task InstallCommands()
		{
			Client.MessageReceived += MessageReceived;
			await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
		}

		async Task MessageReceived(SocketMessage msg)
		{
			if (msg is SocketUserMessage message)
			{
				int argPos = 0;
				if (!message.HasCharPrefix('q', ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos) ||
				    message.Author.IsBot) return;

				var context = new CommandContext(Client, message);

				if (message.HasStringPrefix("qree", ref argPos))
				{
					string ree = "REE";
					foreach (var s in message.ToString())
					{
						if (char.ToLower(s) == 'e')
							ree += "EEEEEEEE";
					}
					await context.Channel.SendMessageAsync(ree);
				}

				var result = await Commands.ExecuteAsync(context, argPos);
				
				if(!result.IsSuccess)
					Console.WriteLine(result.ErrorReason);
			}
		}
	}
}