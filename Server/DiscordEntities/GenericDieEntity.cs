using TheOracle2;
using TheOracle2.GameObjects;

namespace Server.DiscordEntities
{
    internal class GenericDieEntity : IDiscordEntity
    {
        public GenericDieEntity(IEnumerable<IDie> dice, DieNotation notation = null)
        {
            Dice = dice;
            Notation = notation;
        }

        public bool IsEphemeral { get; set; } = false;
        public string? DiscordMessage { get; set; }
        public IEnumerable<IDie> Dice { get; }
        public DieNotation Notation { get; }

        public Task<ComponentBuilder?> GetComponentsAsync()
        {
            return Task.FromResult<ComponentBuilder?>(null);
        }

        public EmbedBuilder? GetEmbed()
        {
            var builder = new EmbedBuilder();

            builder.WithTitle($"Dice Roll {Notation?.ToString()}");

            if (Dice.Count() <= EmbedBuilder.MaxFieldCount - 1)
            {
                foreach (var d in Dice)
                {
                    builder.AddField($"d{d.Sides}", d.Value, true);
                }
            }

            builder.AddField("Total", Dice.Sum(d => d.Value));

            return builder;
        }
    }
}
