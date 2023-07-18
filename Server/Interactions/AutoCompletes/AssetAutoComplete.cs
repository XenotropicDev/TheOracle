using Discord.Interactions;
using Server.Data;
using Server.DiscordServer;

namespace TheOracle2;

public class AssetAutoComplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var playerData = services.GetRequiredService<PlayerDataFactory>();
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            if (playerData == null) return (AutocompletionResult.FromSuccess(successList));

            if (userText?.Length > 0)
            {
                var game = IronGameExtenstions.GetIronGameInString(userText);
                userText = IronGameExtenstions.RemoveIronGameInString(userText);
                successList = (await playerData.GetPlayerAssets(context.User.Id, game))
                        .Where(a => a.Name.Contains(userText, StringComparison.OrdinalIgnoreCase) || a.Parent?.Name.Contains(userText, StringComparison.OrdinalIgnoreCase) == true)
                        .OrderBy(a => a.Name)
                        .Take(SelectMenuBuilder.MaxOptionCount)
                        .Select(m => new AutocompleteResult($"{m.Name} [{m.Parent?.Name}]", m.Id.ToString())).AsEnumerable();
            }

            return (AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return (AutocompletionResult.FromError(ex));
        }
    }
}
