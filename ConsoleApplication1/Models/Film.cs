using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.Models
{
    class Film
    {
        [JsonProperty("film_id")]
        public Int32 FilmId { get; set; }
        [JsonProperty("id")]
        public Int32 Id { get; set; }
        public Int32 KudaGoId { get; set; }
        [JsonProperty("site_url")]
        public String SiteUrl { get; set; }
        [JsonProperty("publication_date")]
        public Int64 PublicationDate { get; set; }
        public String Slug { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        [JsonProperty("body_text")]
        public String BodyText { get; set; }
        [JsonProperty("is_editors_choice")]
        public Boolean? IsEditorsChoice { get; set; }
        [JsonProperty("favorites_count")]
        public Int32? FavoritesCount { get; set; }
        public List<Genres> Genres { get; set; }
        [JsonProperty("comments_count")]
        public Int32? CommentsCount { get; set; }
        [JsonProperty("original_title")]
        public String OriginalTitle { get; set; }
        public String Locale { get; set; }
        public String Country { get; set; }
        public String Year { get; set; }
        public String Language { get; set; }
        [JsonProperty("running_time")]
        public String RunningTime { get; set; }
        [JsonProperty("budget_currency")]
        public String BudgetCurrency { get; set; }
        public String Budget { get; set; }
        [JsonProperty("mpaa_rating")]
        public String MpaaRating { get; set; }
        [JsonProperty("age_restriction")]
        public String AgeRestriction { get; set; }
        public String Stars { get; set; }
        public String Director { get; set; }
        public String Writer { get; set; }
        public String Awards { get; set; }
        public String Trailer { get; set; }
        public List<Images> Images { get; set; }
        public Poster Poster { get; set; }
        public String Url { get; set; }
        [JsonProperty("imdb_url")]
        public String ImdbUrl { get; set; }
        [JsonProperty("imdb_rating")]
        public Double? ImdbRating { get; set; }
        [JsonProperty("tags")]
        public List<String> TagsKudaGo { get; set; }
        [JsonProperty("type_vacation")]
        public Int32 TypeVacation { get; set; }
    }
}
