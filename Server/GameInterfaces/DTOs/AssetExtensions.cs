using TheOracle2.Data;
using Server.GameInterfaces.DTOs; // Added for DTO types
using System.Linq;
using System.Collections.Generic;

namespace Server.GameInterfaces.DTOs
{
    public static class AssetExtensions
    {
        public static AssetDTO ToDTO(this Asset asset)
        {
            if (asset == null)
            {
                return null;
            }

            return new AssetDTO
            {
                Id = asset.Id,
                Name = asset.Name,
                AssetType = asset.AssetType,
                Abilities = asset.Abilities?.Select(a => a.ToDTO()).ToList() ?? new List<AbilityDTO>(),
                ConditionMeter = asset.ConditionMeter?.ToDTO(),
                Inputs = asset.Inputs?.Select(i => i.ToDTO()).ToList() ?? new List<InputDTO>(),
                Source = asset.Source != null ? new SourceDTO { Name = asset.Source.Name, Url = null } : null 
                // Assuming Source has a Name property. Url is set to null as per instructions.
            };
        }

        public static AbilityDTO ToDTO(this Ability ability)
        {
            if (ability == null)
            {
                return null;
            }

            return new AbilityDTO
            {
                Id = ability.Id,
                Name = ability.Name,
                Text = ability.Text,
                Enabled = ability.Enabled
            };
        }

        public static ConditionMeterDTO ToDTO(this ConditionMeter conditionMeter)
        {
            if (conditionMeter == null)
            {
                return null;
            }

            return new ConditionMeterDTO
            {
                Id = conditionMeter.Id,
                Name = conditionMeter.Name,
                Value = conditionMeter.Value,
                Min = conditionMeter.Min,
                Max = conditionMeter.Max
            };
        }

        public static InputDTO ToDTO(this Input input)
        {
            if (input == null)
            {
                return null;
            }

            return new InputDTO
            {
                Id = input.Id,
                Name = input.Name,
                InputType = MapAssetInputType(input.InputType),
                Value = input.Value
            };
        }

        private static AssetInputDTOType MapAssetInputType(TheOracle2.Data.AssetInput inputType)
        {
            switch (inputType)
            {
                case TheOracle2.Data.AssetInput.Clock:
                    return AssetInputDTOType.Clock;
                case TheOracle2.Data.AssetInput.Select:
                    return AssetInputDTOType.Select;
                case TheOracle2.Data.AssetInput.Number:
                    return AssetInputDTOType.Number;
                case TheOracle2.Data.AssetInput.Text:
                    return AssetInputDTOType.Text;
                default:
                    // Or throw an exception, or return a default value
                    // For now, let's assume a default or decide based on requirements.
                    // Throwing an ArgumentOutOfRangeException is safer for unhandled enum values.
                    throw new System.ArgumentOutOfRangeException(nameof(inputType), $"Unsupported asset input type: {inputType}");
            }
        }
    }
}
