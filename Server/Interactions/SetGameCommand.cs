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
        var existing = await Db.Players.FindAsync(Context.Interaction.User.Id).ConfigureAwait(false);
        if (existing != null) existing.Game = game;
        else Db.Players.Add(new Player() { DiscordId = Context.Interaction.User.Id, Game = game });

        await Db.SaveChangesAsync().ConfigureAwait(false);

        await RespondAsync($"Game set to {game}", ephemeral: true).ConfigureAwait(false);
    }
}
