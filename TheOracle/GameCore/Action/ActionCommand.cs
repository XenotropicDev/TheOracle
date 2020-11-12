using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
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
		public async Task Action([Summary("Modifier for the action roll")][Remainder] string ModiferAndFluff = "")
		{
			List<int> mod = new List<int>();

			var regex = Regex.Matches(ModiferAndFluff, @"[\+-]?\d+");
			foreach (Match match in regex)
            {
				Int32.TryParse(match.Value, out int temp);
				mod.Add(temp);
			}

			var roll = new ActionRoll(mod.ToArray());
			await ReplyAsync(roll.ToString());
		}
	}
}
