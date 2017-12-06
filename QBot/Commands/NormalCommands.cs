using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace QBot.Commands
{
	/// <summary>
	/// Commands for everyday use
	/// </summary>
	public class NormalCommands : ModuleBase
	{
		[Command("help")]
		[Summary("Lists all the available commands that the bot can currently do")]
		public async Task Help()
		{
			var eb = new EmbedBuilder();

			var mi = GetType().GetMethods();
			for(var o = 0; o < mi.Length; o++)
			{
				var myAttribute1 = mi[o].GetCustomAttributes(true).OfType<CommandAttribute>().FirstOrDefault();
				var myAttribute2 = mi[o].GetCustomAttributes(true).OfType<SummaryAttribute>().FirstOrDefault();
				if(myAttribute1 != null && myAttribute2 != null)
					eb.AddField(myAttribute1.Text, myAttribute2.Text);
			}

			await ReplyAsync("", false, eb);
		}

		[Command("ping")]
		[Summary("Gets latency to server")]
		public async Task Ping()
		{
			await ReplyAsync($"QPong! {Program.Latency}ms");
		}

		[Command("userinfo")]
		[Summary("Returns info about the current user, or the user parameter, if one passed.")]
		[Alias("user", "whois")]
		public async Task UserInfo([Summary("The (optional) user to get info for")] IUser u = null)
		{
			var eb = new EmbedBuilder();
			if(!(u is SocketGuildUser user)) return;

			var g = "";
			if(user.Game.HasValue)
				g = $"\n**Game:** {user.Game.Value}";
			if(!string.IsNullOrEmpty(user.GetAvatarUrl()))
			{
				var url = user.GetAvatarUrl();
				eb.WithAuthor(b =>
				{
					b.Name = user.Username;
					b.IconUrl = url;
				});
			}
			var bot = user.IsBot ? "**Bot:** BEEP BOOP NOT A BOT" : "**Human:** I AM A HUMAN";
			var roles = "";
			foreach(var r in user.Roles)
				if(r.Name != "@everyone")
					roles += $"{r.Name}, ";
			eb.Description =
				$"**Nickname:** {user.Nickname}\n**ID:** {user.Id}\n**Date Joined:** {user.CreatedAt.DateTime}{g}\n**Status:** {user.Status}\n{bot}\n**Roles:** {roles}";
			eb.WithColor(new Color(Program.R.Next(0, 256), Program.R.Next(0, 256), Program.R.Next(0, 256)));
			await ReplyAsync("", false, eb);
		}

		[Command("getav")]
		[Summary("Gets the avatar of the user")]
		[Alias("av")]
		public async Task GetAvatar(IUser u)
		{
			await ReplyAsync(u.GetAvatarUrl());
		}

		[Command("bigtext")]
		[Summary("Says word in huge text")]
		[Alias("text", "bt")]
		public async Task BigText([Remainder] string word)
		{
			await Context.Message.DeleteAsync();

			var numbers = new Dictionary<char, string>
			{
				['1'] = "one",
				['2'] = "two",
				['3'] = "three",
				['4'] = "four",
				['5'] = "five",
				['6'] = "six",
				['7'] = "seven",
				['8'] = "eight",
				['9'] = "nine",
				['0'] = "zero"
			};

			var msg = "";
			for(var i = 0; i < word.Length; i++)
				if(char.IsDigit(word[i]))
					msg += $":{numbers[word[i]]}:";
				else if(char.ToLower(word[i]) == 'b' || char.ToLower(word[i]) == 'g')
					msg += $":b:";
				else if(char.IsLetter(word[i]))
					msg += $":regional_indicator_{char.ToLower(word[i])}:";
				else if(word[i] == '!')
					msg += ":exclamation:";
				else if(char.IsWhiteSpace(word[i]))
					msg += "  ";
				else
					msg += $"{word[i]}";

			if(msg.Length > 2000)
			{
				var f = Program.Split(msg, 1999);

				foreach(var a in f)
					await ReplyAsync(a);
			}
			else
			{
				await ReplyAsync(msg);
			}
		}


		[Command("delete")]
		[Summary("Deletes all the messages from a user N number of times")]
		[RequireUserPermission(GuildPermission.ManageMessages)]
		[RequireBotPermission(ChannelPermission.ManageMessages)]
		public async Task Delete(IUser user, int n = 1)
		{
			if(!(user is SocketGuildUser u)) return;

			var a = Context.Channel.GetMessagesAsync(n).Flatten().Result.Where(m => m.Author.Id == u.Id);

			await Context.Channel.DeleteMessagesAsync(a);
		}

		[Command("delete")]
		[Summary("Deletes all the messages from a user N number of times")]
		[RequireUserPermission(GuildPermission.ManageMessages)]
		[RequireBotPermission(ChannelPermission.ManageMessages)]
		public async Task Delete(int n = 1)
		{
			var a = await Context.Channel.GetMessagesAsync(n + 1).Flatten();

			await Context.Channel.DeleteMessagesAsync(a);
		}

		[Command("down")]
		[Summary("Shuts down the bot!")]
		[Alias("shutdown", "downs")]
		public async Task ShutDown()
		{
			if(Context.User.ToString() == "Quincy#1672")
			{
				await ReplyAsync("Shutting down!");
				Program.IsExit = true;
			}
		}

		[Command("reset")]
		[Summary("fuck shit up")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task SetAllNames()
		{
			var users = await Context.Guild.GetUsersAsync();
			foreach(var user in users)
				try
				{
					await user.ModifyAsync(u => u.Nickname = user.Username);
				}
				catch { }
		}

		[Command("nickname")]
		[Summary("Sets all the nicknames on the server to default")]
		[RequireUserPermission(GuildPermission.ChangeNickname)]
		public async Task SetNickName(string name)
		{
			var users = await Context.Guild.GetUsersAsync();
			foreach(var user in users)
				if(user.Id == Context.User.Id)
				{
					await user.ModifyAsync(u => u.Nickname = name);
					break;
				}
		}
	}
}