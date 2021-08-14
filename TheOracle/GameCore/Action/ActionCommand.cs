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

namespace TheOracle.GameCore.Action
{
	public class ActionCommand : ModuleBase<SocketCommandContext>
	{
		[Command("Action", ignoreExtraArgs: true)]
		[Summary("Performs an Ironsworn action roll")]
		[Alias("act", "move")]
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
      // this is the old version, sent via message
      // await ReplyAsync(roll.ToString());

      await ReplyAsync(embed: roll.toEmbed().WithAuthor($"Action Roll").Build()).ConfigureAwait(false);
		}

		[Command("ProgressRoll", ignoreExtraArgs: true)]
		[Summary("Performs an Progress roll or static action roll")]
		[Alias("StaticAction", "StaticRoll")]
		[Remarks("Useful for rolling progress without a full tracker, or some assets the force the action die's value")]
		public async Task StaticAction([Summary("Modifier for the action roll")][Remainder] string StaticAmountAndFluff = "")
		{
			var match = Regex.Match(StaticAmountAndFluff, @"[\+-]?\d+");
			if (match.Success && Int32.TryParse(match.Groups[0].Value, out int temp))
            {
				var roll = new ActionRoll(0, temp);
				await ReplyAsync(embed: roll.toEmbed().WithAuthor($"Action Roll").Build()).ConfigureAwait(false);
			}
		}
	}
}
