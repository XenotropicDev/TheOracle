﻿using Discord.Interactions;
using Server.Data;
using Server.DiscordServer;

namespace TheOracle2;

public class AssetAutoComplete : AutocompleteHandler
{
    private readonly ApplicationContext db;

    public AssetAutoComplete(ApplicationContext db)
    {
        this.db = db;
    }

    public PlayerDataFactory? Assets { get; set; }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        try
        {
            IEnumerable<AutocompleteResult> successList = new List<AutocompleteResult>();
            var userText = autocompleteInteraction.Data.Current.Value as string;
            var userId = autocompleteInteraction.User.Id;
            var guildId = context.Guild?.Id ?? autocompleteInteraction.User.Id;

            if (Assets == null) return (AutocompletionResult.FromSuccess(successList));

            if (userText?.Length > 0)
            {
                    successList = Assets.GetPlayerAssets(context.User.Id)
                        .Where(m => m.Name.Contains(userText, StringComparison.OrdinalIgnoreCase) || m.Parent?.Name.Contains(userText, StringComparison.OrdinalIgnoreCase) == true)
                        .OrderBy(m => m.Name)
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
