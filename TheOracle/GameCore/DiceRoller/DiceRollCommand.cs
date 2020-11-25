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
        [Summary("Rolls dice using standard die notation\n• Sample usage: Roll two 10 sided die `!roll 2d10`, Roll a ten sided die twice `!roll 1d10 2`")]
        [Command("Roll")]
        [Alias("Dice")]
        public async Task RollCommand(string dieNotation, int timesToRoll = 1)
        {
            var roller = new GenericDieRoller(dieNotation, timesToRoll);

            string replyMessage = string.Join(", ", roller.Dice.Select(roll => roll.ToString()));

            if (replyMessage.Length > DiscordConfig.MaxMessageSize) replyMessage = $"{dieNotation} x{timesToRoll} : {roller.RollTotal}";

            if (replyMessage.Length > 0) await ReplyAsync(replyMessage).ConfigureAwait(false);
        }
    }

    public class DieRoll
    {
        public int Sides { get; set; }
        public int NumberOfDie { get; set; }
        public int Bonus { get; }
        public Random Rand { get; }
        public List<int> RollValues { get; } = new List<int>();
        public int RollResult { get => RollValues.Sum() + Bonus; }

        public DieRoll(int sides, int numberOfDie = 1, int bonus = 0, Random rand = null)
        {
            Sides = sides;
            NumberOfDie = numberOfDie;
            Bonus = bonus;
            rand ??= BotRandom.Instance;
            Rand = rand;
            for (int i = 0; i < numberOfDie; i++) RollValues.Add(rand.Next(1, Sides + 1));
        }

        public override string ToString()
        {
            return this.ToString(DieResultFormat.ShowDice);
        }

        public string ToString(DieResultFormat format)
        {
            string bonusValue = (Bonus == 0) ? string.Empty : (Bonus > 0) ? $"+{Bonus}" : Bonus.ToString();
            var showDice = (format == DieResultFormat.ShowDice) ? $"{NumberOfDie}d{Sides}{bonusValue} : " : string.Empty;
            return $"{showDice}{RollValues.Sum() + Bonus}";
        }
    }

    public enum DieResultFormat
    {
        ShowDice,
        ResultOnly
    }

    public class GenericDieRoller
    {
        public List<DieRoll> Dice { get; set; } = new List<DieRoll>();

        public int RollTotal { get => Dice.Sum(roll => roll.RollResult); }

        public GenericDieRoller(string dieNotaion, int timesToRoll = 1)
        {
            int numberOfDice;
            int sizeOfDie;
            int bonus = 0;
            var dieNotationRegex = Regex.Match(dieNotaion, @"(\d+)?d(\d+)([\+-]\d+)?");
            if (dieNotationRegex.Success)
            {
                if (!int.TryParse(dieNotationRegex.Groups[1].Value, out numberOfDice)) numberOfDice = 1;
                int.TryParse(dieNotationRegex.Groups[2].Value, out sizeOfDie);
                if (!int.TryParse(dieNotationRegex.Groups[3].Value, out bonus)) bonus = 0;
            }
            else
            {
                numberOfDice = 1;
                if (!int.TryParse(dieNotaion, out sizeOfDie)) throw new ArgumentException(String.Format(RollResources.UnknownNotation, dieNotaion));
            }

            for (int i = 0; i < timesToRoll; i++) Dice.Add(new DieRoll(sizeOfDie, numberOfDice, bonus));
        }
    }
}