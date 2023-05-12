using System.Text.RegularExpressions;
using Discord.Interactions;
using Server.Data;
using TheOracle2.Data;

namespace TheOracle2.Commands;

public class OracleAutocomplete : AutocompleteHandler
{
    private async Task<AutocompletionResult> GetEmptyOralceResult(IInteractionContext context, PlayerDataFactory dataFactory)
    {
        //Todo: remove results like "Faction" from "action"
        var oracles = (await dataFactory.GetPlayerOracles(context.User.Id)).Where(o => o.Name == "Pay the Price"
        || Regex.IsMatch(o.Category, @"\bAction", RegexOptions.IgnoreCase)
        || Regex.IsMatch(o.Category, @"\bCore", RegexOptions.IgnoreCase)).AsEnumerable()
        ;
        var list = oracles
            .SelectMany(x => GetOracleAutocompleteResults(x))
            .OrderBy(x =>
                x.Name == "Pay the Price" ? 1 :
                2)
            .Take(SelectMenuBuilder.MaxOptionCount);

        return AutocompletionResult.FromSuccess(list);
    }

    public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
    {
        var dataFactory = services.GetRequiredService<PlayerDataFactory>();

        try
        {
            List<AutocompleteResult> successList = new List<AutocompleteResult>();

            var value = autocompleteInteraction.Data.Current.Value as string;

            if (String.IsNullOrWhiteSpace(value))
            {
                return await GetEmptyOralceResult(context, dataFactory);
            }

            var oracles = await dataFactory.GetPlayerOracles(context.User.Id);

            successList.AddRange(oracles.GetOraclesFromUserInput(value)
                         .SelectMany(x => GetOracleAutocompleteResults(x)));

            return AutocompletionResult.FromSuccess(successList.Take(SelectMenuBuilder.MaxOptionCount));
        }
        catch (Exception ex)
        {
            return AutocompletionResult.FromError(ex);
        }
    }

    private IEnumerable<AutocompleteResult> GetOracleAutocompleteResults(Oracle oracle)
    {
        var list = new List<AutocompleteResult>();
        if (oracle.Oracles?.Count > 0)
        {
            if (oracle.Table?.Count > 0) list.Add(new AutocompleteResult(GetOracleDisplayName(oracle), oracle.Id));
            foreach (var t in oracle.Oracles)
            {
                list.Add(new AutocompleteResult(GetOracleDisplayName(oracle, t), t.Id));
            }
        }
        else
        {
            list.Add(new AutocompleteResult(GetOracleDisplayName(oracle), oracle.Id));
        }
        return list;
    }

    private string GetOracleDisplayName(Oracle oracle, Oracle subTable = null)
    {
        string name = oracle.Display?.Title ?? oracle.Name;
        var superTypeName = (oracle.Parent != null) ? oracle.Parent.Name : string.Empty;
        
        if (!string.IsNullOrWhiteSpace(oracle.Category))
        {
            superTypeName = oracle.Category[(oracle.Category.LastIndexOf("/") + 1)..].Replace("_", " ");
        }
        name += $" [{superTypeName}]";

        if (subTable != null) name += $" - {subTable.Name}";

        return name;
    }

    private string GetOracleDisplayName(OracleCategory cat, Oracle subTable = null)
    {
        string name = cat.Name;
        if (subTable != null) name += $" - {subTable.Name}";

        return name;
    }
}
