using System.Reflection;
using Castle.Core.Internal;

namespace TheOracle2.Data;

public class ReferenceStringConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        var singleStringProp = objectType.GetProperties().SingleOrDefault(p => p.GetType() == typeof(string));
        singleStringProp ??= objectType.GetProperties().SingleOrDefault(p => p.GetAttribute<ReferenceStringWrapperAttribute>() != null);

        return singleStringProp != null;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var singleStringProp = objectType.GetProperties().SingleOrDefault(p => p.PropertyType == typeof(string));
        singleStringProp ??= objectType.GetProperties().SingleOrDefault(p => p.GetAttribute<ReferenceStringWrapperAttribute>() != null);

        if (singleStringProp != null)
        {
            var obj = Activator.CreateInstance(objectType); 
            singleStringProp.SetValue(obj, reader.Value.ToString());
            return obj;
        }

        throw new JsonSerializationException($"The type {objectType.Name} could not be desearialized. If there's more than one string property please decorate one with the {nameof(ReferenceStringWrapperAttribute)}");
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class ReferenceStringWrapperAttribute : System.Attribute
{

}
