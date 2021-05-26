using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.BotCore;

namespace TheOracle.GameCore.Assets
{
    public class MultiFieldAssetTrack : IMultiFieldAssetTrack
    {
        public MultiFieldAssetTrack()
        {
            Fields = new List<IAssetEmbedField>();
        }

        [JsonConverter(typeof(ConcreteListTypeConverter<IAssetEmbedField, AssetEmbedField>))]
        public IList<IAssetEmbedField> Fields { get; set; }

        public IMultiFieldAssetTrack DeepCopy()
        {
            var track = (MultiFieldAssetTrack)this.MemberwiseClone();
            track.Fields = this.Fields.Select(item => item).ToList();

            return track;
        }
    }
}