using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace gamecore.actionsystem
{
    public abstract class GameAction
    {
        [JsonIgnore]
        public List<GameAction> PreReactions { get; private set; } = new();

        [JsonIgnore]
        public List<GameAction> PerformReactions { get; private set; } = new();

        [JsonIgnore]
        public List<GameAction> PostReactions { get; private set; } = new();
    }

    public class GameActionConverter : JsonConverter<GameAction>
    {
        public override GameAction ReadJson(
            JsonReader reader,
            Type objectType,
            GameAction existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            JObject obj = JObject.Load(reader);

            // Use the $type metadata to load the correct concrete type
            var typeName =
                (string)obj["$type"]
                ?? throw new JsonSerializationException(
                    "Missing $type for GameAction deserialization"
                );
            var type =
                Type.GetType(typeName)
                ?? throw new JsonSerializationException($"Cannot find type {typeName}");
            return (GameAction)obj.ToObject(type, serializer);
        }

        public override void WriteJson(
            JsonWriter writer,
            GameAction value,
            JsonSerializer serializer
        )
        {
            serializer.Serialize(writer, value);
        }
    }
}
