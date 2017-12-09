using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LiteDB;
using Newtonsoft.Json;
using QBot.Json;

namespace QBot.Commands
{
	/// <summary>
	///     Commands that are "sexy" ;)
	/// </summary>
	public class SexyCommands : ModuleBase
	{
		[Command("e621")]
		[Alias("e6", "yiff")]
		[Summary("Find content through e621")]
		public async Task SearchByTags(params string[] _tags)
		{
			try
			{
				if(!Context.Channel.IsNsfw)
				{
					await ReplyAsync("NSFW content not allowed here");
					return;
				}

				List<string> tags = new List<string>(_tags);

				for(int i = 0; i < tags.Count; i++)
				{
					var tag = tags[i];
					if(tag.Contains("/"))
					{
						tags[i] = tag.Replace("/", "%25-2F");
					}
				}

				//combine all the parsed _tags into one string again
				var search = new StringBuilder();
				search.AppendJoin("+", tags);
				search.Append(" ");

				var blackList = GetBlackListTags().Result;
				if(!string.IsNullOrEmpty(blackList))
				{
					search.Append(blackList);
				}

				var global = GetGlobalTagBans().Result;
				if(!string.IsNullOrEmpty(global))
				{
					search.Append(global);
				}

				//using the e621 api to grab the page in JSON
				var e621 = new Uri("https://e621.net/post/index.json?tags=" + search + "&limit=50");

				Console.WriteLine(e621.ToString());

				//https://e621.net/help/show/api
				await Task.Run(async () =>
				{
					using(var web = new WebClient())
					{
						try
						{
							web.Headers.Add(HttpRequestHeader.UserAgent,
								"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
							var json = web.DownloadString(e621);
							//Console.WriteLine(json);
							var o = JsonConvert.DeserializeObject<List<E621.RootObject>>(json);

							var post = QRandom.ArrayRandom(o);
							var eb = new EmbedBuilder();
							//eb.Author = new EmbedAuthorBuilder();
							//eb.Author.Name = post.author;
							//eb.Url = post.source;
							//eb.ImageUrl = post.file_url;
							eb.WithAuthor($"{string.Join(", ", post.artist)}", null, $"{post.source}");
							eb.WithImageUrl($"{post.file_url}");

							int C()
							{
								return QRandom.Number(1, 255);
							}

							eb.WithColor(C(), C(), C());
							eb.WithFooter("Bot by Owner!");
							var d = Context.Channel.EnterTypingState();
							await ReplyAsync("", false, eb);
							d.Dispose();
							//await ReplyAsync(o[randomPost].file_url);
						}
						catch(Exception e)
						{
							Console.WriteLine("Crash" + e);
							await ReplyAsync("Could not find what we were looking for, sorry!");
						}
					}
				});
			}
			catch(Exception e)
			{
				await ReplyAsync("Could not find what we were looking for, sorry!");
			}
		}

		class BannedTags
		{
			public Guid Id { get; set; }

			public string Tags { get; set; } = "";
		}

		[Command("setglobalbannedtags")]
		[Summary("Removes these tags from search no matter who searches")]
		[Alias("sgbt")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task SetGlobalBannedTags(params string[] _tags)
		{
			//cant use list in parameter because discord doesnt support it
			List<string> tags = new List<string>(_tags);

			try
			{
				using(var db = new LiteDatabase(Program.DatabaseConnectionString))
				{
					if(db.CollectionExists($"tags-{Context.Guild.Id}"))
					{
						db.DropCollection($"tags-{Context.Guild.Id}");
					}

					var collection = db.GetCollection<BannedTags>($"tags-{Context.Guild.Id}");

					var ban = new BannedTags();
					foreach(var tag in tags)
					{
						ban.Tags += tag;
					}

					collection.Insert(ban);

					Console.WriteLine("Set global banned tags!");
					await ReplyAsync("Set global banned tags!");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		[Command("getglobalbannedtags")]
		[Summary("Shows the tags that were banned in this server")]
		[Alias("ggbt")]
		public async Task GetGlobalTags()
		{
			var tags = GetGlobalTagBans().Result;
			if(string.IsNullOrEmpty(tags))
			{
				await ReplyAsync("No global tags set!");
				return;
			}
			await ReplyAsync($"Banned global tags are {tags}");
		}

		/// <summary>
		/// Set your preferred _tags to blacklist
		/// </summary>
		/// <returns></returns>
		[Command("setblacklist")]
		[Summary("Set blacklisted tags")]
		[Alias("sbl")]
		public async Task SetBlackListTags(params string[] tags)
		{
			try
			{
				using(var db = new LiteDatabase(Program.DatabaseConnectionString))
				{
					var members = db.GetCollection<GuildMember>($"{Context.Guild.Id}");

					//Find the member who called this func from the db
					var member = members.FindOne(id => id.UniqueId == Context.User.Id);

					foreach(var tag in tags)
					{
						if(tag.ToLower() == "clear")
						{
							//set no _tags
							member.BlackTags = "";
							Console.WriteLine($"Cleared tags for {member.UserName}!");
							await ReplyAsync($"Cleared tags for {member.UserName}!");
							members.Update(member);
							return;
						}
						member.BlackTags += " " + tag;
					}

					members.Update(member);
					await ReplyAsync($"Updated tags for {member.UserName}!");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
			await Task.CompletedTask;
		}

		[Command("nope")]
		[Summary("Deletes the last nsfw post if the search didnt blacklist tags correctly")]
		[Alias("badbot")]
		public async Task Nope()
		{
			var messages =
				new List<IMessage>(await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before).Flatten());

			var msg = messages.Where(m => m.Author.Id == Program.Bot.Id && m.Embeds.Count > 0).ToArray()[0];

			await Context.Channel.DeleteMessagesAsync(new IMessage[1] {msg});
		}

		[Command("getblacklist")]
		[Summary("Get the tags that the user set")]
		[Alias("gbl")]
		public async Task GetBlackTags()
		{
			var memberTags = GetBlackListTags().Result;
			if(string.IsNullOrEmpty(memberTags))
				await ReplyAsync($"Tags for {Context.User.Username}: None set!");
			else
			{
				await ReplyAsync($"Tags for {Context.User.Username}: {memberTags}!");
			}

			await Task.CompletedTask;
		}

		Task<string> GetBlackListTags()
		{
			try
			{
				using(var db = new LiteDatabase(Program.DatabaseConnectionString))
				{
					var members = db.GetCollection<GuildMember>($"{Context.Guild.Id}");

					//Find the member who called this func from the db
					var member = members.FindOne(id => id.UniqueId == Context.User.Id);

					return Task.FromResult(member.BlackTags);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return Task.FromResult("");
		}

		Task<string> GetGlobalTagBans()
		{
			try
			{
				using(var db = new LiteDatabase(Program.DatabaseConnectionString))
				{
					var members = db.GetCollection<BannedTags>($"tags-{Context.Guild.Id}");

					var tags = members.FindAll();

					var tag = tags.FirstOrDefault();

					if(!string.IsNullOrEmpty(tag?.Tags))
						return Task.FromResult(tag.Tags);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}

			return Task.FromResult("");
		}

		public static string GetRedditJson(string sub, string id = null, int count = 25)
		{
			string json;
			using(var wb = new WebClient())
			{
				if(string.IsNullOrEmpty(id))
					json = wb.DownloadString($"https://reddit.com/r/{sub}/.json?limit={count}/");
				else
					json = wb.DownloadString($"https://reddit.com/r/{sub}/.json?limit={count}&after={id}/");
			}
			return json;
		}

		[Command("subpic")]
		[Alias("sp", "pic")]
		[Summary("Get a picture from a subreddit")]
		public async Task Sub([Summary("The sub to get the image from")] string sub, int amount = 1)
		{
			const int limit = 25;
			try
			{
				if(amount > limit)
					throw new EvaluateException($"Amount must be {limit} or less");
				Reddit.RootObject page = null;
				const string nextId = "";
				var urls = new List<ushort>();
				page = JsonConvert.DeserializeObject<Reddit.RootObject>(GetRedditJson(sub));
				var tries = 0;
				while(QRandom.Percent() > 0.5 && tries < 100)
				{
					tries++;
					page = JsonConvert.DeserializeObject<Reddit.RootObject>(GetRedditJson(sub, page.Data.After.ToString(), 100));
				}

				if(page == null) return;
				var post = "";
				//reset tries
				tries = 0;
				var count = 0;
				//only try 100 times so that it doesnt get caught in an infinite loop
				while(count < amount && tries < 100)
				{
					tries++;
					var i = (ushort) QRandom.Next(0, page.Data.Children.Count);
					if(page.Data.Children[i].Data.Domain.Contains("imgur")
					   || page.Data.Children[i].Data.Domain.Contains("jpg")
					   || page.Data.Children[i].Data.Domain.Contains("png")
					   || page.Data.Children[i].Data.Domain.Contains("gfycat")
					   || page.Data.Children[i].Data.Domain.Contains("mp4")
					   || page.Data.Children[i].Data.Domain.Contains("gif"))
						if(Context.Channel.IsNsfw && page.Data.Children[i].Data.Over_18
						   || !Context.Channel.IsNsfw && !page.Data.Children[i].Data.Over_18
						   || Context.Channel.IsNsfw && !page.Data.Children[i].Data.Over_18)
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
	}
}