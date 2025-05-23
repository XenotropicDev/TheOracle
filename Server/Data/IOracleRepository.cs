using TheOracle2.Data;
using Server.GameInterfaces.DTOs; // Added for DTOs
using static Server.GameInterfaces.DTOs.OracleExtensions; // Added for ToDTO extensions
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO; // Added for Path, DirectoryInfo
using Newtonsoft.Json; // Added for JsonConvert

namespace Server.Data;

public interface IOracleRepository
{
    IEnumerable<OracleDTO> GetOracles(); // Changed
    IEnumerable<OracleRootDTO> GetOracleRoots(); // Changed
    IEnumerable<OracleCategoryDTO> GetOracleCategories(); // Added
    OracleDTO? GetOracleById(string id); // Changed
}

public static class OracleRepositoryExtenstions // Will be addressed later
{
    public static IEnumerable<Oracle> GetOraclesFromUserInput(this IEnumerable<Oracle> oracles, string query, StringComparison comparer = StringComparison.OrdinalIgnoreCase)
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
}

public class JsonOracleRepository : IOracleRepository
{
    private List<OracleRoot>? _oracleRootsStore; // Renamed field

    // Internal method to get raw Oracle entities
    private Oracle? GetOracleEntityById(string id)
    {
        // Check top-level oracles in roots
        var topLevelOracle = GetOraclesInternal().FirstOrDefault(o => o.Id == id);
        if (topLevelOracle != null) return topLevelOracle;

        // Check sub-oracles within top-level oracles
        var subOracle = GetOraclesInternal()
            .SelectMany(o => o.Oracles ?? Enumerable.Empty<Oracle>())
            .FirstOrDefault(sub => sub.Id == id);
        if (subOracle != null) return subOracle;
        
        // Check oracles within categories
        var categories = GetOracleRootsInternal().SelectMany(or => or.Categories ?? Enumerable.Empty<OracleCategory>());
        foreach (var cat in categories)
        {
            var oracleInCategory = findOracleRecursive(cat.Oracles, id);
            if (oracleInCategory != null) return oracleInCategory;
        }
        
        // Fallback for partial ID match (original logic) - consider if this is desired for DTOs
        // This might be better handled in a search-specific method rather than GetById
        var partial = GetOraclesInternal().SingleOrDefault(o => o.Id.Contains(id))
            ?? GetOraclesInternal().SelectMany(o => o.Oracles?.Where(sub => sub.Id.Contains(id)) ?? Array.Empty<Oracle>()).SingleOrDefault();
        
        return partial;
    }
    
    private string DetermineParentCategoryName(Oracle oracle)
    {
        if (oracle == null) return null;

        // If the oracle's Category property itself is a meaningful name (unlikely, as it's an ID)
        // For now, we find the category it belongs to.
        var allCategories = GetOracleRootsInternal().SelectMany(root => root.Categories ?? Enumerable.Empty<OracleCategory>());
        foreach (var category in allCategories)
        {
            if (category.Oracles?.Any(o => o.Id == oracle.Id) == true)
            {
                return category.Name;
            }
            // Check sub-oracles of oracles within this category
            if (category.Oracles != null) {
                foreach(var catOracle in category.Oracles) {
                    var foundInSubOracle = FindOracleAndGetParentNameRecursive(catOracle.Oracles, oracle.Id, catOracle.Name);
                    if(foundInSubOracle != null) return category.Name; // Return the top category name
                }
            }
        }
        
        // Check if it's a sub-oracle of a root-level oracle
        var rootOracles = GetOraclesInternal();
         foreach(var rootOracle in rootOracles) {
            var foundInSubOracle = FindOracleAndGetParentNameRecursive(rootOracle.Oracles, oracle.Id, rootOracle.Name);
            if(foundInSubOracle != null) return rootOracle.Name; // The parent oracle acts as category
        }

        return null; // No specific category found, or it's a root oracle
    }

    // Helper to find an oracle recursively and return its direct parent's name if found
    private Oracle FindOracleAndGetParentNameRecursive(List<Oracle> oraclesToSearch, string targetId, string currentParentName)
    {
        if (oraclesToSearch == null || !oraclesToSearch.Any()) return null;
        foreach (var oracle in oraclesToSearch)
        {
            if (oracle.Id == targetId) return oracle; // Found it

            var foundInChild = FindOracleAndGetParentNameRecursive(oracle.Oracles, targetId, oracle.Name);
            if (foundInChild != null) return foundInChild; // Found in sub-tree
        }
        return null;
    }


    public OracleDTO? GetOracleById(string id)
    {
        var oracleEntity = GetOracleEntityById(id);
        if (oracleEntity == null) return null;

        string parentCategoryName = DetermineParentCategoryName(oracleEntity);
        return oracleEntity.ToDTO(parentCategoryName);
    }

    private Oracle? findOracleRecursive(List<Oracle> oracles, string id) // Existing recursive helper
    {
        if (oracles == null || oracles.Count == 0) return null; // Added null check
        foreach (var oracle in oracles)
        {
            if (oracle.Id == id) return oracle;

            if (oracle.Oracles != null)
            {
                var test = findOracleRecursive(oracle.Oracles, id);
                if (test != null) return test;
            }
        }
        return null;
    }

    private IEnumerable<OracleRoot> GetOracleRootsInternal() // Renamed
    {
        if (_oracleRootsStore == null)
        {
            _oracleRootsStore = new List<OracleRoot>();
            var files = new List<FileInfo>();
            var ironswornPath = Path.Combine("Data", "ironsworn");
            if(Directory.Exists(ironswornPath)) files.AddRange(new DirectoryInfo(ironswornPath).GetFiles("*oracle*.json"));
            
            var starforgedPath = Path.Combine("Data", "starforged");
            if(Directory.Exists(starforgedPath)) files.AddRange(new DirectoryInfo(starforgedPath).GetFiles("*oracle*.json"));

            foreach (var file in files)
            {
                using var fileStream = file.OpenText();
                string text = fileStream.ReadToEnd();
                var rootList = JsonConvert.DeserializeObject<List<OracleRoot>>(text, new JsonSerializerSettings() { MetadataPropertyHandling = MetadataPropertyHandling.Ignore });
                if (rootList != null) _oracleRootsStore.AddRange(rootList);
            }

            foreach (var node in _oracleRootsStore) // Original parent assignment logic
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
                            catOracle.Parent = node; // This seems to assign the OracleRoot as parent, not the OracleCategory
                        }
                    }
                }
            }
        }
        return _oracleRootsStore;
    }
    
    public IEnumerable<OracleRootDTO> GetOracleRoots() // New public method
    {
        return GetOracleRootsInternal().Select(root => root.ToDTO());
    }

    private IEnumerable<Oracle> GetOraclesInternal() // Renamed
    {
        // This returns top-level oracles directly under roots
        return GetOracleRootsInternal().SelectMany(root => root.Oracles ?? Enumerable.Empty<Oracle>());
    }

    public IEnumerable<OracleDTO> GetOracles() // New public method
    {
        // For top-level oracles, parentCategoryName is null
        return GetOraclesInternal().Select(oracle => oracle.ToDTO(parentCategoryName: null));
    }

    public IEnumerable<OracleCategoryDTO> GetOracleCategories() // New public method
    {
        return GetOracleRootsInternal()
               .SelectMany(root => root.Categories ?? Enumerable.Empty<OracleCategory>())
               .Select(category => category.ToDTO());
    }
}
