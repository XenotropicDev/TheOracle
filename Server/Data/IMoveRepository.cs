using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dataforged;
using Server.DiscordServer;
using TheOracle2;

namespace Server.Data;

public interface IMoveRepository
{
    IEnumerable<Move> GetMoves();
    IEnumerable<MoveCategory> GetMoveRoots();

    Move? GetMove(string id);
}

public class JsonMoveRepository : IMoveRepository
{
    private List<MoveCategory>? _moves;

    public Move? GetMove(string id)
    {
        return GetMoves().FirstOrDefault(o => o.JsonId == id);
    }

    public IEnumerable<MoveCategory> GetMoveRoots()
    {
        if (_moves == null)
        {
            _moves = new List<MoveCategory>();
            var files = new DirectoryInfo(Path.Combine("Data", "ironsworn")).GetFiles("*moves*.json").ToList();
            files.AddRange(new DirectoryInfo(Path.Combine("Data", "starforged")).GetFiles("*moves*.json").ToList());

            foreach (var file in files)
            {
                using var fileStream = file.OpenText();
                string text = fileStream.ReadToEnd();

                var root = JsonConvert.DeserializeObject<List<MoveCategory>>(text, new JsonSerializerSettings() { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });

                if (root != null) _moves.AddRange(root);
            }
        }

        return _moves;
    }

    public IEnumerable<Move> GetMoves()
    {
        return GetMoveRoots().SelectMany(root => root.Moves);
    }
}
