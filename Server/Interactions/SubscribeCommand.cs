using Discord.Interactions;
using Server.DiscordServer;

namespace TheOracle2;

public class SubscribeCommand : InteractionModuleBase
{
    public SubscribeCommand(ApplicationContext db)
    {
        Db = db;
    }

    public ApplicationContext Db { get; }

    [SlashCommand("subscribe", "Adds a game content subscription")]
    public async Task Subscribe([Autocomplete(typeof(SubscriptionAutocomplete))] string subscriptionId)
    {
        try
        {
            var existing = await Db.Players.FindAsync(Context.Interaction.User.Id).ConfigureAwait(false);
            var homebrewSet = await Db.GameContentSets.FindAsync(subscriptionId).ConfigureAwait(false);

            if (homebrewSet == null)
            {
                await RespondAsync($"Added subscription", ephemeral: true).ConfigureAwait(false);
                return;
            }

            existing.GameDataSets.Add(homebrewSet);
            await Db.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception)
        {
            await RespondAsync($"Something happened and your subscription wasn't updated. Please try again, and report this issue if it continues", ephemeral: true).ConfigureAwait(false);
            throw;
        }
    }
}
