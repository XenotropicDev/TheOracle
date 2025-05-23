using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheOracle2.Data; // Assuming this is the correct namespace for Asset, Ability, etc.
using Server.GameInterfaces.DTOs; // For AssetDTO, AbilityDTO, etc. and AssetExtensions
using System.Collections.Generic;
using System.Linq;

namespace ServerTests.GameInterfaces.DTOs
{
    [TestClass]
    public class AssetMappingTests
    {
        [TestMethod]
        public void TestAbilityToAbilityDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var ability = new Ability
            {
                Id = "test_ability_id",
                Name = "Test Ability",
                Text = "This is a test ability.",
                Enabled = true
            };

            // Act
            var abilityDto = ability.ToDTO();

            // Assert
            Assert.IsNotNull(abilityDto);
            Assert.AreEqual(ability.Id, abilityDto.Id);
            Assert.AreEqual(ability.Name, abilityDto.Name);
            Assert.AreEqual(ability.Text, abilityDto.Text);
            Assert.AreEqual(ability.Enabled, abilityDto.Enabled);
        }

        [TestMethod]
        public void TestAbilityToAbilityDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            Ability ability = null;

            // Act
            var abilityDto = ability.ToDTO();

            // Assert
            Assert.IsNull(abilityDto);
        }

        [TestMethod]
        public void TestConditionMeterToConditionMeterDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var conditionMeter = new ConditionMeter
            {
                Id = "test_cm_id",
                Name = "Test Condition Meter",
                Value = 5,
                Min = 0,
                Max = 10
            };

            // Act
            var conditionMeterDto = conditionMeter.ToDTO();

            // Assert
            Assert.IsNotNull(conditionMeterDto);
            Assert.AreEqual(conditionMeter.Id, conditionMeterDto.Id);
            Assert.AreEqual(conditionMeter.Name, conditionMeterDto.Name);
            Assert.AreEqual(conditionMeter.Value, conditionMeterDto.Value);
            Assert.AreEqual(conditionMeter.Min, conditionMeterDto.Min);
            Assert.AreEqual(conditionMeter.Max, conditionMeterDto.Max);
        }

        [TestMethod]
        public void TestConditionMeterToConditionMeterDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            ConditionMeter conditionMeter = null;

            // Act
            var conditionMeterDto = conditionMeter.ToDTO();

            // Assert
            Assert.IsNull(conditionMeterDto);
        }

        [TestMethod]
        public void TestInputToInputDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var input = new Input
            {
                Id = "test_input_id",
                Name = "Test Input",
                InputType = TheOracle2.Data.AssetInput.Text, // Using the source enum
                Value = "Test Value"
            };

            // Act
            var inputDto = input.ToDTO();

            // Assert
            Assert.IsNotNull(inputDto);
            Assert.AreEqual(input.Id, inputDto.Id);
            Assert.AreEqual(input.Name, inputDto.Name);
            Assert.AreEqual(AssetInputDTOType.Text, inputDto.InputType); // Asserting the mapped enum
            Assert.AreEqual(input.Value, inputDto.Value);
        }

        [TestMethod]
        public void TestInputToInputDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            Input input = null;

            // Act
            var inputDto = input.ToDTO();

            // Assert
            Assert.IsNull(inputDto);
        }

        [TestMethod]
        public void TestInputToInputDTOMapping_MapsAllEnumValuesCorrectly()
        {
            // Arrange
            var inputClock = new Input { InputType = TheOracle2.Data.AssetInput.Clock };
            var inputSelect = new Input { InputType = TheOracle2.Data.AssetInput.Select };
            var inputNumber = new Input { InputType = TheOracle2.Data.AssetInput.Number };
            var inputText = new Input { InputType = TheOracle2.Data.AssetInput.Text };

            // Act
            var dtoClock = inputClock.ToDTO();
            var dtoSelect = inputSelect.ToDTO();
            var dtoNumber = inputNumber.ToDTO();
            var dtoText = inputText.ToDTO();

            // Assert
            Assert.AreEqual(AssetInputDTOType.Clock, dtoClock.InputType);
            Assert.AreEqual(AssetInputDTOType.Select, dtoSelect.InputType);
            Assert.AreEqual(AssetInputDTOType.Number, dtoNumber.InputType);
            Assert.AreEqual(AssetInputDTOType.Text, dtoText.InputType);
        }

        [TestMethod]
        public void TestAssetToAssetDTOMapping_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var asset = new Asset
            {
                Id = "test_asset_id",
                Name = "Test Asset",
                AssetType = "Test Type",
                Source = new Source { Name = "Test Source Book" },
                Abilities = new List<Ability>
                {
                    new Ability { Id = "ab1", Name = "Ability 1", Text = "Text 1", Enabled = true },
                    new Ability { Id = "ab2", Name = "Ability 2", Text = "Text 2", Enabled = false }
                },
                ConditionMeter = new ConditionMeter { Id = "cm1", Name = "Health", Value = 10, Min = 0, Max = 10 },
                Inputs = new List<Input>
                {
                    new Input { Id = "in1", Name = "Input 1", InputType = TheOracle2.Data.AssetInput.Number, Value = 123 }
                }
            };

            // Act
            var assetDto = asset.ToDTO();

            // Assert
            Assert.IsNotNull(assetDto);
            Assert.AreEqual(asset.Id, assetDto.Id);
            Assert.AreEqual(asset.Name, assetDto.Name);
            Assert.AreEqual(asset.AssetType, assetDto.AssetType);

            Assert.IsNotNull(assetDto.Source);
            Assert.AreEqual(asset.Source.Name, assetDto.Source.Name);
            Assert.IsNull(assetDto.Source.Url); // As per extension method logic

            Assert.IsNotNull(assetDto.Abilities);
            Assert.AreEqual(2, assetDto.Abilities.Count);
            Assert.AreEqual(asset.Abilities[0].Id, assetDto.Abilities[0].Id);
            Assert.AreEqual(asset.Abilities[1].Enabled, assetDto.Abilities[1].Enabled);

            Assert.IsNotNull(assetDto.ConditionMeter);
            Assert.AreEqual(asset.ConditionMeter.Id, assetDto.ConditionMeter.Id);
            Assert.AreEqual(asset.ConditionMeter.Value, assetDto.ConditionMeter.Value);

            Assert.IsNotNull(assetDto.Inputs);
            Assert.AreEqual(1, assetDto.Inputs.Count);
            Assert.AreEqual(asset.Inputs[0].Id, assetDto.Inputs[0].Id);
            Assert.AreEqual(AssetInputDTOType.Number, assetDto.Inputs[0].InputType);
        }

        [TestMethod]
        public void TestAssetToAssetDTOMapping_NullSource_ShouldReturnNull()
        {
            // Arrange
            Asset asset = null;

            // Act
            var assetDto = asset.ToDTO();

            // Assert
            Assert.IsNull(assetDto);
        }

        [TestMethod]
        public void TestAssetToAssetDTOMapping_NullAndEmptyCollections_ShouldMapToEmptyDTOCollections()
        {
            // Arrange
            var assetWithNullCollections = new Asset
            {
                Id = "asset_null_collections",
                Name = "Asset with Null Collections",
                Abilities = null, // Null list
                Inputs = new List<Input>() // Empty list
            };

            // Act
            var assetDto = assetWithNullCollections.ToDTO();

            // Assert
            Assert.IsNotNull(assetDto);
            Assert.IsNotNull(assetDto.Abilities); // Should be an empty list, not null
            Assert.AreEqual(0, assetDto.Abilities.Count);
            Assert.IsNotNull(assetDto.Inputs); // Should be an empty list, not null
            Assert.AreEqual(0, assetDto.Inputs.Count);
            Assert.IsNull(assetDto.ConditionMeter); // ConditionMeter was not set, so should be null
            Assert.IsNull(assetDto.Source); // Source was not set, so should be null
        }

        [TestMethod]
        public void TestAssetToAssetDTOMapping_NullNestedObjects_ShouldMapToNullDTOs()
        {
            // Arrange
            var assetWithNullNested = new Asset
            {
                Id = "asset_null_nested",
                Name = "Asset with Null Nested Objects",
                ConditionMeter = null,
                Source = null
            };

            // Act
            var assetDto = assetWithNullNested.ToDTO();

            // Assert
            Assert.IsNotNull(assetDto);
            Assert.IsNull(assetDto.ConditionMeter);
            Assert.IsNull(assetDto.Source);
        }
    }
}
