using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace map_generator.JsonLoading;

/**
 * Json Converter class used to replace JSON Dictionary&lt;K,V> loading with a less resource intensive ICollection&lt;(K,V)>.
 *
 * <summary>
 * <para>
 * Code based on that found at: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-7-0#sample-factory-pattern-converter
 * </para>
 * </summary>
 */
public class PairCollectionJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        // Ensure target is compatible with ICollection<T>
        if (!(
                typeToConvert.IsGenericType
                && typeToConvert.GetGenericTypeDefinition() == typeof(ICollection<>)))
        {
            return false;
        }

        var enumerableType = typeToConvert.GetGenericArguments()[0];

        // Ensure ICollection<T> follows T == ValueTuple<T,U>
        return enumerableType.IsGenericType && enumerableType.GetGenericTypeDefinition() == typeof(ValueTuple<,>);
    }

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        // Get the TKey and TValue types from ICollection<(,)>
        Type keyType = type.GetGenericArguments()[0].GetGenericArguments()[0];
        Type valueType = type.GetGenericArguments()[0].GetGenericArguments()[1];

        // Cursed reflective hack to create a JsonConverter of the specific type
        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(PairCollectionConverterInner<,>).MakeGenericType(keyType, valueType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { options },
            culture: null)!;

        return converter;
    }

    private class PairCollectionConverterInner<TKey, TValue> : JsonConverter<ICollection<(TKey, TValue)>>
    {
        private readonly JsonConverter<TKey> _keyConverter;
        private readonly JsonConverter<TValue> _valueConverter;

        private readonly Type _keyType;
        private readonly Type _valueType;

        public PairCollectionConverterInner(JsonSerializerOptions options)
        {
            // Piggyback off existing converters
            _keyConverter = (JsonConverter<TKey>)options.GetConverter(typeof(TKey));
            _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));

            // Cache the key and value types
            _keyType = typeof(TKey);
            _valueType = typeof(TValue);
        }

        public override ICollection<(TKey, TValue)> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            // Fail if we're not parsing a JSON object
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            // Backing list used to store the ICollection
            List<(TKey, TValue)> enumerable = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) return enumerable;

                // First property should be key
                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();
                TKey key = _keyConverter.Read(ref reader, _keyType, options)!;

                reader.Read();

                TValue value = _valueConverter.Read(ref reader, _valueType, options)!;

                enumerable.Add((key, value));
            }

            // No end found
            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            ICollection<(TKey, TValue)> collection,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            foreach ((TKey key, TValue value) in collection)
            {
                // Key must be string
                string propertyName = key?.ToString() ?? "null";
                writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);

                _valueConverter.Write(writer, value, options);
            }

            writer.WriteEndObject();
        }
    }
}