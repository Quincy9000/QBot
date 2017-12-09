using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace QBot.Commands
{
	public class HelpCommands : ModuleBase
	{
		[Command("help")]
		[Summary("Lists all of my commands!")]
		public async Task Help()
		{
			var eb = new EmbedBuilder();

			var joke = typeof(JokeCommands).GetMethods();
			var sexy = typeof(SexyCommands).GetMethods();
			var norm = typeof(NormalCommands).GetMethods();

			for(var o = 0; o < joke.Length; o++)
			{
				var myAttribute1 = joke[o].GetCustomAttributes(true).OfType<CommandAttribute>().FirstOrDefault();
				var myAttribute2 = joke[o].GetCustomAttributes(true).OfType<SummaryAttribute>().FirstOrDefault();
				var myAttribute3 = joke[o].GetCustomAttributes(true).OfType<AliasAttribute>().FirstOrDefault();
				string[] aliases = new string[2];
				if(myAttribute3 != null)
					aliases = myAttribute3.Aliases;
				if(myAttribute1 != null && myAttribute2 != null)
					eb.AddField(myAttribute1.Text + ", " + string.Join(", ", aliases), myAttribute2.Text);
			}

			for(var o = 0; o < sexy.Length; o++)
			{
				var myAttribute1 = sexy[o].GetCustomAttributes(true).OfType<CommandAttribute>().FirstOrDefault();
				var myAttribute2 = sexy[o].GetCustomAttributes(true).OfType<SummaryAttribute>().FirstOrDefault();
				var myAttribute3 = sexy[o].GetCustomAttributes(true).OfType<AliasAttribute>().FirstOrDefault();
				string[] aliases = new string[2];
				if(myAttribute3 != null)
					aliases = myAttribute3.Aliases;
				if(myAttribute1 != null && myAttribute2 != null)
					eb.AddField(myAttribute1.Text + ", " + string.Join(", ", aliases), myAttribute2.Text);
			}

			for(var o = 0; o < norm.Length; o++)
			{
				var myAttribute1 = norm[o].GetCustomAttributes(true).OfType<CommandAttribute>().FirstOrDefault();
				var myAttribute2 = norm[o].GetCustomAttributes(true).OfType<SummaryAttribute>().FirstOrDefault();
				var myAttribute3 = norm[o].GetCustomAttributes(true).OfType<AliasAttribute>().FirstOrDefault();
				string[] aliases = new string[2];
				if(myAttribute3 != null)
					aliases = myAttribute3.Aliases;
				if(myAttribute1 != null && myAttribute2 != null)
					eb.AddField(myAttribute1.Text + ", " + string.Join(", ", aliases), myAttribute2.Text);
			}

			await Context.User.SendMessageAsync("", false, eb);
		}
	}
}