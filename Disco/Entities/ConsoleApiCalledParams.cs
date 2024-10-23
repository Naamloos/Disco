using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Disco.Entities
{
    public class ConsoleApiCalledParams
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";
        [JsonPropertyName("args")]
        public ConsoleArg[] Args { get; set; } = Array.Empty<ConsoleArg>();
    }

    public class ConsoleArg
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";
        [JsonPropertyName("value")]
        public string Value { get; set; } = "";
    }
}
