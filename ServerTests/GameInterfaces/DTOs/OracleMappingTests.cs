using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.Data; // Assuming this is the correct namespace for Oracle, Table, etc.
using Server.GameInterfaces.DTOs; // For OracleDTO, OracleTableEntryDTO, etc. and OracleExtensions
using System.Collections.Generic;
using System.Linq;

// For testing purposes, if TheOracle2.Data.Source is not fully defined or accessible
// in a way that suits the test, a minimal version might be assumed.
// However, OracleExtensions.cs uses `TheOracle2.Data.Source` directly.

namespace ServerTests.GameInterfaces.DTOs
{
    [TestClass]
    public class OracleMappingTests
    {
        [TestMethod]
        public void TestOracleTableEntryToDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var tableEntry = new Table // Assuming TheOracle2.Data.Table is the source type
            {
                Id = "test_table_entry_id",
                Floor = 1,
                Ceiling = 10,
                Result = "Test Result Text"
            };

            // Act
            var dto = tableEntry.ToDTO();

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(tableEntry.Id, dto.Id);
            Assert.AreEqual(tableEntry.Floor, dto.Floor);
            Assert.AreEqual(tableEntry.Ceiling, dto.Ceiling);
            Assert.AreEqual(tableEntry.Result, dto.ResultText);
        }

        [TestMethod]
        public void TestOracleTableEntryToDTOMapping_NullInput_ShouldReturnNull()
        {
            // Arrange
            Table tableEntry = null;

            // Act
            var dto = tableEntry.ToDTO();

            // Assert
            Assert.IsNull(dto);
        }

        [TestMethod]
        public void TestOracleToDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var oracle = new Oracle
            {
                Id = "oracle_id_1",
                Name = "Test Oracle",
                Description = "An oracle for testing",
                Category = "parent_category_id_string", // String ID
                Source = new Source { Name = "Core Source" },
                Aliases = new List<string> { "alias1", "alias2" },
                Table = new List<Table>
                {
                    new Table { Id = "entry1", Floor = 1, Ceiling = 50, Result = "Result A" },
                    new Table { Id = "entry2", Floor = 51, Ceiling = 100, Result = "Result B" }
                },
                Oracles = new List<Oracle> // Sub-oracles
                {
                    new Oracle { Id = "sub_oracle_1", Name = "Sub Oracle 1", Category = "oracle_id_1" }
                }
            };
            string parentCategoryName = "Test Parent Category Name";

            // Act
            var dto = oracle.ToDTO(parentCategoryName);

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(oracle.Id, dto.Id);
            Assert.AreEqual(oracle.Name, dto.Name);
            Assert.AreEqual(oracle.Description, dto.Description);
            Assert.AreEqual(oracle.Category, dto.CategoryId); // Direct mapping of string ID
            Assert.AreEqual(parentCategoryName, dto.CategoryName); // Passed parameter
            
            Assert.IsNotNull(dto.Source);
            Assert.AreEqual(oracle.Source.Name, dto.Source.Name);
            Assert.IsNull(dto.Source.Url);

            Assert.IsNotNull(dto.Aliases);
            Assert.AreEqual(2, dto.Aliases.Count);
            Assert.IsTrue(dto.Aliases.Contains("alias1"));

            Assert.IsNotNull(dto.Table);
            Assert.AreEqual(2, dto.Table.Count);
            Assert.AreEqual(oracle.Table[0].Result, dto.Table[0].ResultText);
            Assert.AreEqual(oracle.Table[1].Id, dto.Table[1].Id);

