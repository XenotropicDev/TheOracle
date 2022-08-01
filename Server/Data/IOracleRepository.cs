using Server.DiscordServer;
using TheOracle2;
using TheOracle2.Data;

namespace Server.Data;

public interface IOracleRepository
{
    IEnumerable<Oracle> GetOracles();
    IEnumerable<OracleRoot> GetOracleRoots();

    Oracle? GetOracleById(string id);

    //void CreateOracle(int id);
    //void UpdateOracle(int id);
    //void DeleteOracle(int id);
}

public class JsonOracleRepository : IOracleRepository
{
    private List<OracleRoot>? _oracles;

    public Oracle? GetOracleById(string id)
    {
        var topLevelOracle = GetOracles().FirstOrDefault(o => o.Id == id);
        if (topLevelOracle != null) return topLevelOracle;

        var subOracle = GetOracles().SelectMany(o => o.Oracles?.Where(sub => sub.Id == id) ?? Array.Empty<Oracle>()).FirstOrDefault();
        if (subOracle != null) return subOracle;

        var partial = GetOracles().SingleOrDefault(o => o.Id.Contains(id))
            ?? GetOracles().SelectMany(o => o.Oracles?.Where(sub => sub.Id.Contains(id)) ?? Array.Empty<Oracle>()).SingleOrDefault();

        return partial;
    }

    public IEnumerable<OracleRoot> GetOracleRoots()
    {
        if (_oracles == null)
        {
            _oracles = new List<OracleRoot>();
            var files = new DirectoryInfo(Path.Combine("Data", "ironsworn")).GetFiles("*oracle*.json").ToList();
            files.AddRange(new DirectoryInfo(Path.Combine("Data", "starforged")).GetFiles("*oracle*.json").ToList());

            foreach (var file in files)
            {
                using var fileStream = file.OpenText();
                string text = fileStream.ReadToEnd();

                var root = JsonConvert.DeserializeObject<List<OracleRoot>>(text, new JsonSerializerSettings() { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                
                if (root != null) _oracles.AddRange(root);
            }

            foreach (var node in _oracles)
            {
                foreach (var oracle in node.Oracles)
                {
                    oracle.Parent = node;
                }
            }
        }

        return _oracles;
    }

    public IEnumerable<Oracle> GetOracles()
    {
        return GetOracleRoots().SelectMany(root => root.Oracles);
    }
}
