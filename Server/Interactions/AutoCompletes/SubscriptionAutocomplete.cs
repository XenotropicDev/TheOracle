using Discord.Interactions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Server.Data;
using Server.DiscordServer;

namespace TheOracle2;

public class PublicSubAutoComplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrWhiteSpace(userText)) return AutocompletionResult.FromSuccess(successList);

            var db = services.GetRequiredService<ApplicationContext>();
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            var matches = db.GameContentSets.Where(cs => cs.SetName.Contains(userText, StringComparison.OrdinalIgnoreCase) && cs.IsPublic);

            successList = matches.Select(cs => new AutocompleteResult(cs.SetName, cs.Id)).AsEnumerable();

            return AutocompletionResult.FromSuccess(successList);
        }
        catch (Exception ex)
        {
            return AutocompletionResult.FromError(ex);
        }
    }
}

public class OwnerSubAutoComplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;

            if (string.IsNullOrWhiteSpace(userText)) return AutocompletionResult.FromSuccess(successList);

            var db = services.GetRequiredService<ApplicationContext>();
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            var matches = db.GameContentSets.Where(cs => cs.SetName.Contains(userText, StringComparison.OrdinalIgnoreCase) && cs.CreatorId == userId);

            successList = matches.Select(cs => new AutocompleteResult(cs.SetName, cs.Id)).AsEnumerable();

            return AutocompletionResult.FromSuccess(successList);
        }
        catch (Exception ex)
        {
            return AutocompletionResult.FromError(ex);
        }
    }
}
