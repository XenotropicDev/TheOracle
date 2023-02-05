using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Server.DiscordServer;

namespace TheOracle2;

public class PartyAutocomplete : AutocompleteHandler
{
    private readonly string likeFormatter = "%{0}%";

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationContext>();
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            //Todo: might want something to limit results when only a few characters have been typed (regex \b{userText}?)
            if (userText.Length > 0)
            {
                successList = db.Parties
                    .Where((party) => party.DiscordGuildId == guildId && EF.Functions.ILike(party.Name, string.Format(likeFormatter, userText)))
                    .OrderBy(pc => pc.Name)
                    .Take(SelectMenuBuilder.MaxOptionCount)
                    .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
            }
            else
            {
                // fallback to list of users own guild PCs, sorted alphabetically
                successList = db.Parties
                    .Where((pc) => pc.DiscordGuildId == guildId)
                    .OrderBy(pc => pc.Name)
                    .Take(SelectMenuBuilder.MaxOptionCount)
                    .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
            }

            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}
