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
	// Create a module with no prefix
	public class InfoModule : ModuleBase<SocketCommandContext>
	{
		[Command("Help")]
		[Summary("Lists the available commands and their summary")]
		public async Task Help()
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

	// Create a module with the 'sample' prefix
	[Group("sample")]
	public class SampleModule : ModuleBase<SocketCommandContext>
	{
		// ~sample square 20 -> 400
		[Command("square")]
		[Summary("Squares a number.")]
		public async Task SquareAsync(
			[Summary("The number to square.")]
		int num)
		{
			// We can also access the channel from the Command Context.
			await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
		}

		// ~sample userinfo --> foxbot#0282
		// ~sample userinfo @Khionu --> Khionu#8708
		[Command("userinfo")]
		[Summary
		("Returns info about the current user, or the user parameter, if one passed.")]
		[Alias("user", "whois")]
		public async Task UserInfoAsync(
			[Summary("The (optional) user to get info from")]
		SocketUser user = null)
		{
			var userInfo = user ?? Context.Client.CurrentUser;
			await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
		}
	}

}
