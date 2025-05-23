using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.DiscordServer;
using TheOracle2;
using TheOracle2.Data;
using Server.GameInterfaces.DTOs; // Added for DTOs
using static Server.GameInterfaces.DTOs.MoveExtensions; // Added for ToDTO extensions
using Newtonsoft.Json; // Added for JsonConvert
using System.IO; // Added for Path and DirectoryInfo

namespace Server.Data;

public interface IMoveRepository
{
    IEnumerable<MoveDTO> GetMoves(); // Changed return type
    // IEnumerable<MoveRoot> GetMoveRoots(); // Commented out
    MoveDTO? GetMove(string id); // Changed return type
}

public class JsonMoveRepository : IMoveRepository
{
    private List<MoveRoot>? _moveRootsStore; // Renamed field

    private IEnumerable<MoveRoot> GetMoveRootsInternal() // Renamed to Internal
    {
        if (_moveRootsStore == null)
        {
            _moveRootsStore = new List<MoveRoot>();
            var files = new List<FileInfo>();

            var ironswornPath = Path.Combine("Data", "ironsworn");
            if (Directory.Exists(ironswornPath))
            {
                files.AddRange(new DirectoryInfo(ironswornPath).GetFiles("*moves*.json"));
            }
            
            var starforgedPath = Path.Combine("Data", "starforged");
            if (Directory.Exists(starforgedPath))
            {
                 files.AddRange(new DirectoryInfo(starforgedPath).GetFiles("*moves*.json"));
            }
            
            foreach (var file in files)
            {
                using var fileStream = file.OpenText();
                string text = fileStream.ReadToEnd();
                var rootList = JsonConvert.DeserializeObject<List<MoveRoot>>(text, new JsonSerializerSettings() { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (rootList != null) _moveRootsStore.AddRange(rootList);
            }

            foreach (var node in _moveRootsStore)
            {
                foreach (var moveItem in node.Moves) // Renamed Move to moveItem
                {
                    moveItem.Parent = node;
                }
            }
        }
        return _moveRootsStore;
    }

    // public IEnumerable<MoveRoot> GetMoveRoots() // Commented out public version
    // {
    //     return GetMoveRootsInternal();
    // }

    private IEnumerable<Move> GetMovesInternal() // Renamed to Internal
    {
        return GetMoveRootsInternal().SelectMany(root => root.Moves);
    }

    public MoveDTO? GetMove(string id) // Updated signature
    {
        var moveEntity = GetMovesInternal().FirstOrDefault(o => o.Id == id);
        return moveEntity?.ToDTO(); // Use extension method
    }

    public IEnumerable<MoveDTO> GetMoves() // Updated signature
    {
        return GetMovesInternal().Select(move => move.ToDTO()); // Use extension method
    }
}
