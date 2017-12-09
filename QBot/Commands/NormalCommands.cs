using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;

namespace QBot.Commands
{
	/// <summary>
	/// Commands for everyday use
	/// </summary>
	public class NormalCommands : ModuleBase
	{
		[Command("ping")]
		[Summary("Gets latency to server")]
		public async Task Ping()
		{
			await ReplyAsync($"QPong {Program.Latency}ms!");
		}

		[Command("users")]
		[Summary("Lists all the users in the guild")]
		public async Task ListUsers()
		{
			string users = "";
			try
			{
				using(var db = new LiteDatabase(Program.DatabaseConnectionString))
				{
					var collection = db.GetCollection<GuildMember>($"{Context.Guild.Id}");

					bool flag = true;
					foreach(var user in collection.FindAll())
					{
						if(flag)
						{
							users += user.UserName;
							flag = false;
						}
						else
						{
							users += ", " + user.UserName;
						}
					}

					await ReplyAsync(users + $"\nUsers: {collection.Count()}");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		[Command("userinfo")]
		[Summary("Returns info about the current user, or the user parameter, if one passed.")]
		[Alias("user", "whois")]
		public async Task UserInfo([Summary("The (optional) user to get info for")] IUser u = null)
		{
			var eb = new EmbedBuilder();

			if(u is null)
			{
				var uuser = (SocketGuildUser) Context.User;
				var ug = "";
				if(uuser.Game.HasValue)
					ug = $"\n**Game:** {uuser.Game.Value}";
				if(!string.IsNullOrEmpty(uuser.GetAvatarUrl()))
				{
					var url = uuser.GetAvatarUrl();
					eb.WithAuthor(b =>
					{
						b.Name = uuser.Username;
						b.IconUrl = url;
					});
				}
				var ubot = uuser.IsBot ? "**Bot:** BEEP BOOP NOT A BOT" : "**Human:** I AM A HUMAN";
				var uroles = string.Join(", ", uuser.Roles);
				eb.Description =
					$"**Nickname:** {uuser.Nickname}\n**ID:** {uuser.Id}\n**Date Joined:** {uuser.CreatedAt.DateTime}{ug}\n**Status:** {uuser.Status}\n{ubot}\n**Roles:** {uroles}";
				eb.WithColor(new Color(QRandom.Next(0, 256), QRandom.Next(0, 256), QRandom.Next(0, 256)));
				await ReplyAsync("", false, eb);
				return;
			}

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
			eb.WithColor(new Color(QRandom.Next(0, 256), QRandom.Next(0, 256), QRandom.Next(0, 256)));
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
			{
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
			}

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
			if(Context.User.Id == Program.Owner.Id)
			{
				await ReplyAsync("Shutting down!");
				Program.IsExit = true;
				Program.IsReboot = false;
			}
		}

		[Command("reboot")]
		[Summary("reboot the bot")]
		public async Task Reboot()
		{
			if (Context.User.Id == Program.Owner.Id)
			{
				await ReplyAsync("Rebooting!");
				Program.IsExit = true;
				Program.IsReboot = true;
			}
		}

		[Command("reset")]
		[Summary("Resets all nicknames to default username")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task SetAllNames()
		{
			var users = await Context.Guild.GetUsersAsync();
			foreach(var user in users)
			{
				try
				{
					await user.ModifyAsync(u => u.Nickname = user.Username);
				}
				catch { }
			}
		}

		[Command("nickname")]
		[Summary("Sets your nickname to the one you choose")]
		[RequireUserPermission(GuildPermission.ChangeNickname)]
		public async Task SetNickName(string name)
		{
			var users = await Context.Guild.GetUsersAsync();
			foreach(var user in users)
			{
				if(user.Id == Context.User.Id)
				{
					await user.ModifyAsync(u => u.Nickname = name);
					break;
				}
			}
		}
	}
}