using Discord;
using System;
using System.Linq;
using TheOracle.Core;

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

        public ActionRollResult RollResult
        {
            get
            {
                if (ActionScore > ChallengeDie1 && ActionScore > ChallengeDie2)
                {
                    if (ChallengeDie1 == ChallengeDie2) return ActionRollResult.MatchHit;
                    return ActionRollResult.StrongHit;
                }
                if (ActionScore <= ChallengeDie1 && ActionScore <= ChallengeDie2)
                {
                    if (ChallengeDie1 == ChallengeDie2) return ActionRollResult.MatchMiss;
                    return ActionRollResult.Miss;
                }
                return ActionRollResult.WeakHit;
            }
        }

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
            return RollResult switch
            {
                ActionRollResult.Miss => ActionResources.Miss,
                ActionRollResult.WeakHit => ActionResources.Weak_Hit,
                ActionRollResult.StrongHit => ActionResources.Strong_Hit,
                ActionRollResult.MatchHit => ActionResources.Opportunity,
                ActionRollResult.MatchMiss => ActionResources.Complication,
                _ => "ERROR",
            };
        }

        public Color ResultColor()
        {
            return RollResult switch
            {
                ActionRollResult.Miss => new Color(0xC50933),
                ActionRollResult.WeakHit => new Color(0x842A8C),
                ActionRollResult.StrongHit => new Color(0x47AEDD),
                ActionRollResult.MatchHit => new Color(0x47AEDD),
                ActionRollResult.MatchMiss => new Color(0xC50933),
                _ => new Color(0x842A8C),
            };
        }

        public string ResultIcon()
        {
            return RollResult switch
            {
                ActionRollResult.Miss => ActionResources.MissImageURL,
                ActionRollResult.WeakHit => ActionResources.WeakHitImageURL,
                ActionRollResult.StrongHit => ActionResources.StrongHitImageURL,
                ActionRollResult.MatchHit => ActionResources.StrongHitImageURL,
                ActionRollResult.MatchMiss => ActionResources.MissImageURL,
                _ => ActionResources.MissImageURL,
            };
        }

        public EmbedBuilder ToEmbed()
        {
            return new EmbedBuilder().WithColor(ResultColor()).WithThumbnailUrl(ResultIcon()).WithTitle(ResultText()).WithDescription(ToString()).WithFooter(OverMaxMessage()).WithAuthor("Roll");
        }

        public string OverMaxMessage()
        {
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