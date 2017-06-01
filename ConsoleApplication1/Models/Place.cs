using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class Place
    {
        public Int32 PlaceId { get; set; }
        public Int32 Id { get; set; }
        public String Title { get; set; }
        [JsonProperty("short_title")]
        public String ShortTitle { get; set; }
        public String Slug { get; set; }
        public String Address { get; set; }
        public String Location { get; set; }
        public String Timetable { get; set; }
        public String Phone { get; set; }
        [JsonProperty("is_stub")]
        public Boolean? IsStub { get; set; }
        public List<Images> Images { get; set; }
        public String Description { get; set; }
        [JsonProperty("body_text")]
        public String BodyText { get; set; }
        [JsonProperty("site_url")]
        public String SiteUrl { get; set; }
        [JsonProperty("foreign_url")]
        public String ForeignUrl { get; set; }
        public String CoordsStr { get { if (Coords != null) return Coords.Lon + ";" + Coords.Lat; else return null; } }
        public Coords Coords { get; set; }
        public String Subway { get; set; }
        [JsonProperty("favorites_count")]
        public Int32 FavoritesCount { get; set; }
        [JsonProperty("comments_count")]
        public Int32 CommentsCount { get; set; }
        [JsonProperty("is_closed")]
        public Boolean? IsClosed { get; set; }
        [JsonProperty("categories")]
        public String CategoriesKudaGo { get; set; }
        [JsonProperty("tags")]
        public List<String> TagsKudaGo { get; set; }
        //[JsonProperty("tags")]
        //public String Tags { get; set; }
        public Int32 CityId { get; set; }
        public Int32 Count { get; set; }
        [JsonProperty("has_parking_lot")]
        public Boolean? HasParkingLot { get; set; }
        [JsonProperty("type_vacation")]
        public Int32 TypeVacation { get; set; }
    }
}
