using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;

public class ShouldSerializeContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
        foreach (var prop in props)
        {
            prop.Ignored = true;   // Ignore [JsonIgnore]
            prop.Converter = null;  // Ignore [JsonConverter]
            prop.PropertyName = "Holi";//prop.UnderlyingName;  // restore original property name
        }
        return props;
    }
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyName == "age")
        {
            property.PropertyName = "AGEEE";
        }

        if (property.PropertyType == typeof(string))
        {
            // Do not include emptry strings
            property.ShouldSerialize = instance =>
            {
                return !string.IsNullOrWhiteSpace(instance.GetType().GetProperty(member.Name).GetValue(instance, null) as string);
            };
        }
        else if (property.PropertyType == typeof(DateTime))
        {
            // Do not include zero DateTime
            property.ShouldSerialize = instance =>
            {
                return Convert.ToDateTime(instance.GetType().GetProperty(member.Name).GetValue(instance, null)) != default(DateTime);
            };
        }
        else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
        {
            // Do not include zero-length lists
            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    property.ShouldSerialize = instance =>
                    {
                        var enumerable = instance.GetType().GetProperty(member.Name).GetValue(instance, null) as IEnumerable;
                        return enumerable != null ? enumerable.GetEnumerator().MoveNext() : false;
                    };
                    break;
                case MemberTypes.Field:
                    property.ShouldSerialize = instance =>
                    {
                        var enumerable = instance.GetType().GetField(member.Name).GetValue(instance) as IEnumerable;
                        return enumerable != null ? enumerable.GetEnumerator().MoveNext() : false;
                    };
                    break;
            }
        }
        return property;
    }
}