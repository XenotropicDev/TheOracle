using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle.IronSworn
{
	public class ActionCommand : ModuleBase<SocketCommandContext>
	{
		[Command("Action")]
		[Summary("Performs an Iron Sworn action roll")]
		[Alias("act")]
		public async Task Action()
		{
			EmbedBuilder embedBuilder = new EmbedBuilder();

			foreach (CommandInfo command in CommandHandlerRegister.Handler.Commands)
			{
				// Get the command Summary attribute information
				string embedFieldText = command.Summary ?? "No description available\n";

			}

			await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
		}
	}
}