            Assert.IsNotNull(dto.Oracles);
            Assert.AreEqual(1, dto.Oracles.Count);
            Assert.AreEqual(oracle.Oracles[0].Id, dto.Oracles[0].Id);
            Assert.AreEqual(oracle.Name, dto.Oracles[0].CategoryName); // Sub-oracle's CategoryName is parent oracle's name
        }

        [TestMethod]
        public void TestOracleToDTOMapping_NullInput_ShouldReturnNull()
        {
            // Arrange
            Oracle oracle = null;

            // Act
            var dto = oracle.ToDTO();

            // Assert
            Assert.IsNull(dto);
        }

        [TestMethod]
        public void TestOracleToDTOMapping_NullAndEmptyCollectionsAndNestedObjects_ShouldHandleGracefully()
        {
            // Arrange
            var oracle = new Oracle
            {
                Id = "oracle_id_2",
                Name = "Minimal Oracle",
                Source = null,
                Table = null, // Null list
                Oracles = new List<Oracle>(), // Empty list
                Aliases = null // Null list
            };

            // Act
            var dto = oracle.ToDTO("SomeCategory");

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(oracle.Name, dto.Name);
            Assert.IsNull(dto.Source);
            
            Assert.IsNotNull(dto.Table); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Table.Count);
            
            Assert.IsNotNull(dto.Oracles); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Oracles.Count);

            Assert.IsNotNull(dto.Aliases); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Aliases.Count);
        }

        [TestMethod]
        public void TestOracleCategoryToDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var category = new OracleCategory
            {
                Id = "cat_id_1",
                Name = "Test Category",
                Description = "A category for testing",
                Source = new Source { Name = "Category Source" },
                Aliases = new List<string> { "cat_alias1" },
                Oracles = new List<Oracle>
                {
                    new Oracle { Id = "oracle_in_cat_1", Name = "Oracle 1 in Cat", Category = "cat_id_1" },
                    new Oracle { Id = "oracle_in_cat_2", Name = "Oracle 2 in Cat", Category = "cat_id_1" }
                }
            };

            // Act
            var dto = category.ToDTO();

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(category.Id, dto.Id);
            Assert.AreEqual(category.Name, dto.Name);
            Assert.AreEqual(category.Description, dto.Description);

            Assert.IsNotNull(dto.Source);
            Assert.AreEqual(category.Source.Name, dto.Source.Name);
            Assert.IsNull(dto.Source.Url);

            Assert.IsNotNull(dto.Aliases);
            Assert.AreEqual(1, dto.Aliases.Count);
            Assert.IsTrue(dto.Aliases.Contains("cat_alias1"));

            Assert.IsNotNull(dto.Oracles);
            Assert.AreEqual(2, dto.Oracles.Count);
            Assert.AreEqual(category.Oracles[0].Id, dto.Oracles[0].Id);
            Assert.AreEqual(category.Name, dto.Oracles[0].CategoryName); // Oracle's CategoryName is parent category's name
        }

        [TestMethod]
        public void TestOracleCategoryToDTOMapping_NullInput_ShouldReturnNull()
        {
            // Arrange
            OracleCategory category = null;

            // Act
            var dto = category.ToDTO();

            // Assert
            Assert.IsNull(dto);
        }

        [TestMethod]
        public void TestOracleCategoryToDTOMapping_NullAndEmptyCollectionsAndNestedObjects_ShouldHandleGracefully()
        {
            // Arrange
            var category = new OracleCategory
            {
                Id = "cat_id_2",
                Name = "Minimal Category",
                Source = null,
                Oracles = null, // Null list
                Aliases = new List<string>() // Empty list
            };

            // Act
            var dto = category.ToDTO();

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(category.Name, dto.Name);
            Assert.IsNull(dto.Source);

            Assert.IsNotNull(dto.Oracles); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Oracles.Count);

            Assert.IsNotNull(dto.Aliases); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Aliases.Count);
        }

        [TestMethod]
        public void TestOracleRootToDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var root = new OracleRoot
            {
                Id = "root_id_1",
                Name = "Test Oracle Root",
                Description = "A root for testing",
                Source = new Source { Name = "Root Source" },
                Aliases = new List<string> { "root_alias" },
                Categories = new List<OracleCategory>
                {
                    new OracleCategory 
                    { 
                        Id = "cat_in_root_1", 
                        Name = "Category in Root", 
                        Oracles = new List<Oracle> 
                        { 
                            new Oracle { Id = "oracle_in_cat_in_root", Name = "Oracle in Cat in Root", Category = "cat_in_root_1" } 
                        } 
                    }
                },
                Oracles = new List<Oracle> // Top-level oracles
                {
                    new Oracle { Id = "top_level_oracle_1", Name = "Top Level Oracle 1" }
                }
            };

            // Act
            var dto = root.ToDTO();

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(root.Id, dto.Id);
            Assert.AreEqual(root.Name, dto.Name);
            Assert.AreEqual(root.Description, dto.Description);

            Assert.IsNotNull(dto.Source);
            Assert.AreEqual(root.Source.Name, dto.Source.Name);
            Assert.IsNull(dto.Source.Url);

            Assert.IsNotNull(dto.Aliases);
            Assert.AreEqual(1, dto.Aliases.Count);
            Assert.IsTrue(dto.Aliases.Contains("root_alias"));

            Assert.IsNotNull(dto.Categories);
            Assert.AreEqual(1, dto.Categories.Count);
            Assert.AreEqual(root.Categories[0].Id, dto.Categories[0].Id);
            Assert.IsNotNull(dto.Categories[0].Oracles);
            Assert.AreEqual(1, dto.Categories[0].Oracles.Count);
            Assert.AreEqual(root.Categories[0].Oracles[0].Id, dto.Categories[0].Oracles[0].Id);
            Assert.AreEqual(root.Categories[0].Name, dto.Categories[0].Oracles[0].CategoryName); // CategoryName from parent category

            Assert.IsNotNull(dto.Oracles);
            Assert.AreEqual(1, dto.Oracles.Count);
            Assert.AreEqual(root.Oracles[0].Id, dto.Oracles[0].Id);
            Assert.IsNull(dto.Oracles[0].CategoryName); // Top-level oracle in root has null CategoryName
        }

        [TestMethod]
        public void TestOracleRootToDTOMapping_NullInput_ShouldReturnNull()
        {
            // Arrange
            OracleRoot root = null;

            // Act
            var dto = root.ToDTO();

            // Assert
            Assert.IsNull(dto);
        }

        [TestMethod]
        public void TestOracleRootToDTOMapping_NullAndEmptyCollectionsAndNestedObjects_ShouldHandleGracefully()
        {
            // Arrange
            var root = new OracleRoot
            {
                Id = "root_id_2",
                Name = "Minimal Root",
                Source = null,
                Categories = null, // Null list
                Oracles = new List<Oracle>(), // Empty list
                Aliases = null // Null list
            };

            // Act
            var dto = root.ToDTO();

            // Assert
            Assert.IsNotNull(dto);
            Assert.AreEqual(root.Name, dto.Name);
            Assert.IsNull(dto.Source);

            Assert.IsNotNull(dto.Categories); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Categories.Count);

            Assert.IsNotNull(dto.Oracles); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Oracles.Count);

            Assert.IsNotNull(dto.Aliases); // Should be initialized to empty list
            Assert.AreEqual(0, dto.Aliases.Count);
        }
    }
}
