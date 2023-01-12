using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Disco.Entities
{
    internal class DebuggerPayload
    {
        [JsonPropertyName("id")]
        public int Id { get; set; } = 69; // nice

        [JsonPropertyName("method")]
        public string Method { get; set; } = "Runtime.evaluate";

        [JsonPropertyName("params")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Params { get; set; } = null;
    }

    internal class DebuggerParams
    {
        [JsonPropertyName("expression")]
        public string Expression { get; set; } = "";
    }
}
