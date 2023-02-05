using Discord.Interactions;
using Server.Data;
using Server.DiscordServer;

namespace TheOracle2;

public class ReferenceAutoComplete : AutocompleteHandler
{
    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            var moves = services.GetRequiredService<IMoveRepository>();
            var factory = services.GetRequiredService<PlayerDataFactory>();

            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            if (moves == null) return AutocompletionResult.FromSuccess(successList);

            if (userText?.Length > 0)
            {
                var game = IronGameExtenstions.GetIronGameInString(userText);
                userText = IronGameExtenstions.RemoveIronGameInString(userText);
                successList = (await factory.GetPlayerMoves(context.User.Id, game))
                        .Where(m => m.Name.Contains(userText, StringComparison.OrdinalIgnoreCase) || m.Category.Contains(userText, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(m => m.Name)
                        .Take(SelectMenuBuilder.MaxOptionCount)
                        .Select(m => new AutocompleteResult($"{m.Name} [{m.Parent?.Name ?? m.Category}]", m.Id.ToString())).AsEnumerable();
            }
            else
            {
                var initialMoves = new List<string> { "Face Danger", "Secure an Advantage" };
                successList = (await factory.GetPlayerMoves(context.User.Id)).Where(m => initialMoves.Any(im => m.Name.Contains(im, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(m => m.Name)
                    .Take(SelectMenuBuilder.MaxOptionCount)
                    .Select(m => new AutocompleteResult($"{m.Name} [{m.Parent?.Name ?? m.Category}]", m.Id.ToString())).AsEnumerable();
            }

            return AutocompletionResult.FromSuccess(successList);
        }
        catch (Exception ex)
        {
            return AutocompletionResult.FromError(ex);
        }
    }
}
