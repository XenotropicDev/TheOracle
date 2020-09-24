using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheOracle
{
	public class HelpCommands : ModuleBase<SocketCommandContext>
	{
        public CommandService commandService { get; set; }

        [Command("Help")]
		[Summary("Lists the available commands and their summary")]
		public async Task Help()
		{
			EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commandService.Commands)
            {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary.Length > 0 ? command.Summary : "No description available\n";
                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description:", false, embedBuilder.Build());
		}
	}
}
