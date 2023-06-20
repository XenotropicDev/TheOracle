using Dataforged;
using Server.DiscordServer;

namespace Server.Data;

public interface IOracleRepository
{
    IEnumerable<OracleTable> GetOracles();

    OracleTable? GetOracleById(string id);
}

public static class OracleRepositoryExtenstions
{
    public static IEnumerable<OracleTable> GetOraclesFromUserInput(this IEnumerable<OracleTable> oracles, string query, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
    {
        var nameMatch = oracles.Where(x => x.Name.Contains(query, comparer) || x.Display?.Title?.Contains(query, comparer) == true);
        var parentNameMatch = oracles.Where(x => x.Parent?.Name.Contains(query, comparer) == true).Select(x => x.Parent);
        var parentNameOracles = parentNameMatch.SelectMany(p => p.Oracles);
        var categoryOracles = oracles.Where(o => o.Category?.Contains(query, comparer) == true);

        var parentAliasMatch = oracles.Where(x => x.Parent?.Aliases?.Any(s => s.Contains(query, comparer)) == true);
        var aliasMatch = oracles.Where(x => x.Aliases?.Any(s => s.Contains(query, comparer)) == true);

        return nameMatch
            .Concat(parentNameOracles)
            .Concat(categoryOracles)
            .Concat(parentAliasMatch)
            .Concat(aliasMatch)
            .DistinctBy(o => o.Id);
    }

    public static IDictionary<string, OracleCollection> GetOracleCollection(this OracleTable oracle, IOracleRepository repo)
    {
        return repo.
    }
}

public class PlayerOracleRepository : IOracleRepository
{
    public PlayerOracleRepository(ApplicationContext playerData)
    {
        PlayerData = playerData;
    }

    public ApplicationContext PlayerData { get; }

    public OracleTable? GetOracleById(string id)
    {
        if (int.TryParse(id, out var oracleId))
        {
            return GetOracles().FirstOrDefault(o => o.Id == oracleId);
        }

        return GetOracles().FirstOrDefault(o => o.JsonId == id);
    }

    public IEnumerable<OracleTable> GetOracles()
    {
        return PlayerData.Oracles;
    }
}

public class JsonOracleRepository : IOracleRepository
{
    private List<OracleCollection>? _oracles;

    public OracleTable? GetOracleById(string id)
    {
        var topLevelOracle = GetOracles().FirstOrDefault(o => o.JsonId == id);
        if (topLevelOracle != null) return topLevelOracle;

        var subOracle = GetOracles().SelectMany(o => o.Oracles?.Where(sub => sub.JsonId == id) ?? Array.Empty<OracleTable>()).FirstOrDefault();
        if (subOracle != null) return subOracle;

        var cats = GetOracleRoots().Where(or => or.Categories != null).SelectMany(or => or.Categories);
        var catOracle = cats.SelectMany(c => c.Oracles).FirstOrDefault(o => o.JsonId == id);
        if (catOracle != null) return catOracle;

        foreach (var cat in cats)
        {
            var oracle = findOracleRecursive(cat.Oracles, id);
            if (oracle != null) return oracle;
        }

        var partial = GetOracles().SingleOrDefault(o => o.JsonId.Contains(id))
            ?? GetOracles().SelectMany(o => o.Oracles?.Where(sub => sub.JsonId.Contains(id)) ?? Array.Empty<OracleTable>()).SingleOrDefault();

        return partial;
    }

    private OracleTable? findOracleRecursive(IEnumerable<OracleTable> oracles, string id)
    {
        if (oracles.Count() == 0) return null;
        uint.TryParse(id, out uint intId);
        foreach (var oracle in oracles)
        {
            if (intId != 0 && oracle.Id == intId) return oracle;
            if (oracle.JsonId == id) return oracle;

            if (oracle.Oracles != null)
            {
                var test = findOracleRecursive(oracle.Oracles, id);
                if (test != null) return test;
            }
        }

        return null;
    }

    public IEnumerable<OracleCollection> GetOracleRoots()
    {
        if (_oracles == null)
        {
            _oracles = new List<OracleCollection>();
            var files = new DirectoryInfo(Path.Combine("Data", "ironsworn")).GetFiles("*oracle*.json").ToList();
            files.AddRange(new DirectoryInfo(Path.Combine("Data", "starforged")).GetFiles("*oracle*.json").ToList());

            foreach (var file in files)
            {
                using var fileStream = file.OpenText();
                string text = fileStream.ReadToEnd();

                var root = JsonConvert.DeserializeObject<List<OracleCollection>>(text, new JsonSerializerSettings() { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });

                if (root != null) _oracles.AddRange(root);
            }

            foreach (var node in _oracles)
            {
                foreach (var oracle in node.Oracles)
                {
                    oracle.Parent = node;
                }

                if (node.Categories?.Count > 0)
                {
                    foreach (var cat in node.Categories)
                    {
                        foreach (var catOracle in cat.Oracles)
                        {
                            catOracle.Parent = node;
                        }
                    }
                }
            }
        }

        return _oracles;
    }

    public IEnumerable<OracleTable> GetOracles()
    {
        return GetOracleRoots().SelectMany(root => root.Oracles);
    }
}
