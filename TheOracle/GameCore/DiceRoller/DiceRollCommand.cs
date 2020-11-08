using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.GameCore.DiceRoller
{
    public class DiceRollCommand : ModuleBase<SocketCommandContext>
    {
        [Summary("Rolls die")]
        [Command("Roll")]
        [Alias("Dice")]
        public async Task RollCommand(string dieNotaion, int timesToRoll = 1)
        {
            int numberOfDice;
            int sizeOfDie;
            var dieNotationRegex = Regex.Match(dieNotaion, @"(\d+)d(\d+)");
            if (dieNotationRegex.Success)
            {
                int.TryParse(dieNotationRegex.Groups[1].Value, out numberOfDice);
                int.TryParse(dieNotationRegex.Groups[2].Value, out sizeOfDie);
            }
            else
            {
                numberOfDice = 1;
                if (!int.TryParse(dieNotaion, out sizeOfDie)) throw new ArgumentException(String.Format(RollResources.UnknownNotation, dieNotaion));
                dieNotaion = $"1d{sizeOfDie}";
            }

            if (numberOfDice > 5000 || timesToRoll > 100) throw new ArgumentException("Nice try.");

            Random random = BotRandom.Instance;

            List<int> FinalResults = new List<int>();

            string replyMessage = string.Empty;
            string itemBreak = (timesToRoll <= 5) ? "\n" : ", ";
            for (int i = 0; i < timesToRoll; i++)
            {
                replyMessage += $"{dieNotaion}";
                List<int> RollResults = new List<int>();
                for (int j = 0; j < numberOfDice; j++) RollResults.Add(random.Next(1, sizeOfDie + 1));
                if (RollResults.Count > 1) replyMessage += $" ({String.Join(' ', RollResults)})";
                replyMessage += $" = {RollResults.Sum()}{itemBreak}";
                FinalResults.Add(RollResults.Sum());
            }

            if (replyMessage.Length > DiscordConfig.MaxMessageSize)
            {
                string rollMultiplier = (timesToRoll > 1) ? $"x{timesToRoll} " : string.Empty;
                replyMessage = $"{dieNotaion} {rollMultiplier}: {String.Join(", ", FinalResults)}";
            }
            if (replyMessage.Length > 0) await ReplyAsync(replyMessage).ConfigureAwait(false);
        }
    }
}