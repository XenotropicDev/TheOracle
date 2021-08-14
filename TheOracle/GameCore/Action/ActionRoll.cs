using System;
using System.Linq;
using TheOracle.Core;
using Discord;
using TheOracle.GameCore.Action;

namespace TheOracle.GameCore.Action
{
    public class ActionRoll
    {
        public int ActionDie { get; set; }
        public string Message { get; }
        public int[] Modifiers { get; set; }
        public int ChallengeDie1 { get; set; }
        public int ChallengeDie2 { get; set; }
        public int ActionScore { get => ActionDie + Modifiers.Sum() <= 10 ? ActionDie + Modifiers.Sum() : 10; }

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
            // strong hit
            if (ActionScore > ChallengeDie1 && ActionScore > ChallengeDie2)
            {
                // match
                if (ChallengeDie1 == ChallengeDie2) return $"{ActionResources.Opportunity}";
                return ActionResources.Strong_Hit;
            }
            // miss
            if (ActionScore <= ChallengeDie1 && ActionScore <= ChallengeDie2)
            {
              // match
                if (ChallengeDie1 == ChallengeDie2) return $"{ActionResources.Complication}";
                return ActionResources.Miss;
            }
            return ActionResources.Weak_Hit;
        }
        public Color ResultColor() {
      
            if (ActionScore > ChallengeDie1 && ActionScore > ChallengeDie2)
            // strong hit (Starforged blue)
            {
              return new Color(0x47AEDD);
            }
            if (ActionScore <= ChallengeDie1 && ActionScore <= ChallengeDie2)
            // miss (Starforged red)
            {
              return new Color(0xC50933);
            }
            // weak hit (Starforged purple)
            return new Color(0x842A8C);
    }
            public string ResultIcon() {
      
            // strong hit
            if (ActionScore > ChallengeDie1 && ActionScore > ChallengeDie2)
            {
              return new string("https://i.imgur.com/yQeM5dI.png");
            }
            // miss
            if (ActionScore <= ChallengeDie1 && ActionScore <= ChallengeDie2)
            {
              return new string("https://i.imgur.com/3bAS1Rx.png");
            }
            // weak hit
            return new string("https://i.imgur.com/xrKLiNC.png");
    }
    public EmbedBuilder toEmbed() {
      return new EmbedBuilder().WithColor(ResultColor()).WithThumbnailUrl(ResultIcon()).WithTitle(ResultText()).WithDescription(ToString()).WithFooter(OverMaxMessage()).WithAuthor("Roll");
    }
        public string OverMaxMessage(){
          return (ActionDie + Modifiers.Sum() > 10) ? "\n" + String.Format(ActionResources.OverMaxMessage, ActionDie + Modifiers.Sum()) : string.Empty;
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
            // var overMaxMessage = (ActionDie + Modifiers.Sum() > 10) ? "\n" + String.Format(ActionResources.OverMaxMessage, ActionDie + Modifiers.Sum()) : string.Empty;
            return $"{messageValue}**{ActionScore}**{rollValues} {ActionResources.VS} {ChallengeDie1} {ActionResources.and} {ChallengeDie2}";
        }
    }
}