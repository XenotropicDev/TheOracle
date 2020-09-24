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
		public async Task Help(string CommandName = default)
		{
			EmbedBuilder embedBuilder = new EmbedBuilder();

            if (CommandName == default)
            {
                foreach (CommandInfo command in commandService.Commands)
                {
                    string embedFieldText = command.Summary.Length > 0 ? command.Summary : "No description available\n";
                    var aliasCommands = command.Aliases.Where(a => !a.Equals(command.Name, StringComparison.OrdinalIgnoreCase));
                    string aliasCSV = aliasCommands.Count() > 0 ? $" [{String.Join(',', aliasCommands)}]" : string.Empty;

                    embedBuilder.AddField($"{command.Name}{aliasCSV}", embedFieldText);
                }

                await ReplyAsync("Here's a list of commands and their description:", false, embedBuilder.Build());
                return;
            }

            var singleCommand = commandService.Commands.SingleOrDefault(c => c.Name.Equals(CommandName, StringComparison.OrdinalIgnoreCase));

            string fieldText = singleCommand.Summary.Length > 0 ? singleCommand.Summary : "No description available\n";
            string aliasText = singleCommand.Aliases.Count > 0 ? $" [{String.Join(',', singleCommand.Aliases.Where(a => !a.Equals(singleCommand.Name, StringComparison.OrdinalIgnoreCase)))}]" : string.Empty;

            embedBuilder.AddField($"{singleCommand.Name}{aliasText}", fieldText);
            await ReplyAsync("Here's information on the command:", false, embedBuilder.Build());
            return;
        }
	}
}
