using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.IronSworn
{
	public class ActionCommand : ModuleBase<SocketCommandContext>
	{
		[Command("Action", ignoreExtraArgs: true)]
		[Summary("Performs an Iron Sworn action roll")]
		[Alias("act")]
		public async Task Action([Summary("Modifier for the action roll")] int Modifier = 0)
		{
			var roll = new IronSwornRoll(Modifier);
			await ReplyAsync(roll.ToString());
		}
	}
}
