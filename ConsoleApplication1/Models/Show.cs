using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class Show
    {
        public Int32 ShowId { get; set; }
        [JsonProperty("id")]
        public Int32 KudagoId { get; set; }
        [JsonProperty("movie")]
        public Movie MovieId { get; set; }
        [JsonProperty("place")]
        public PlaceShow PlaceId { get; set; }
        public DateTime Datetime { get; set; }
        [JsonProperty("datetime")]
        public long DatetimeKudago { get; set; }
        [JsonProperty("three_d")]
        public Boolean ThreeD { get; set; }
        public Boolean Imax { get; set; }
        [JsonProperty("four_dx")]
        public Boolean FourDx { get; set; }
        [JsonProperty("original_language")]
        public Boolean OriginalLanguage { get; set; }
        public String Price { get; set; }
    }
}
