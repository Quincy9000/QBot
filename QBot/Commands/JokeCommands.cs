using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using QBot.Json;

namespace QBot.Commands
{
	/// <summary>
	/// Silly Commands
	/// </summary>
	public class JokeCommands : ModuleBase
	{
		[Command("say")]
		[Summary("says whatever you say, then tries to delete your message if possible")]
		[Alias("echo", "s")]
		public async Task Say([Remainder] string echo)
		{
			await Context.Message.DeleteAsync();
			await ReplyAsync(echo);
		}

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
					int r = QRandom.Next(0, Emotes.Count);
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
				Reddit.RootObject source = JsonConvert.DeserializeObject<Reddit.RootObject>(FunCommands.GetRedditJson("copypasta"));
				int tries = 0;
				while(QRandom.Percent() > 0.5 && tries < 100)
				{
					tries++;
					source = JsonConvert.DeserializeObject<Reddit.RootObject>(FunCommands.GetRedditJson("copypasta",
						source.Data.After.ToString(),
						100));
				}
				if(source == null) return;

				int r = QRandom.Next(0, source.Data.Children.Count);
				text = source.Data.Children[r].Data.Selftext;
				if(!string.IsNullOrEmpty(text))
					done = true;
			}
			if(text.Length > 2000)
			{
				foreach(var str in Program.Split(text, 2000))
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
			await ReplyAsync($"https://cdn.discordapp.com/emojis/{id}.png");
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