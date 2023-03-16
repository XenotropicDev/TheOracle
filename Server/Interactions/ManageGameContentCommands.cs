using Discord.Interactions;
using Serilog;
using Server.Data.homebrew;
using Server.DiscordServer;

namespace TheOracle2;

public enum GameContentManagementAction
{
    Create,
    Delete,
    Update
}

[Group("manage", "manages custom game content.")]
public class ManageGameContentCommands : InteractionModuleBase
{
    public ManageGameContentCommands(ApplicationContext db)
    {
        Db = db;
    }

    public ApplicationContext Db { get; }

    [SlashCommand("create-set", "Creates an empty game content set that content can be added to")]
    public async Task CreateSet([MinLength(2)] string setName)
    {
        Db.GameContentSets.Add(new GameContentSet { CreatorId = Context.Interaction.User.Id, SetName = setName });
        await Db.SaveChangesAsync().ConfigureAwait(false);
        await RespondAsync($"Content set '{setName}' created", ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("rename-set", "Creates an empty game content set that content can be added to")]
    public async Task RenameSet([Autocomplete(typeof(OwnerSubAutoComplete))] int oldSetName, [MinLength(2)] string newSetName)
    {
        var match = Db.GameContentSets.FirstOrDefault(s => s.CreatorId == Context.Interaction.User.Id && s.Id == oldSetName);

        if (match == null)
        {
            await RespondAsync($"You don't have permission to rename the content set or it could not be found", ephemeral: true).ConfigureAwait(false);
            return;
        }

        match.SetName = newSetName;
        await Db.SaveChangesAsync().ConfigureAwait(false);
        await RespondAsync($"The content set has been renamed.", ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("delete-set", "Deletes a game content set.")]
    public async Task DeleteSet([Autocomplete(typeof(OwnerSubAutoComplete))] int setName)
    {
        var set = Db.GameContentSets.FirstOrDefault(s => s.CreatorId == Context.Interaction.User.Id && s.Id == setName);

        if (set == null)
        {            
            await RespondAsync($"You don't have permission to delete the content set or it could not be found", ephemeral: true).ConfigureAwait(false);
            return;
        }

        set.Moves.Clear();
        set.Oracles.Clear();
        set.Assets.Clear();

        Db.GameContentSets.Remove(set);
        await Db.SaveChangesAsync().ConfigureAwait(false);
        await RespondAsync($"Content set deleted.\nNote: This just removes the set from the search options, it doesn't delete the underlying data. This is to allow things already created with this set to continue working.", ephemeral: true).ConfigureAwait(false);
    }

    [SlashCommand("subscribe", "Adds a game content subscription")]
    public async Task Subscribe([Autocomplete(typeof(PublicSubAutoComplete))] int subscriptionId)
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
