using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle2.Data;
using Server.GameInterfaces.DTOs; // Existing using statement
using Newtonsoft.Json; // Added for JsonConvert
using System.IO; // Added for Path and DirectoryInfo

namespace Server.Data;

public interface IAssetRepository
{
    IEnumerable<AssetDTO> GetAssets(); // Return type is AssetDTO
    // IEnumerable<AssetRoot> GetAssetRoots(); // Commented out
    AssetDTO? GetAsset(string id); // Return type is AssetDTO?
}

public class JsonAssetRepository : IAssetRepository
{
    private List<AssetRoot>? _assetRootsStore; // Renamed field for clarity

    // Private method to load and cache asset roots
    private IEnumerable<AssetRoot> GetAssetRootsInternal()
    {
        if (_assetRootsStore == null)
        {
            _assetRootsStore = new List<AssetRoot>();
            var files = new List<FileInfo>();
            
            var ironswornAssetPath = Path.Combine("Data", "ironsworn");
            if (Directory.Exists(ironswornAssetPath))
            {
                files.AddRange(new DirectoryInfo(ironswornAssetPath).GetFiles("*assets*.json"));
            }

            var starforgedAssetPath = Path.Combine("Data", "starforged");
            if (Directory.Exists(starforgedAssetPath))
            {
                files.AddRange(new DirectoryInfo(starforgedAssetPath).GetFiles("*assets*.json"));
            }

            foreach (var file in files)
            {
                using var fileStream = file.OpenText();
                string text = fileStream.ReadToEnd();
                var rootList = JsonConvert.DeserializeObject<List<AssetRoot>>(text, new JsonSerializerSettings() { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (rootList != null)
                {
                    _assetRootsStore.AddRange(rootList);
                }
            }

            foreach (var node in _assetRootsStore)
            {
                foreach (var assetItem in node.Assets) // assetItem to avoid conflict with Asset class
                {
                    assetItem.Parent = node;
                }
            }
        }
        return _assetRootsStore;
    }

    // Commented out public GetAssetRoots as per instructions
    // public IEnumerable<AssetRoot> GetAssetRoots()
    // {
    //     return GetAssetRootsInternal();
    // }

    // Private method to get all raw Asset entities
    private IEnumerable<Asset> GetAssetsInternal()
    {
        return GetAssetRootsInternal().SelectMany(root => root.Assets);
    }

    public AssetDTO? GetAsset(string id)
    {
        var assetEntity = GetAssetsInternal().FirstOrDefault(o => o.Id == id);
        return assetEntity?.ToDTO(); // Convert to DTO after retrieval
    }

    public IEnumerable<AssetDTO> GetAssets()
    {
        return GetAssetsInternal().Select(asset => asset.ToDTO()); // Convert to DTOs after retrieval
    }
}
