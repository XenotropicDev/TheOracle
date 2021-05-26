using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.BotCore
{
    public class ConcreteListTypeConverter<TInterface, TImplementation> : JsonConverter where TImplementation : TInterface
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var res = serializer.Deserialize<List<TImplementation>>(reader);
            return res.ConvertAll(x => (TInterface)x);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    public class ConcreteTypeConverter<TConcrete> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<TConcrete>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
