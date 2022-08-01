using System.ComponentModel.DataAnnotations;
using Discord.Interactions;
using Server.DiscordServer;
using TheOracle2.GameObjects;

namespace TheOracle2;

public class SetGameCommand : InteractionModuleBase
{
    private readonly Random random;

    public SetGameCommand(ApplicationContext db)
    {
        Db = db;
    }

    public ApplicationContext Db { get; }

    [SlashCommand("set-game", "Sets the game for your discord Id in all channels/servers.")]
    public async Task AskTheOracle(IronGame game)
    {
        var existing = Db.Players.Find(Context.Interaction.User.Id);
        if (existing != null) existing.Game = game;
        else Db.Players.Add(new Player() { DiscordId = Context.Interaction.User.Id, Game = game });

        await Db.SaveChangesAsync().ConfigureAwait(false);
            
        await RespondAsync($"Game set to {game}", ephemeral: true).ConfigureAwait(false);
    }
}
