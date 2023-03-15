using Discord.Interactions;
using Server.DiscordServer;

namespace TheOracle2;

public class SetGameCommand : InteractionModuleBase
{
    public SetGameCommand(ApplicationContext db)
    {
        Db = db;
    }

    public ApplicationContext Db { get; }

    [SlashCommand("set-game", "Sets the game for your discord Id in all channels/servers.")]
    public async Task SetGame(IronGame game)
    {
        try
        {
            var existing = await Db.Players.FindAsync(Context.Interaction.User.Id).ConfigureAwait(false);
            existing.Game = game;
            await Db.SaveChangesAsync().ConfigureAwait(false);
            await RespondAsync($"Game set to {game}", ephemeral: true).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await RespondAsync($"An error occurred setting the default game to {game}", ephemeral: true).ConfigureAwait(false);
            throw;
        }
    }
}
