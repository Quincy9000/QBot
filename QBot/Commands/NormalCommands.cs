using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace QBot.Commands
{
	public class NormalCommands : ModuleBase
	{
		[Command("help")]
		[Summary("Lists all the available commands that the bot can currently do")]
		public async Task Help()
		{
			EmbedBuilder eb = new EmbedBuilder();

			MethodInfo[] mi = GetType().GetMethods();
			for(int o = 0; o < mi.Length; o++)
			{
				CommandAttribute myAttribute1 = mi[o].GetCustomAttributes(true).OfType<CommandAttribute>().FirstOrDefault();
				SummaryAttribute myAttribute2 = mi[o].GetCustomAttributes(true).OfType<SummaryAttribute>().FirstOrDefault();
				if(myAttribute1 != null && myAttribute2 != null)
					eb.AddField(myAttribute1.Text, myAttribute2.Text);
			}

			await ReplyAsync("", false, eb);
		}

		[Command("say")]
		[Summary("says whatever you say, then tries to delete your message if possible")]
		[Alias("echo", "s")]
		public async Task Say([Remainder] string echo)
		{
			await Context.Message.DeleteAsync();
			await ReplyAsync(echo);
		}

		//		[Command("square")]
		//		[Summary("Squares a number.")]
		//		public async Task Square([Summary("The number to square.")] int num)
		//		{
		//			await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
		//		}

		[Command("roll")]
		[Summary("Rolls a dice n number of sides n number of times, ex qroll 4 4, rolls 4 sided dice 4 times")]
		[Alias("r")]
		public async Task Roll(int sides = 6, int times = 1)
		{
			var numbers = QRandom.EmulateRolls(sides, times);
			int total = 0;
			foreach(var number in numbers)
				total += number;
			await ReplyAsync($"{Context.User.Username} rolled {string.Join(", ", numbers)} totalling to {total}!");
		}

		[Command("userinfo")]
		[Summary("Returns info about the current user, or the user parameter, if one passed.")]
		[Alias("user", "whois")]
		public async Task UserInfo([Summary("The (optional) user to get info for")] IUser u = null)
		{
			EmbedBuilder eb = new EmbedBuilder();
			var user = u as SocketGuildUser;
			if(user == null) return;

			string g = "";
			if(user.Game.HasValue)
				g = $"\n**Game:** {user.Game.Value}";
			if(!string.IsNullOrEmpty(user.GetAvatarUrl()))
			{
				string url = user.GetAvatarUrl();
				eb.WithAuthor(b =>
				{
					b.Name = user.Username;
					b.IconUrl = url;
				});
			}
			string bot = user.IsBot ? "**Bot:** BEEP BOOP NOT A BOT" : "**Human:** I AM A HUMAN";
			string roles = "";
			foreach(var r in user.Roles)
			{
				if(r.Name != "@everyone")
					roles += $"{r.Name}, ";
			}
			eb.Description =
				$"**Nickname:** {user.Nickname}\n**ID:** {user.Id}\n**Date Joined:** {user.CreatedAt.DateTime}{g}\n**Status:** {user.Status}\n{bot}\n**Roles:** {roles}";
			eb.WithColor(new Color(Program.R.Next(0, 256), Program.R.Next(0, 256), Program.R.Next(0, 256)));
			await ReplyAsync("", false, eb);
		}

		[Command("getav")]
		[Summary("Gets the avatar of the user")]
		[Alias("av")]
		public async Task GetAvatar(IUser u) => await ReplyAsync(u.GetAvatarUrl());

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
				['0'] = "zero",
			};

			string msg = "";
			for(int i = 0; i < word.Length; i++)
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
				var f = Split(msg, 1999);

				foreach(var a in f)
				{
					await ReplyAsync(a);
				}
			}
			else
			{
				await ReplyAsync(msg);
			}
		}

		private static IEnumerable<string> Split(string str, int chunkSize) => Enumerable.Range(0, str.Length / chunkSize)
			.Select(i => str.Substring(i * chunkSize, chunkSize));

		private string GetRedditJson(string sub, string id = null, int count = 25)
		{
			string json;
			using(WebClient wb = new WebClient())
			{
				if(string.IsNullOrEmpty(id))
					json = wb.DownloadString($"https://reddit.com/r/{sub}/.json?limit={count}/");
				else
					json = wb.DownloadString($"https://reddit.com/r/{sub}/.json?limit={count}&after={id}/");
			}
			return json;
		}

		[Command("subpic")]
		[Summary("Get a pic from a subreddit")]
		[Alias("sp", "pic")]
		public async Task Sub([Summary("The sub to get the image from")] string sub, int amount = 1)
		{
			const int limit = 25;
			try
			{
				if(amount > limit)
					throw new EvaluateException($"Amount must be {limit} or less");
				Reddit.RootObject page = null;
				const string nextId = "";
				List<ushort> urls = new List<ushort>();

				page = JsonConvert.DeserializeObject<Reddit.RootObject>(GetRedditJson(sub));
				int tries = 0;
				while(Program.R.NextDouble() > 0.5 && tries < 100)
				{
					tries++;
					page = JsonConvert.DeserializeObject<Reddit.RootObject>(GetRedditJson(sub, page.Data.After.ToString(), 100));
				}
				if(page == null) return;

				string post = "";
				//reset tries
				tries = 0;
				int count = 0;
				//only try 100 times so that it doesnt get caught in an infinite loop
				while(count < amount && tries < 100)
				{
					tries++;
					ushort i = (ushort)Program.R.Next(0, page.Data.Children.Count);
					if(page.Data.Children[i].Data.Domain.Contains("imgur")
					   || page.Data.Children[i].Data.Domain.Contains("jpg")
					   || page.Data.Children[i].Data.Domain.Contains("png")
					   || page.Data.Children[i].Data.Domain.Contains("gfycat")
					   || page.Data.Children[i].Data.Domain.Contains("mp4")
					   || page.Data.Children[i].Data.Domain.Contains("gif"))
					{
						if((Context.Channel.IsNsfw && page.Data.Children[i].Data.Over_18)
						   || (!Context.Channel.IsNsfw && !page.Data.Children[i].Data.Over_18)
						   || (Context.Channel.IsNsfw && !page.Data.Children[i].Data.Over_18))
						{
							if(!urls.Contains(i))
							{
								urls.Add(i);
								post += $"{page.Data.Children[i].Data.Url}\n";
								Console.WriteLine(page.Data.Children[i].Data.Url);
								count++;
							}
						}
						else
						{
							await ReplyAsync("NSFW content not allowed in this channel");
							return;
						}
					}
				}

				await ReplyAsync(post);
			}
			catch(EvaluateException ee)
			{
				Console.WriteLine(ee.Message);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
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

		[Command("emojis")]
		[Summary("Sends all the custom emojis in the server")]
		public async Task Emojis()
		{
			List<GuildEmote> Emotes = new List<GuildEmote>(Context.Guild.Emotes);
			string emojis = "";
			foreach(var guildEmote in Emotes)
			{
				emojis += $"<:{guildEmote.Name}:{guildEmote.Id}> ";
			}
			await ReplyAsync(emojis);
		}

		[Command("emoji")]
		[Summary("Sends a random emoji")]
		public async Task Emoji(int i = 1)
		{
			if(i <= 50)
			{
				var e = Context.Guild.Emotes;
				//await Context.Message.DeleteAsync();
				List<GuildEmote> Emotes = new List<GuildEmote>(e);
				List<int> randomsUsed = new List<int>();
				string emote = "";
				for(int j = 0; j < i;)
				{
					int r = Program.R.Next(0, Emotes.Count);
					if(!randomsUsed.Contains(r))
					{
						randomsUsed.Add(r);
						emote += $"<:{Emotes[r].Name}:{Emotes[r].Id}> ";
						j++;
					}
				}
				await ReplyAsync(emote);
			}
			else
			{
				await ReplyAsync("Highest number of emotes is 50, did you mean qemojis?");
			}
		}

		[Command("cp")]
		[Alias("cheesepizza", "copypasta")]
		[Summary("WHAT THE FUCK DID YOU JUST SAY ABOUT ME?")]
		public async Task CopyPasta()
		{
			string text = "";
			bool done = false;
			while(!done)
			{
				//await Context.Message.DeleteAsync();
				Reddit.RootObject source = JsonConvert.DeserializeObject<Reddit.RootObject>(GetRedditJson("copypasta"));
				int tries = 0;
				while(Program.R.NextDouble() > 0.5 && tries < 100)
				{
					tries++;
					source = JsonConvert.DeserializeObject<Reddit.RootObject>(GetRedditJson("copypasta", source.Data.After.ToString(),
						100));
				}
				if(source == null) return;

				int r = Program.R.Next(0, source.Data.Children.Count);
				text = source.Data.Children[r].Data.Selftext;
				if(!string.IsNullOrEmpty(text))
					done = true;
			}
			if(text.Length > 2000)
			{
				foreach(var str in Split(text, 2000))
				{
					await ReplyAsync(str);
				}
			}
			else
			{
				await ReplyAsync(text);
			}
		}

		[Command("bigly")]
		[Summary("big emote")]
		public async Task Bigly(string s)
		{
			Regex rx = new Regex("<:([^:]+):([^:>]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			MatchCollection matches = rx.Matches(s);

			if(matches.Count < 1)
			{
				await ReplyAsync("no");
				return;
			}

			Match match = matches[0];
			GroupCollection groups = match.Groups;

			string name = groups[1].Value;
			string id = groups[2].Value;
			//
			//		    Console.WriteLine("name={0}, id={1}", name, id);
			//		    Console.WriteLine("url=https://cdn.discordapp.com/emojis/{0}.png", id);
			await ReplyAsync($"https://cdn.discordapp.com/emojis/{id}.png");
		}

		[Command("reset")]
		[Summary("fuck shit up")]
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
				catch
				{
					continue;
				}
			}
		}

		[Command("nickname")]
		[Summary("fuck shit up")]
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

		[Command("henlo")]
		[Summary("henlo you stinky")]
		public async Task Henlo([Remainder] string hi = null)
		{
			if(string.IsNullOrEmpty(hi))
			{
				await ReplyAsync("henlo you stinky lizard");
				await ReplyAsync("go eat a bug you ugly");
			}
			else if(hi.Contains("gay"))
			{
				await ReplyAsync("Henlo gay faggot");
				await ReplyAsync("go eat a balls stinky");
			}
			else if(hi.Contains("fem"))
			{
				await ReplyAsync("henlo you ugly sjw cuck");
				await ReplyAsync("go eat a patriarchy u ugly whore");
			}
			else if(hi.Contains("birb") || hi.Contains("bird"))
			{
				await ReplyAsync("henlo you ugly birb");
				await ReplyAsync("go eat a seed smelly");
			}
			else if(hi.Contains("cist"))
			{
				await ReplyAsync("henlo you white male");
				await ReplyAsync("go eat a privilege ugly");
			}
			else if(hi.Contains("aus"))
			{
				await ReplyAsync("henlo you stupid autist");
				await ReplyAsync("go eat a fidget spinner aspie");
			}
			else if(hi.Contains("obliv"))
			{
				await ReplyAsync("henlo you stupid adoring fan");
				await ReplyAsync("go eat a sword u ugly retard");
			}
			else if(hi.Contains("skyrim"))
			{
				await ReplyAsync("henlo you stupid guard");
				await ReplyAsync("go eat an arrow stinky");
			}
			else if(hi.Contains("frog"))
			{
				await ReplyAsync("henlo you slimy frog");
				await ReplyAsync("go eat a property smelly");
			}
		}
	}
}