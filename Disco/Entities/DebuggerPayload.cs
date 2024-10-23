using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Disco.Entities
{
    public class DebuggerPayload
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } = Random.Shared.Next(0, int.MaxValue); // nice

        [JsonPropertyName("method")]
        public string Method { get; set; } = "Runtime.evaluate";

        [JsonPropertyName("params")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Params { get; set; } = null;
    }

    public class DebuggerParams
    {
        [JsonPropertyName("expression")]
        public string Expression { get; set; } = "";
    }
}
