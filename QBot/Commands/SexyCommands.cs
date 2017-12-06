using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using QBot.Json;

namespace QBot.Commands
{
	/// <summary>
	///     Commands that are "sexy" ;)
	/// </summary>
	public class FunCommands : ModuleBase
	{
		[Command("e621")]
		[Alias("e6")]
		[Summary("Find content through e621")]
		public async Task SearchByTags([Remainder] string tags)
		{
			try
			{
				if(!Context.Channel.IsNsfw)
				{
					await ReplyAsync("NSFW content not allowed here");
					return;
				}
				//first we parse the string to make sure it is valid

				//raw tags
				var allTags = tags.Split(' ');

				//parsed new tags
				var Tags = new List<string>();

				foreach(var tag in allTags)
					if(tag.Contains("/"))
					{
						var newTag = tag.Replace("/", "%25-2F");
						Tags.Add(newTag);
					}
					else
					{
						Tags.Add(tag);
					}

				//combine all the parsed tags into one string again
				var search = new StringBuilder();
				foreach(var tag in Tags)
					search.Append(tag + " ");

				//using the e621 api to grab the page in JSON
				var e621 = new Uri("https://e621.net/post/index.json?tags=" + search);

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
							eb.WithFooter("QBot by Quincy!");
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
		[Summary("Get a pic from a subreddit")]
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
				while(Program.R.NextDouble() > 0.5 && tries < 100)
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
					var i = (ushort) Program.R.Next(0, page.Data.Children.Count);
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