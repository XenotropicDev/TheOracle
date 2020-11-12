using System.Linq;
using TheOracle.GameCore.Action;

namespace TheOracle.Core
{
    public class ActionRoll
    {
        public int ActionDie { get; set; }
        public string Message { get; }
        public int[] Modifiers { get; set; }
        public int ChallengeDie1 { get; set; }
        public int ChallengeDie2 { get; set; }
        public int ActionScore { get => ActionDie + Modifiers.Sum(); }

        /// <summary>
        /// Rolls dice for a Ironsworn game action.
        /// </summary>
        /// <param name="playerModifier"></param>
        /// <param name="actionDie">Sets the value of the ActionDie, useful for things like progress rolls</param>
        public ActionRoll(int playerModifier = 0, int? actionDie = null, string message = "")
        {
            ChallengeDie1 = BotRandom.Instance.Next(1, 11);
            ChallengeDie2 = BotRandom.Instance.Next(1, 11);
            ActionDie = actionDie ?? BotRandom.Instance.Next(1, 7);
            Message = message;
            Modifiers = new int[] { playerModifier };
        }

        /// <summary>
        /// Rolls dice for a Ironsworn game action.
        /// </summary>
        /// <param name="modifiers"></param>
        /// <param name="actionDie">Sets the value of the ActionDie, useful for things like progress rolls</param>
        public ActionRoll(int[] modifiers, int? actionDie = null, string message = "")
        {
            ChallengeDie1 = BotRandom.Instance.Next(1, 11);
            ChallengeDie2 = BotRandom.Instance.Next(1, 11);
            ActionDie = actionDie ?? BotRandom.Instance.Next(1, 7);
            Message = message;
            Modifiers = modifiers;
        }

        public string ResultText()
        {
            if (ActionScore > ChallengeDie1 && ActionScore > ChallengeDie2)
            {
                if (ChallengeDie1 == ChallengeDie2) return $"{ActionResources.Opportunity}";
                return ActionResources.Strong_Hit;
            }
            if (ActionScore <= ChallengeDie1 && ActionScore <= ChallengeDie2)
            {
                if (ChallengeDie1 == ChallengeDie2) return $"{ActionResources.Complication}";
                return ActionResources.Miss;
            }
            return ActionResources.Weak_Hit;
        }

        public override string ToString()
        {
            string modString = string.Empty;
            foreach (int mod in Modifiers)
            {
                if (mod > 0) modString += $"+{mod}";
                if (mod < 0) modString += $"{mod}";
            }

            var rollValues = (Modifiers.Any(mod => mod != 0)) ? $" ({ActionDie}{modString})" : string.Empty;
            var messageValue = (Message.Length > 0) ? $" {Message}\n" : string.Empty;
            return $"{messageValue}**{ActionScore}**{rollValues} {ActionResources.VS} {ChallengeDie1} {ActionResources.and} {ChallengeDie2}\n{ResultText()}";
        }
    }
}