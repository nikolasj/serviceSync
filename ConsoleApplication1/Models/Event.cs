using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class Event
    {
        public Int32 EventId { get; set; }
        public Int32 Id { get; set; }
        [JsonProperty("publication_date")]
        public Int64 PublicationDate { get; set; }
        [JsonProperty("dates")]
        public DateRange[] Dates { get; set; }
        public Int64 DateStart { get; set; }
        public Int64 DateEnd { get; set; }
        public String Title { get; set; }
        [JsonProperty("short_title")]
        public String ShortTitle { get; set; }
        public String Slug { get; set; }
        public Place Place { get; set; }
        public String Description { get; set; }
        [JsonProperty("body_text")]
        public String BodyText { get; set; }
        public Location Location { get; set; }
        public List<String> Categories { get; set; }
        public String Tagline { get; set; }
        [JsonProperty("age_restriction")]
        public String AgeRestriction { get; set; }
        public String Price { get; set; }
        [JsonProperty("is_free")]
        public Boolean? IsFree { get; set; }
        //[JsonProperty("images")]
        //public Images[] Images { get; set; }
        //public String Image { get; set; }
        public List<Images> Images;
        [JsonProperty("favorites_count")]
        public Int32 FavoritesCount { get; set; }
        [JsonProperty("comments_count")]
        public Int32 CommentsCount { get; set; }
        [JsonProperty("site_url")]
        public String SiteUrl { get; set; }
        public String Tags { get; set; }
        [JsonProperty("tags")]
        public List<String> TagsKudaGo { get; set; }
        public String Participants { get; set; }
        [JsonProperty("type_vacation")]
        public Int32 TypeVacation { get; set; }
    }
}
