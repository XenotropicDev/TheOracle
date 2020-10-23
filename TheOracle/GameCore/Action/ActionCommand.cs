using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.IronSworn
{
	public class ActionCommand : ModuleBase<SocketCommandContext>
	{
		[Command("Action", ignoreExtraArgs: true)]
		[Summary("Performs an Iron Sworn action roll")]
		[Alias("act")]
		public async Task Action([Summary("Modifier for the action roll")] [Remainder]string ModiferAndFluff = "")
		{
			int mod = 0;

			var regex = Regex.Match(ModiferAndFluff, @"[\+-]?\d+");
			if (regex.Success) Int32.TryParse(regex.Value, out mod);

			var roll = new ActionRoll(mod);
			await ReplyAsync(roll.ToString());
		}
	}
}
