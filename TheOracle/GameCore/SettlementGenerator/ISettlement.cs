using Discord;
using System.Threading.Tasks;

namespace TheOracle.GameCore.SettlementGenerator
{
    public interface ISettlement
    {
        string Name { get; set; }

        ISettlement SetupFromUserOptions(string options);

        ISettlement FromEmbed(IEmbed embed);

        EmbedBuilder GetEmbedBuilder();
        Task AfterMessageCreated(IUserMessage msg);
    }
}