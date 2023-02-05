using Discord.Interactions;
using Server.Data;

namespace TheOracle2;

public class EntityAutoComplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var dataFactory = services.GetRequiredService<PlayerDataFactory>();
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            if (userText?.Length > 0)
            {
                var game = IronGameExtenstions.GetIronGameInString(userText);
                userText = IronGameExtenstions.RemoveIronGameInString(userText);
                var entites = await dataFactory.GetPlayerEntites(userId, game);

                successList = entites.Where(m => m.SearchName.Contains(userText, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(m => m.SearchName)
                        .Take(SelectMenuBuilder.MaxOptionCount)
                        .Select(m => new AutocompleteResult(m.SearchName, m.Id.ToString())).AsEnumerable();
            }
            else
            {
                var entites = await dataFactory.GetPlayerEntites(userId);

                successList = entites
                    .OrderBy(m => m.SearchName)
                    .Take(SelectMenuBuilder.MaxOptionCount)
                    .Select(m => new AutocompleteResult(m.SearchName, m.Id.ToString())).AsEnumerable();
            }

            return (AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return (AutocompletionResult.FromError(ex));
        }
    }
}
