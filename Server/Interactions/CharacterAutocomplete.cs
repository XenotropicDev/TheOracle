using System.Text.RegularExpressions;
using Discord.Interactions;
using Server.DiscordServer;
using TheOracle2.UserContent;

namespace TheOracle2;

public class CharacterAutocomplete : AutocompleteHandler
{
    public ApplicationContext Db { get; set; }

    public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            //Todo: might want something to limit results when only a few characters have been typed (regex \b{userText}?)
            switch (userText.Length)
            {
                case > 0:
                    {
                        successList = Db.PlayerCharacters
                            .Where((pc) => pc.DiscordGuildId == guildId && pc.Name.Contains(userText, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(pc => pc.UserId != userId).ThenBy(pc => pc.Name)
                            .Take(SelectMenuBuilder.MaxOptionCount)
                            .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
                    }
                    break;

                default:
                    {
                        // fallback to list of users own guild PCs, sorted alphabetically
                        successList = Db.PlayerCharacters
                            .Where((pc) => pc.DiscordGuildId == guildId && pc.UserId == userId)
                            .OrderBy(pc => pc.Name)
                            .Take(SelectMenuBuilder.MaxOptionCount)
                            .Select(pc => new AutocompleteResult(pc.Name, pc.Id.ToString())).AsEnumerable();
                    }
                    break;
            }
            return Task.FromResult(AutocompletionResult.FromSuccess(successList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(AutocompletionResult.FromError(ex));
        }
    }
}
