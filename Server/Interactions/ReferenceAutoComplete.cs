using Discord.Interactions;
using Server.Data;
using Server.DiscordServer;

namespace TheOracle2;

public class ReferenceAutoComplete : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            var moves = services.GetRequiredService<IMoveRepository>();
            var factory = services.GetRequiredService<PlayerDataFactory>();

            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            if (moves == null) return Task.FromResult(AutocompletionResult.FromSuccess(successList));

            if (userText?.Length > 0)
            {
                    successList = factory.GetPlayerMoves(context.User.Id)
                        .Where(m => m.Name.Contains(userText, StringComparison.OrdinalIgnoreCase) || m.Category.Contains(userText, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(m => m.Name)
                        .Take(SelectMenuBuilder.MaxOptionCount)
                        .Select(m => new AutocompleteResult($"{m.Name} [{m.Parent?.Name ?? m.Category}]", m.Id.ToString())).AsEnumerable();
            }
            else
            {
                var initialMoves = new List<string> { "Face Danger", "Secure an Advantage" };
                successList = factory.GetPlayerMoves(context.User.Id).Where(m => initialMoves.Any(im => m.Name.Contains(im, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(m => m.Name)
                    .Take(SelectMenuBuilder.MaxOptionCount)
                    .Select(m => new AutocompleteResult($"{m.Name} [{m.Parent?.Name ?? m.Category}]", m.Id.ToString())).AsEnumerable();
            }

            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}
