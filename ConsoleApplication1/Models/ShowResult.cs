using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class ShowResult
    {
        [JsonProperty("count")]
        public Int32 Count { get; set; }
        [JsonProperty("next")]
        public String Next { get; set; }
        [JsonProperty("previous")]
        public String Previous { get; set; }
        [JsonProperty("results")]
        public List<Show> ResultShow { get; set; }
    }
}
