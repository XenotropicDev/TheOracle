using TheOracle.GameCore.Action;

namespace TheOracle.Core
{
    public class ActionRoll
    {
        public int ActionDie { get; set; }
        public int PlayerModifier { get; set; }
        public int ChallengeDie1 { get; set; }
        public int ChallengeDie2 { get; set; }
        public int ActionScore { get => ActionDie + PlayerModifier; }

        /// <summary>
        /// Rolls dice for a Ironsworn game action.
        /// </summary>
        /// <param name="playerModifier"></param>
        /// <param name="actionDie">Sets the value of the ActionDie, useful for things like progress rolls</param>
        public ActionRoll(int playerModifier = 0, int? actionDie = null)
        {
            ChallengeDie1 = BotRandom.Instance.Next(1, 10);
            ChallengeDie2 = BotRandom.Instance.Next(1, 10);
            ActionDie = actionDie ?? BotRandom.Instance.Next(1, 6);
            PlayerModifier = playerModifier;
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
            var rollValues = (PlayerModifier != 0) ? $" ({ActionDie}+{PlayerModifier})" : string.Empty;
            return $"{ActionScore}{rollValues} {ActionResources.VS} {ChallengeDie1} {ActionResources.and} {ChallengeDie2}\n{ResultText()}";
        }
    }
}