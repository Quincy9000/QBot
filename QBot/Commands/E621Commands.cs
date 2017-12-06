using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace QBot.Commands
{
	public class E621Commands : ModuleBase
	{
		[Command("e621")]
		[Summary("Find content through e621")]
		public async Task SearchByTags([Remainder] string tags)
		{
			try
			{
				if(!Context.Channel.IsNsfw)
				{
					await ReplyAsync("NSFW content not allowed is not allowed in this channel.");
					return;
				}
				//first we parse the string to make sure it is valid

				//raw tags
				string[] allTags = tags.Split(' ');

				//parsed new tags
				List<string> Tags = new List<string>();

				foreach(var tag in allTags)
				{
					if(tag.Contains("/"))
					{
						var newTag = tag.Replace("/", "%25-2F");
						Tags.Add(newTag);
					}
					else
					{
						Tags.Add(tag);
					}
				}

				//combine all the parsed tags into one string again
				StringBuilder search = new StringBuilder();
				foreach(var tag in Tags)
				{
					search.Append(tag + " ");
				}

				//using the e621 api to grab the page in JSON
				Uri e621 = new Uri("https://e621.net/post/index.json?tags=" + search);

				Console.WriteLine(e621.ToString());

				//https://e621.net/help/show/api
				await Task.Run(async () =>
				{
					using(WebClient web = new WebClient())
					{
						try
						{
							web.Headers.Add(HttpRequestHeader.UserAgent,
								"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:57.0) Gecko/20100101 Firefox/57.0");
							var json = web.DownloadString(e621);
							//Console.WriteLine(json);
							var o = JsonConvert.DeserializeObject<List<E621.RootObject>>(json);
							var post = QRandom.ArrayRandom(o);
							EmbedBuilder eb = new EmbedBuilder();
							//eb.Author = new EmbedAuthorBuilder();
							//eb.Author.Name = post.author;
							//eb.Url = post.source;
							//eb.ImageUrl = post.file_url;
							eb.WithAuthor($"{string.Join(", ", post.artist)}", null, $"{post.source}");
							eb.WithImageUrl($"{post.file_url}");
							int C() => QRandom.Number(1, 255);
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
	}
}