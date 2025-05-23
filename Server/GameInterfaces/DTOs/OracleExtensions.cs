using TheOracle2.Data; // For Oracle, OracleCategory, Table, Source
using Server.GameInterfaces.DTOs; // For OracleDTO, OracleCategoryDTO, OracleTableEntryDTO, SourceDTO
using System.Linq;
using System.Collections.Generic;

namespace Server.GameInterfaces.DTOs
{
    public static class OracleExtensions
    {
        // Local ToDTO for Source
        public static SourceDTO ToDTO(this Source source)
        {
            if (source == null)
            {
                return null;
            }
            return new SourceDTO
            {
                Name = source.Name,
                Url = null // Or string.Empty as per instruction
            };
        }

        // ToDTO for Table (OracleTableEntry)
        public static OracleTableEntryDTO ToDTO(this Table tableEntry)
        {
            if (tableEntry == null)
            {
                return null;
            }

            return new OracleTableEntryDTO
            {
                Id = tableEntry.Id, // Added Id mapping
                Floor = tableEntry.Floor,
                Ceiling = tableEntry.Ceiling,
                ResultText = tableEntry.Result
            };
        }

        // ToDTO for Oracle
        public static OracleDTO ToDTO(this Oracle oracle, string parentCategoryName = null)
        {
            if (oracle == null)
            {
                return null;
            }

            var oracleDto = new OracleDTO
            {
                Id = oracle.Id,
                Name = oracle.Name,
                Description = oracle.Description,
                CategoryId = oracle.Category, // This is the Oracle's own category ID string
                CategoryName = parentCategoryName, // Passed in, could be null
                Source = oracle.Source?.ToDTO(),
                Aliases = oracle.Aliases != null ? new List<string>(oracle.Aliases) : new List<string>()
            };

            if (oracle.Table != null)
            {
                oracleDto.Table = oracle.Table.Select(t => t.ToDTO()).ToList();
            }
            else
            {
                oracleDto.Table = new List<OracleTableEntryDTO>();
            }

            if (oracle.Oracles != null) // Sub-oracles
            {
                // Pass current oracle's name as parentCategoryName for sub-oracles,
                // or if they have their own category, that would be handled if Oracle.Category was an object.
                // Given Oracle.Category is a string ID, using oracle.Name seems appropriate for context.
                oracleDto.Oracles = oracle.Oracles.Select(subOracle => subOracle.ToDTO(parentCategoryName: oracle.Name)).ToList();
            }
            else
            {
                oracleDto.Oracles = new List<OracleDTO>();
            }

            return oracleDto;
        }

        // ToDTO for OracleCategory
        public static OracleCategoryDTO ToDTO(this OracleCategory category)
        {
            if (category == null)
            {
                return null;
            }

            var categoryDto = new OracleCategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Source = category.Source?.ToDTO(),
                Aliases = category.Aliases != null ? new List<string>(category.Aliases) : new List<string>()
            };

            if (category.Oracles != null)
            {
                categoryDto.Oracles = category.Oracles.Select(o => o.ToDTO(parentCategoryName: category.Name)).ToList();
            }
            else
            {
                categoryDto.Oracles = new List<OracleDTO>();
            }

            return categoryDto;
        }

        // ToDTO for OracleRoot
        public static OracleRootDTO ToDTO(this OracleRoot root)
        {
            if (root == null)
            {
                return null;
            }

            var rootDto = new OracleRootDTO
            {
                Id = root.Id,
                Name = root.Name,
                Description = root.Description,
                Source = root.Source?.ToDTO(),
                Aliases = root.Aliases != null ? new List<string>(root.Aliases) : new List<string>()
            };

            if (root.Categories != null)
            {
                rootDto.Categories = root.Categories.Select(c => c.ToDTO()).ToList();
            }
            else
            {
                rootDto.Categories = new List<OracleCategoryDTO>();
            }

            if (root.Oracles != null) // Top-level oracles
            {
                // Passing null as parentCategoryName because these are root-level oracles
                rootDto.Oracles = root.Oracles.Select(o => o.ToDTO(parentCategoryName: null)).ToList();
            }
            else
            {
                rootDto.Oracles = new List<OracleDTO>();
            }

            return rootDto;
        }
    }
}
