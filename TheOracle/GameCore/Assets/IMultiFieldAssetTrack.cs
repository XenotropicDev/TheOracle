using Discord;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TheOracle.BotCore;

namespace TheOracle.GameCore.Assets
{
    public interface IMultiFieldAssetTrack
    {
        [JsonConverter(typeof(ConcreteListTypeConverter<IAssetEmbedField, AssetEmbedField>))]
        public IList<IAssetEmbedField> Fields { get; set; }

        IMultiFieldAssetTrack DeepCopy();
    }
}