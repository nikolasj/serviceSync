using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace ConsoleApplication1
{
    class DataLayer
    {

        public static int GetTypeVacationByOpportunity(Event even)
        {
            var activity = new string[] { "парк", "скалодром", "спортивный", "велосипед", "марафон", "вечеринки", "потанцевать", "пешеходные" };
            foreach (var v in activity)
            {
                if (even.Description.ToLower().Contains(v.ToLower()) ||
                    even.Title.ToLower().Contains(v.ToLower()))
                {
                    return 1;
                }
            }
            return 2;
        }

        public static int GetTypeVacationByPlace(Place even)
        {
            var activity = new string[] { "парк", "скалодром", "спортивный", "велосипед", "марафон", "вечеринки", "потанцевать", "пешеходные" };
            foreach (var v in activity)
            {
                if (even.Description.ToLower().Contains(v.ToLower()) ||
                    even.Title.ToLower().Contains(v.ToLower()))
                {
                    return 1;
                }
            }
            return 2;
        }

        public static int GetTypeVacationByFilm(Film even)
        {
            //var activity = new string[] { "парк", "скалодром", "спортивный", "велосипед", "марафон", "вечеринки", "потанцевать", "пешеходные" };
            //foreach (var v in activity)
            //{
            //    if (even.Description.ToLower().Contains(v.ToLower()) ||
            //        even.Title.ToLower().Contains(v.ToLower()))
            //    {
            //        return 1;
            //    }
            //}
            return 2;
        }

        #region City
        const String ConnectionString = "Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;";

        private static List<Models.City> GetCityListFromDb()
        {
            List<Models.City> ourList = new List<Models.City>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT *
                          FROM [test].[dbo].[City]";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    City tempCity = new City();
                    tempCity.CityId = (int)reader["city_id"];
                    tempCity.Name = reader["name"].ToString();
                    ourList.Add(tempCity);
                }
            }

            return ourList;
        }

        private static List<Models.City> GetCityListWithKudago()
        {
            List<Models.City> kugaGoList = new List<Models.City>();
            // вызов сервиса 
            // https://kudago.com/public-api/v1.3/locations/?lang=&fields=&order_by=

            string URL = "https://kudago.com/public-api/v1.3/locations/";
            string urlParameters = "?fields=slug,name,timezone,coords,language";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<City>>().Result;
                foreach (var d in dataObjects)
                {
                    kugaGoList.Add(d);
                }
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return kugaGoList;
        }

        public static int SyncCityListWithKudago()
        {
            List<Models.City> ourList = GetCityListFromDb();
            List<Models.City> kugaGoList = GetCityListWithKudago();

            List<Models.City> listToAdd = new List<City>();
            List<Models.City> listToDelete = new List<City>();
            List<Models.City> listToUpdate = new List<City>();


            foreach (var k in kugaGoList)
            {
                var city = ourList.FirstOrDefault(x => x.Name == k.Name);
                if (city == null)
                    listToAdd.Add(k);
                else
                {
                    city.Timezone = k.Timezone;
                    k.CityId = city.CityId;
                    listToUpdate.Add(k);
                }

            }

            // обработка - логика слияния (мерджа) списков

            return SaveCityListToDb(listToAdd, listToDelete, listToUpdate);
        }

        private static int SaveCityListToDb(List<Models.City> listToAdd, List<Models.City> listToDelete, List<Models.City> listToUpdate)
        {
            // записать в базу
            AddCity(listToAdd);
            UpdateCity(listToUpdate);
            //DeleteCity(listToDelete);
            return 0;
        }

        private static int AddCity(List<Models.City> listToAdd)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var city in listToAdd)
                    {
                        SqlCommand command = new SqlCommand(@"INSERT INTO [dbo].[City]
                           ([slug]
                           ,[name]
                           ,[timezone]
                           ,[coords]
                           ,[language])
                     VALUES
                           (" + DataLayer.ToSqlMSSQL(city.Slug) + @"
                           ," + DataLayer.ToSqlMSSQL(city.Name) + @" 
                           ," + DataLayer.ToSqlMSSQL(city.Timezone) + @"
                           ," + DataLayer.ToSqlMSSQL(city.CoordsStr) + @"
                           ," + DataLayer.ToSqlMSSQL(city.Language) + @")", con);


                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int UpdateCity(List<Models.City> listToUpdate)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var city in listToUpdate)
                    {
                        SqlCommand command = new SqlCommand(@"UPDATE [dbo].[City]
                           SET [slug] = " + DataLayer.ToSqlMSSQL(city.Slug) + @"
                           ,[name] = " + DataLayer.ToSqlMSSQL(city.Name) + @"
                           ,[timezone] = " + DataLayer.ToSqlMSSQL(city.Timezone) + @"
                           ,[coords] = " + DataLayer.ToSqlMSSQL(city.CoordsStr) + @"
                           ,[language] = " + DataLayer.ToSqlMSSQL(city.Language) + @"
                            WHERE name = " + DataLayer.ToSqlMSSQL(city.Name), con);


                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int DeleteCity(List<Models.City> listToDelete)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var city in listToDelete)
                    {
                        SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[City]
                            WHERE name = " + DataLayer.ToSqlMSSQL(city.Name), con);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public static string ToSqlMSSQL(string literal)
        {
            //if (literal is null)
            //return "NULL";
            if (string.IsNullOrEmpty(literal))
            {
                return "''";
            }
            return "'" + literal.Replace("'", "''") + "'";
        }

        public static string ToSqlMSSQL(double? value)
        {
            return (value == null) ? "null" : value.ToString().Replace(",", ".");
        }

        public static string ToSqlMSSQL(Boolean? value)
        {
            return (value == null) ? "null" : (value == true) ? "1" : "0";
        }

        public static string ToSqlMSSQL(DateTime? value)
        {
            return (value == null) ? "null" : "'" + value.ToString() + "'";
        }

        public static string ToSqlMSSQL(Poster value)
        {
            return (value == null) ? "null" : value.ToString();
        }

        public static string ToSqlMSSQL(List<Genres> value)
        {
            return (value == null) ? "null" : value.ToString();
        }

        #endregion

        #region Category

        private static List<Category> GetCategoryListFromDb()
        {
            List<Category> ourList = new List<Category>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT *
                          FROM [test].[dbo].[Category]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Category tempCategory = new Category();
                    tempCategory.CategoryId = (int)reader["category_id"];
                    tempCategory.Name = reader["name"].ToString();
                    ourList.Add(tempCategory);
                }
            }

            return ourList;
        }

        private static List<Category> GetCategoryListWithKudago()
        {
            List<Category> kugaGoList = new List<Category>();
            // вызов сервиса 
            // https://kudago.com/public-api/v1.3/event-categories/?lang=&order_by=&fields=

            string URL = "https://kudago.com/public-api/v1.3/event-categories/";
            string urlParameters = "?lang=&order_by=&fields=";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<IEnumerable<Category>>().Result;
                foreach (var d in dataObjects)
                {
                    kugaGoList.Add(d);
                }
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return kugaGoList;
        }

        public static int SyncCategoryListWithKudago()
        {
            List<Category> ourList = GetCategoryListFromDb();
            List<Category> kugaGoList = GetCategoryListWithKudago();

            List<Category> listToAdd = new List<Category>();
            List<Category> listToDelete = new List<Category>();
            List<Category> listToUpdate = new List<Category>();


            foreach (var k in kugaGoList)
            {
                var category = ourList.FirstOrDefault(x => x.Name == k.Name);
                if (category == null)
                    listToAdd.Add(k);
                else
                {
                    category.Slug = k.Slug;
                    k.CategoryId = category.CategoryId;
                    listToUpdate.Add(k);
                }

            }

            // обработка - логика слияния (мерджа) списков

            return SaveCategoryListToDb(listToAdd, listToDelete, listToUpdate);
        }

        private static int SaveCategoryListToDb(List<Category> listToAdd, List<Category> listToDelete, List<Category> listToUpdate)
        {
            // записать в базу
            AddCategory(listToAdd);
            UpdateCategory(listToUpdate);
            //DeleteCity(listToDelete);
            return 0;
        }

        private static int AddCategory(List<Category> listToAdd)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var cat in listToAdd)
                    {
                        SqlCommand command = new SqlCommand(@"INSERT INTO [dbo].[Category]
                           ([slug]
                           ,[name])
                           VALUES
                           (" + DataLayer.ToSqlMSSQL(cat.Slug) + @"
                           ," + DataLayer.ToSqlMSSQL(cat.Name) + @")", con);

                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int UpdateCategory(List<Category> listToUpdate)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var cat in listToUpdate)
                    {
                        SqlCommand command = new SqlCommand(@"UPDATE [dbo].[Category]
                           SET [slug] = " + DataLayer.ToSqlMSSQL(cat.Slug) + @"
                           ,[name] = " + DataLayer.ToSqlMSSQL(cat.Name) + @"
                            WHERE name = " + DataLayer.ToSqlMSSQL(cat.Name), con);


                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int DeleteCategory(List<Category> listToDelete)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var cat in listToDelete)
                    {
                        SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[Category]
                            WHERE name = " + DataLayer.ToSqlMSSQL(cat.Name), con);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion

        #region Place

        private static List<Place> GetPlaceListFromDb()
        {
            List<Place> ourList = new List<Place>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT *
                          FROM [test].[dbo].[Place]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Place tempPlace = new Place();
                    tempPlace.PlaceId = (int)reader["place_id"];
                    tempPlace.Id = (int)reader["id"];
                    ourList.Add(tempPlace);
                }
            }

            return ourList;
        }

        private static List<Place> GetPlaceListWithKudago()
        {
            List<Place> kugaGoList = new List<Place>();
            // вызов сервиса 
            // https://kudago.com/public-api/v1.3/places/?lang=&fields=&expand=&order_by=&text_format=&ids=&location=&has_showings=&showing_since=1444385206&showing_until=1444385206&categories=airports,-anticafe&lon=&lat=&radius=

            string URL = "https://kudago.com/public-api/v1.3/places/";
            Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            string urlParameters = "?showing_since=" + unixTimestamp + "&fields=tags,id,title,short_title,slug,address,location,timetable,phone,is_stub,images,description,body_text,site_url,foreign_url,coords,subway,favorites_count,comments_count,is_closed";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<PlaceResult>().Result;
                foreach (var d in dataObjects.ResultPlace)
                {
                    kugaGoList.Add(d);
                }
                // dataObjects.Next != "https://kudago.com/public-api/v1.3/places/?page=3"
                int j = 0;
                while (dataObjects.Next != null)
                {
                    urlParameters = dataObjects.Next.Replace(URL, "");
                    response = client.GetAsync(urlParameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        dataObjects = response.Content.ReadAsAsync<PlaceResult>().Result;
                        foreach (var d in dataObjects.ResultPlace)
                        {
                            kugaGoList.Add(d);
                        }
                    }
                    else
                    {
                        break;
                    }
                    j++;
                }
            }

            return kugaGoList;
        }

        public static int SyncPlaceListWithKudago()
        {
            List<Place> ourList = GetPlaceListFromDb();
            List<Place> kugaGoList = GetPlaceListWithKudago();

            List<Place> listToAdd = new List<Place>();
            List<Place> listToDelete = new List<Place>();
            List<Place> listToUpdate = new List<Place>();


            foreach (var k in kugaGoList)
            {
                var place = ourList.FirstOrDefault(x => x.Id == k.Id);
                if (place == null)
                    listToAdd.Add(k);
                else
                {
                    place.Address = k.Address;
                    place.Slug = k.Slug;
                    listToUpdate.Add(k);
                }

            }

            // обработка - логика слияния (мерджа) списков

            return SavePlaceListToDb(listToAdd, listToDelete, listToUpdate);
        }

        private static int SavePlaceListToDb(List<Place> listToAdd, List<Place> listToDelete, List<Place> listToUpdate)
        {
            // записать в базу
            AddPlace(listToAdd);
            UpdatePlace(listToUpdate);
            //DeleteCity(listToDelete);
            return 0;
        }

        private static int AddPlace(List<Place> listToAdd)
        {
            int errorCount = 0;
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {

                con.Open();
                foreach (var cat in listToAdd)
                {
                    try
                    {
                        String sql = @"INSERT INTO [dbo].[Place]
                           (
	                        [id]
	                        ,[title]
	                        ,[short_title]
	                        ,[slug]
	                        ,[address]
	                        ,[location]
	                        ,[timetable]
	                        ,[phone]
	                        ,[is_stub]
	                        ,[images]
	                        ,[description]
	                        ,[body_text]
	                        ,[site_url]
	                        ,[foreign_url]
	                        ,[coords]
	                        ,[subway]
	                        ,[favorites_count]
	                        ,[comments_count]
	                        ,[is_closed]
	                        ,[categories]
                            ,[has_parking_lot]
                            ,[type_vacation]
	                        ,[tags]
	                        ,[city_id])
                           VALUES
                           (
                            " + DataLayer.ToSqlMSSQL(cat.Id) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Title) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.ShortTitle) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Slug) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Address) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Location) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Timetable) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Phone) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.IsStub) + @"
                            ," + ((cat.Images != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", cat.Images.Select(x => x.Image))) : "''") + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Description) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.BodyText) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.SiteUrl) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.ForeignUrl) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.CoordsStr) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.Subway) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.FavoritesCount) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.CommentsCount) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.IsClosed) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.CategoriesKudaGo) + @"
                            ," + DataLayer.ToSqlMSSQL(cat.HasParkingLot) + @"
                            ," + DataLayer.ToSqlMSSQL(GetTypeVacationByPlace(cat)) + @"
                            ," + ((cat.TagsKudaGo != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", cat.TagsKudaGo)) : "''") + @"
                            ," + DataLayer.ToSqlMSSQL(cat.CityId) + @")
SELECT @@IDENTITY as event_id";
                        SqlCommand command = new SqlCommand(sql, con);

                        Int32 placeId = Int32.Parse(command.ExecuteScalar().ToString());
                        // command.ExecuteNonQuery();

                        List<Tag> currentTags = AddTags(cat.TagsKudaGo);
                        AddTagPlace(currentTags, placeId, false);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                    }
                }
                con.Close();
            }
            return 0;
        }


        private static int UpdatePlace(List<Place> listToUpdate)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var cat in listToUpdate)
                    {
                        SqlCommand command = new SqlCommand(@"UPDATE [dbo].[Place]
                           SET 
	                        [id] = " + DataLayer.ToSqlMSSQL(cat.Id) + @",
	                        [title] = " + DataLayer.ToSqlMSSQL(cat.Title) + @",
	                        [short_title] = " + DataLayer.ToSqlMSSQL(cat.ShortTitle) + @",
	                        [slug] = " + DataLayer.ToSqlMSSQL(cat.Slug) + @",
	                        [address] = " + DataLayer.ToSqlMSSQL(cat.Address) + @",
	                        [location] = " + DataLayer.ToSqlMSSQL(cat.Location) + @",
	                        [timetable] = " + DataLayer.ToSqlMSSQL(cat.Timetable) + @",
	                        [phone] = " + DataLayer.ToSqlMSSQL(cat.Phone) + @",
	                        [is_stub] = " + DataLayer.ToSqlMSSQL(cat.IsStub) + @",
	                        [images] = " + ((cat.Images != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", cat.Images.Select(x => x.Image))) : "''") + @",
	                        [description] = " + DataLayer.ToSqlMSSQL(cat.Description) + @",
	                        [body_text] = " + DataLayer.ToSqlMSSQL(cat.BodyText) + @",
	                        [site_url] = " + DataLayer.ToSqlMSSQL(cat.SiteUrl) + @",
	                        [foreign_url] = " + DataLayer.ToSqlMSSQL(cat.ForeignUrl) + @",
	                        [coords] = " + DataLayer.ToSqlMSSQL(cat.CoordsStr) + @",
	                        [subway] = " + DataLayer.ToSqlMSSQL(cat.Subway) + @",
	                        [favorites_count] = " + DataLayer.ToSqlMSSQL(cat.FavoritesCount) + @",
	                        [comments_count] = " + DataLayer.ToSqlMSSQL(cat.CommentsCount) + @",
	                        [is_closed] = " + DataLayer.ToSqlMSSQL(cat.IsClosed) + @",
	                        [categories] = " + DataLayer.ToSqlMSSQL(cat.CategoriesKudaGo) + @",
                            [type_vacation] = " + DataLayer.ToSqlMSSQL(GetTypeVacationByPlace(cat)) + @",
	                        [tags] = " + ((cat.TagsKudaGo != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", cat.TagsKudaGo)) : "''") + @",
	                        [city_id] = " + DataLayer.ToSqlMSSQL(cat.CityId) + @"
                            WHERE id = " + DataLayer.ToSqlMSSQL(cat.Id), con);

                        List<Tag> currentTags = AddTags(cat.TagsKudaGo);
                        AddTagPlace(currentTags, cat.PlaceId, false);

                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int DeletePlace(List<Place> listToDelete)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var cat in listToDelete)
                    {
                        SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[Place]
                            WHERE id = " + DataLayer.ToSqlMSSQL(cat.Id), con);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion

        #region Event

        private static List<Event> GetEventListFromDb()
        {
            List<Event> ourList = new List<Event>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT *
                          FROM [test].[dbo].[Event]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Event tempEvent = new Event();
                    tempEvent.EventId = (int)reader["event_id"];
                    tempEvent.Id = (int)reader["id"];
                    ourList.Add(tempEvent);
                }
            }

            return ourList;
        }

        private static List<Event> GetEventListWithKudago()
        {
            List<Event> kugaGoList = new List<Event>();
            // вызов сервиса 
            // 

            string URL = "https://kudago.com/public-api/v1.3/events/";
            Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            string urlParameters = "?expand=place&actual_since=" + unixTimestamp + @"&lang=&fields=id,publication_date,title,short_title,description,body_text,dates,price,location,categories,tagline,age_restriction,is_free,favorites_count,comments_count,site_url,place,tags,images";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<EventResult>().Result;
                foreach (var d in dataObjects.ResultEvent)
                {
                    if (d.Dates != null)
                    {
                        if (d.Dates.Count() > 0)
                        {
                            d.DateStart = d.Dates[0].Start;
                            d.DateEnd = d.Dates[0].End;
                        }
                    }

                    kugaGoList.Add(d);
                }
                //dataObjects.Next != "https://kudago.com/public-api/v1.3/events/?actual_since=1478922044&fields=id%2Cpublication_date%2Ctitle%2Cshort_title%2Cdescription%2Cbody_text%2Cdates%2Cprice%2Cis_free%2Cfavorites_count%2Ccomments_count%2Csite_url%2Cplace%2tags&lang=&page=3"
                int j = 0;
                while (dataObjects.Next != null)
                {
                    urlParameters = dataObjects.Next.Replace(URL, "");
                    response = client.GetAsync(urlParameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        dataObjects = response.Content.ReadAsAsync<EventResult>().Result;
                        foreach (var d in dataObjects.ResultEvent)
                        {
                            if (d.Dates != null)
                            {
                                if (d.Dates.Count() > 0)
                                {
                                    d.DateStart = d.Dates[0].Start;
                                    d.DateEnd = d.Dates[0].End;
                                }
                            }

                            kugaGoList.Add(d);
                        }
                        //j++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return kugaGoList;
        }

        public static int SyncEventListWithKudago()
        {
            List<Event> ourList = GetEventListFromDb();
            List<Event> kugaGoList = GetEventListWithKudago();

            List<Event> listToAdd = new List<Event>();
            List<Event> listToDelete = new List<Event>();
            List<Event> listToUpdate = new List<Event>();


            foreach (var k in kugaGoList)
            {
                var ev = ourList.FirstOrDefault(x => x.Id == k.Id);
                if (ev == null)
                    listToAdd.Add(k);
                else
                {
                    ev.Images = k.Images;
                    ev.Slug = k.Slug;
                    k.EventId = ev.EventId;
                    listToUpdate.Add(k);
                }

            }

            // обработка - логика слияния (мерджа) списков

            return SaveEventListToDb(listToAdd, listToDelete, listToUpdate);
        }

        private static int SaveEventListToDb(List<Event> listToAdd, List<Event> listToDelete, List<Event> listToUpdate)
        {
            // записать в базу
            AddEvent(listToAdd);
            UpdateEvent(listToUpdate);
            //DeleteCity(listToDelete);
            return 0;
        }

        private static int AddEvent(List<Event> listToAdd)
        {
            int errorCount = 0;
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in listToAdd)
                {
                    try
                    {
                        String sql = @"INSERT INTO [dbo].[Event]
                           (
                           [id]
                           ,[publication_date]
                           ,[date_start]
                           ,[date_end]
                           ,[title]
                           ,[short_title]
                           ,[slug]
                           ,[place]
                           ,[description]
                           ,[body_text]
                           ,[location]
                           ,[categories]
                           ,[tagline]
                           ,[age_restriction]
                           ,[price]
                           ,[is_free]
                           ,[favorites_count]
                           ,[comments_count]
                           ,[site_url]
                           ,[type_vacation]
                           ,[tags]
                           ,[participants]
                           ,[images])
                           VALUES
                           (
                            " + DataLayer.ToSqlMSSQL(ev.Id) + @",
                            " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.PublicationDate).ToUniversalTime()) + @",
                            " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.DateStart).ToUniversalTime()) + @",
                            " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.DateEnd).ToUniversalTime()) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Title) + @",
                            " + DataLayer.ToSqlMSSQL(ev.ShortTitle) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Slug) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Place.Id) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Description) + @",
                            " + DataLayer.ToSqlMSSQL(ev.BodyText) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Location.Slug) + @",
                            " + DataLayer.ToSqlMSSQL(String.Join(", ", ev.Categories)) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Tagline) + @",
                            " + DataLayer.ToSqlMSSQL(ev.AgeRestriction) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Price) + @",
                            " + DataLayer.ToSqlMSSQL(ev.IsFree) + @",                    
                            " + DataLayer.ToSqlMSSQL(ev.FavoritesCount) + @",
                            " + DataLayer.ToSqlMSSQL(ev.CommentsCount) + @",
                            " + DataLayer.ToSqlMSSQL(ev.SiteUrl) + @",
                            " + DataLayer.ToSqlMSSQL(GetTypeVacationByOpportunity(ev)) + @",
                            " + ((ev.TagsKudaGo != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.TagsKudaGo)) : "''") + @",
                            " + ((ev.Participants != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Participants)) : "''") + @",
                            " + ((ev.Images != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Images.Select(x => x.Image))) : "''") + @");
                            SELECT @@IDENTITY as event_id";
                        SqlCommand command = new SqlCommand(sql, con);

                        Int32 eventId = Int32.Parse(command.ExecuteScalar().ToString());

                        if (!GetPlaceListFromDb().Exists(x => x.Id == ev.Place.Id))
                            AddPlace(new List<Place>() { ev.Place });

                        List<Tag> currentTags = AddTags(ev.TagsKudaGo);
                        AddTagEvent(currentTags, eventId, false);
                    }

                    catch (Exception ex)
                    {
                        errorCount++;
                    }
                }
                con.Close();
            }
            //if (errorCount > 0)
            //    return -1;

            return 0;
        }


        private static bool AddTagEvent(List<Tag> tags, int eventId, bool deleteFirst)
        {
            if (deleteFirst)
            {
                // delete from ... where eventId=
                try
                {
                    using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                    {
                        con.Open();
                        foreach (var tag in tags)
                        {
                            SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[TagEvent]
                            WHERE event_id = " + eventId, con);
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            //insert
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in tags)
                {
                    try
                    {
                        String sql = @"INSERT INTO [dbo].[TagEvent]
                        ([tag_id],
                         [event_id])
                         VALUES
                        (" + ev.TagId + @", 
                         " + eventId + ")";
                        SqlCommand command = new SqlCommand(sql, con);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return true;
        }

        private static bool AddTagFilm(List<Tag> tags, int filmId, bool deleteFirst)
        {
            if (deleteFirst)
            {
                // delete from ... where eventId=
                try
                {
                    using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                    {
                        con.Open();
                        foreach (var tag in tags)
                        {
                            SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[TagFilm]
                            WHERE film_id = " + filmId, con);
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            //insert
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in tags)
                {
                    try
                    {
                        String sql = @"INSERT INTO [dbo].[TagFilm]
                        ([tag_id],
                         [film_id])
                         VALUES
                        (" + ev.TagId + @", 
                         " + filmId + ")";
                        SqlCommand command = new SqlCommand(sql, con);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return true;
        }

        private static bool AddTagPlace(List<Tag> tags, int placeId, bool deleteFirst)
        {
            if (deleteFirst)
            {
                // delete from ... where eventId=
                try
                {
                    using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                    {
                        con.Open();
                        foreach (var tag in tags)
                        {
                            SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[TagPlace]
                            WHERE place_id = " + placeId, con);
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            //insert
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in tags)
                {
                    try
                    {
                        String sql = @"INSERT INTO [dbo].[TagPlace]
                        ([tag_id],
                         [place_id])
                         VALUES
                        (" + ev.TagId + @", 
                         " + placeId + ")";
                        SqlCommand command = new SqlCommand(sql, con);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return true;
        }

        private static bool AddGenreFilm(List<Genres> genres, int filmId, bool deleteFirst)
        {
            if (deleteFirst)
            {
                // delete from ... where eventId=
                try
                {
                    using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                    {
                        con.Open();
                        foreach (var tag in genres)
                        {
                            SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[GenresFilm]
                            WHERE film_id = " + filmId, con);
                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                }
            }

            //insert
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in genres)
                {
                    try
                    {
                        String sql = @"INSERT INTO [dbo].[GenresFilm]
                        ([genres_id],
                         [film_id])
                         VALUES
                        (" + ev.Id + @", 
                         " + filmId + ")";
                        SqlCommand command = new SqlCommand(sql, con);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return true;
        }

        private static List<Tag> AddTags(List<String> tags)
        {
            if (tags == null) return null;
            List<String> ourList = new List<String>();
            List<String> newList = new List<String>();
            List<Tag> listFromDb = new List<Tag>();

            //...заполнение из базы - SELECT ...
            String sql = @"SELECT text
                          FROM [test].[dbo].[Tag]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    String tempTags;
                    tempTags = reader["text"].ToString();
                    ourList.Add(tempTags);
                }
            }
            foreach (var tag in tags)
            {
                if (!ourList.Any(x => x == tag))
                {
                    newList.Add(tag);
                }
            }

            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in newList)
                {
                    try
                    {
                        String sql2 = @"INSERT INTO [dbo].[Tag]
                           (
                           [text]
                           )
                           VALUES
                           ( " + DataLayer.ToSqlMSSQL(ev) + @")";
                        SqlCommand command = new SqlCommand(sql2, con);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }

                foreach (var ev in tags)
                {
                    Tag t = new Tag() { Text = ev };
                    sql = @"SELECT TOP 1 tag_id FROM [dbo].[Tag] WHERE text = " + DataLayer.ToSqlMSSQL(t.Text);
                    SqlCommand command = new SqlCommand(sql, con);
                    t.TagId = Int32.Parse(command.ExecuteScalar().ToString());
                    listFromDb.Add(t);
                }

                con.Close();
                return listFromDb;

            }
        }

        //        private static List<Tag> AddPlaces(List<String> place)
        //        {
        //            if (place == null) return null;
        //            List<String> ourList = new List<String>();
        //            List<String> newList = new List<String>();
        //            List<Place> listFromDb = new List<Place>();

        //            //...заполнение из базы - SELECT ...
        //            String sql = @"SELECT Id
        //                          FROM [test].[dbo].[Place]";
        //            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
        //            {
        //                SqlCommand command = new SqlCommand(sql, con);

        //                con.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    String tempPlace;
        //                    tempPlace = reader["Id"].ToString();
        //                    ourList.Add(tempPlace);
        //                }
        //            }
        //            foreach (var tag in place)
        //            {
        //                if (!ourList.Any(x => x == tag))
        //                {
        //                    newList.Add(tag);
        //                }
        //            }

        //            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
        //            {
        //                con.Open();
        //                foreach (var ev in newList)
        //                {
        //                    try
        //                    {
        //                        String sqlPlace = @"INSERT INTO [dbo].[Place]
        //                           (
        //	                        [id]
        //	                        ,[title]
        //	                        ,[short_title]
        //	                        ,[slug]
        //	                        ,[address]
        //	                        ,[location]
        //	                        ,[timetable]
        //	                        ,[phone]
        //	                        ,[is_stub]
        //	                        ,[images]
        //	                        ,[description]
        //	                        ,[body_text]
        //	                        ,[site_url]
        //	                        ,[foreign_url]
        //	                        ,[coords]
        //	                        ,[subway]
        //	                        ,[favorites_count]
        //	                        ,[comments_count]
        //	                        ,[is_closed]
        //	                        ,[categories]
        //                            ,[has_parking_lot]
        //	                        ,[tags]
        //	                        ,[city_id])
        //                           VALUES
        //                           (
        //                            " + DataLayer.ToSqlMSSQL(ev.Id) + @"
        //                            ," + DataLayer.ToSqlMSSQL(ev.Title) + @"
        //                            ," + DataLayer.ToSqlMSSQL(ev.ShortTitle) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.Slug) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.Address) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.Location) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.Timetable) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.Phone) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.IsStub) + @"
        //                            ," + ((cat.Images != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", cat.Images.Select(x => x.Image))) : "''") + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.Description) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.BodyText) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.SiteUrl) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.ForeignUrl) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.CoordsStr) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.Subway) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.FavoritesCount) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.CommentsCount) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.IsClosed) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.CategoriesKudaGo) + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.HasParkingLot) + @"
        //                            ," + ((cat.TagsKudaGo != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", cat.TagsKudaGo)) : "''") + @"
        //                            ," + DataLayer.ToSqlMSSQL(cat.CityId) + @")
        //SELECT @@IDENTITY as event_id";
        //                        SqlCommand command = new SqlCommand(sql2, con);

        //                        command.ExecuteNonQuery();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        return null;
        //                    }
        //                }

        //                foreach (var ev in place)
        //                {
        //                    Tag t = new Tag() { Text = ev };
        //                    sql = @"SELECT TOP 1 tag_id FROM [dbo].[Tag] WHERE text = " + DataLayer.ToSqlMSSQL(t.Text);
        //                    SqlCommand command = new SqlCommand(sql, con);
        //                    t.TagId = Int32.Parse(command.ExecuteScalar().ToString());
        //                    listFromDb.Add(t);
        //                }

        //                con.Close();
        //                return listFromDb;

        //            }
        //        }

        private static List<Genres> AddGenres(List<Genres> genres)
        {
            if (genres == null) return null;
            List<Genres> ourList = new List<Genres>();
            List<Genres> newList = new List<Genres>();
            List<Genres> listFromDb = new List<Genres>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT name, slug
                          FROM [test].[dbo].[Genres]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Genres tempGenres = new Genres();
                    tempGenres.Name = reader["name"].ToString();
                    tempGenres.Slug = reader["slug"].ToString();
                    ourList.Add(tempGenres);
                }
            }
            foreach (var genre in genres)
            {
                if (!ourList.Any(x => x.Name == genre.Name && x.Slug == genre.Slug))
                {
                    newList.Add(genre);
                }
            }

            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in newList)
                {
                    try
                    {
                        String sql2 = @"INSERT INTO [dbo].[Genres]
                           ([id]
                           ,[name]
                           ,[slug]
                           )
                           VALUES
                           (" + DataLayer.ToSqlMSSQL(ev.Id) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Name) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Slug) + @")";
                        SqlCommand command = new SqlCommand(sql2, con);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                con.Close();
            }

            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var genre in genres)
                {
                    Genres t = new Genres() { Name = genre.Name, Slug = genre.Slug };
                    sql = @"SELECT TOP 1 genres_id FROM [dbo].[Genres] WHERE name = " + DataLayer.ToSqlMSSQL(t.Name) +
                        @"OR slug = " + DataLayer.ToSqlMSSQL(t.Slug);
                    SqlCommand command = new SqlCommand(sql, con);
                    t.Id = Int32.Parse(command.ExecuteScalar().ToString());
                    listFromDb.Add(t);
                }

                con.Close();
            }
            return listFromDb;

        }

        private static int UpdateEvent(List<Event> listToUpdate)
        {
            int errorCount = 0;
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";         
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                try
                {
                    con.Open();
                    foreach (var ev in listToUpdate)
                    {
                        SqlCommand command = new SqlCommand(@"UPDATE [dbo].[Event]
                           SET 
	                        [id] = " + DataLayer.ToSqlMSSQL(ev.Id) + @",
	                        [publication_date] = " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.PublicationDate).ToUniversalTime()) + @",
	                        [date_start] = " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.DateStart).ToUniversalTime()) + @",
                            [date_end] = " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.DateEnd).ToUniversalTime()) + @",
	                        [title] = " + DataLayer.ToSqlMSSQL(ev.Title) + @",
	                        [short_title] = " + DataLayer.ToSqlMSSQL(ev.ShortTitle) + @",
	                        [slug] = " + DataLayer.ToSqlMSSQL(ev.Slug) + @",
	                        [place] = " + DataLayer.ToSqlMSSQL(ev.Place.Id) + @",
	                        [description] = " + DataLayer.ToSqlMSSQL(ev.Description) + @",
	                        [body_text] = " + DataLayer.ToSqlMSSQL(ev.BodyText) + @",
	                        [location] = " + DataLayer.ToSqlMSSQL(ev.Location.Slug) + @",
	                        [categories] = " + DataLayer.ToSqlMSSQL(String.Join(", ", ev.Categories)) + @",
	                        [tagline] = " + DataLayer.ToSqlMSSQL(ev.Tagline) + @",
	                        [age_restriction] = " + DataLayer.ToSqlMSSQL(ev.AgeRestriction) + @",
	                        [price] = " + DataLayer.ToSqlMSSQL(ev.Price) + @",
	                        [is_free] = " + DataLayer.ToSqlMSSQL(ev.IsFree) + @",
	                        [images] = " + ((ev.Images != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Images.Select(x => x.Image))) : "''") + @",
	                        [favorites_count] = " + DataLayer.ToSqlMSSQL(ev.FavoritesCount) + @",
	                        [comments_count] = " + DataLayer.ToSqlMSSQL(ev.CommentsCount) + @",
	                        [site_url] = " + DataLayer.ToSqlMSSQL(ev.SiteUrl) + @",
                            [type_vacation] = " + DataLayer.ToSqlMSSQL(GetTypeVacationByOpportunity(ev)) + @",
	                        [tags] = " + ((ev.Tags != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Tags)) : "''") + @",
                            [participants] = " + ((ev.Tags != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Participants)) : "''") + @"
                            WHERE id = " + DataLayer.ToSqlMSSQL(ev.Id), con);

                        if (!GetPlaceListFromDb().Exists(x => x.Id == ev.Place.Id))
                            AddPlace(new List<Place>() { ev.Place });

                        List<Tag> currentTags = AddTags(ev.TagsKudaGo);
                        AddTagEvent(currentTags, ev.EventId, false);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                }
                con.Close();
            }
            if (errorCount > 0)
                return -1;

            return 0;
        }


        private static int DeleteEvent(List<Event> listToDelete)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var cat in listToDelete)
                    {
                        SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[Event]
                            WHERE id = " + DataLayer.ToSqlMSSQL(cat.Id), con);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion

        #region Film

        private static List<Film> GetFilmListFromDb()
        {
            List<Film> ourList = new List<Film>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT *
                          FROM [test].[dbo].[Film]";
            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Film tempFilm = new Film();
                    tempFilm.Id = (int)reader["id"];
                    tempFilm.FilmId = (int)reader["film_id"];
                    tempFilm.Slug = reader["slug"].ToString();
                    ourList.Add(tempFilm);
                }
            }

            return ourList;
        }

        private static List<Film> GetFilmListWithKudago()
        {
            List<Film> kugaGoList = new List<Film>();
            // вызов сервиса 
            // 

            string URL = "https://kudago.com/public-api/v1.3/movies/";
            Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            string urlParameters = "?lang=&actual_since=" + unixTimestamp + "&fields=id,publication_date,slug,title,genres,description,body_text,poster,language,running_time,budget_currency,mpaa_rating,age_restriction,stars,director,writer,awards,trailer,url,imdb_url,imdb_rating,trailer,country,images";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<FilmResult>().Result;
                foreach (var d in dataObjects.ResultFilm)
                {
                    kugaGoList.Add(d);
                }
                while (dataObjects.Next != null)
                {
                    urlParameters = dataObjects.Next.Replace(URL, "");
                    response = client.GetAsync(urlParameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        dataObjects = response.Content.ReadAsAsync<FilmResult>().Result;
                        foreach (var d in dataObjects.ResultFilm)
                        {
                            kugaGoList.Add(d);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return kugaGoList;
        }

        public static int SyncFilmListWithKudago()
        {
            List<Film> ourList = GetFilmListFromDb();
            List<Film> kugaGoList = GetFilmListWithKudago();

            List<Film> listToAdd = new List<Film>();
            List<Film> listToDelete = new List<Film>();
            List<Film> listToUpdate = new List<Film>();


            foreach (var k in kugaGoList)
            {
                var film = ourList.FirstOrDefault(x => x.Id == k.Id);
                if (film == null)
                    listToAdd.Add(k);
                else
                {
                    film.Slug = k.Slug;
                    k.FilmId = film.FilmId;
                    listToUpdate.Add(k);
                }

            }

            // обработка - логика слияния (мерджа) списков

            return SaveFilmListToDb(listToAdd, listToDelete, listToUpdate);
        }

        private static int SaveFilmListToDb(List<Film> listToAdd, List<Film> listToDelete, List<Film> listToUpdate)
        {
            // записать в базу
            AddFilm(listToAdd);
            UpdateFilm(listToUpdate);
            //DeleteCity(listToDelete);
            return 0;
        }

        private static int AddFilm(List<Film> listToAdd)
        {
            int errorCount = 0;
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";

            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in listToAdd)
                {
                    try
                    {
                        String sql = @"INSERT INTO [dbo].[Film]
                           ([id]
                           ,[site_url]
                           ,[publication_date]
                           ,[slug]
                           ,[title]
                           ,[description]
                           ,[body_text]
                           ,[is_editors_choice]
                           ,[favorites_count]
                           ,[genres]
                           ,[comments_count]
                           ,[original_title]
                           ,[locale]
                           ,[country]
                           ,[year]
                           ,[language]
                           ,[running_time]
                           ,[budget_currency]
                           ,[budget]
                           ,[mpaa_rating]
                           ,[age_restriction]
                           ,[stars]
                           ,[director]
                           ,[writer]
                           ,[awards]
                           ,[trailer]
                           ,[type_vacation]
                           ,[images]
                           ,[poster]
                           ,[url]
                           ,[imdb_url]
                           ,[imdb_rating])
                           VALUES
                           (" + DataLayer.ToSqlMSSQL(ev.Id) + @",
                            " + DataLayer.ToSqlMSSQL(ev.SiteUrl) + @",
                            " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.PublicationDate).ToUniversalTime()) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Slug) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Title) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Description) + @",
                            " + DataLayer.ToSqlMSSQL(ev.BodyText.Replace('\'', '"')) + @",
                            " + DataLayer.ToSqlMSSQL(ev.IsEditorsChoice) + @",
                            " + DataLayer.ToSqlMSSQL(ev.FavoritesCount) + @",
                            " + ((ev.Genres != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Genres.Select(x => x.Name))) : "''") + @",
                            " + DataLayer.ToSqlMSSQL(ev.CommentsCount) + @",
                            " + DataLayer.ToSqlMSSQL(ev.OriginalTitle) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Locale) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Country) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Year) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Language.ToLower()) + @",
                            " + DataLayer.ToSqlMSSQL(ev.RunningTime) + @",
                            " + DataLayer.ToSqlMSSQL(ev.BudgetCurrency) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Budget) + @",
                            " + DataLayer.ToSqlMSSQL(ev.MpaaRating) + @",
                            " + DataLayer.ToSqlMSSQL(ev.AgeRestriction) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Stars) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Director) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Writer) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Awards) + @",
                            " + DataLayer.ToSqlMSSQL(ev.Trailer) + @",
                            " + GetTypeVacationByFilm(ev) + @",
                            " + ((ev.Images != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Images.Select(x => x.Image))) : "''") + @",
                            " + ((ev.Poster != null) ? DataLayer.ToSqlMSSQL(ev.Poster.Image) : "''") + @",
                            " + DataLayer.ToSqlMSSQL(ev.Url) + @",
                            " + DataLayer.ToSqlMSSQL(ev.ImdbUrl) + @",
                            " + DataLayer.ToSqlMSSQL(ev.ImdbRating) + @")";
                        SqlCommand command = new SqlCommand(sql, con);
                        command.ExecuteNonQuery();

                        //Int32 filmId = Int32.Parse(command.ExecuteScalar().ToString());
                        string tags = "кино," + ev.AgeRestriction + "," + ev.Language + "," + ((ev.Genres != null) ? String.Join(", ", ev.Genres.Select(x => x.Name)) : "");
                        tags = tags.Replace(" ", "").Trim().ToLower();
                        while (tags.IndexOf(",,") >= 0)
                            tags = tags.Replace(",,", ",");

                        List<Genres> currentGenres = AddGenres(ev.Genres);
                        AddGenreFilm(currentGenres, ev.FilmId, false);

                        List<Tag> currentTags = AddTags(tags.Split(new char[] { ',' }).ToList());
                        AddTagFilm(currentTags, ev.Id, false);
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                    }
                }
                con.Close();
                //if (errorCount > 0)
                //    return -1;

                return 0;

            }
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
            return dtDateTime;
        }

        private static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Int64.Parse(unixTimeStamp));
            return dtDateTime;
        }

        private static int UpdateFilm(List<Film> listToUpdate)
        {
            int errorCount = 0;
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";

            using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
            {
                con.Open();
                foreach (var ev in listToUpdate)
                {
                    try
                    {
                        SqlCommand command = new SqlCommand(@"UPDATE [dbo].[Film]
                           SET 
                            [site_url] = " + DataLayer.ToSqlMSSQL(ev.SiteUrl) + @",
	                        [publication_date] = " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(ev.PublicationDate).ToUniversalTime()) + @",
	                        [slug] = " + DataLayer.ToSqlMSSQL(ev.Slug) + @",
	                        [title] = " + DataLayer.ToSqlMSSQL(ev.Title) + @",
	                        [description] = " + DataLayer.ToSqlMSSQL(ev.Description) + @",
	                        [body_text] = " + DataLayer.ToSqlMSSQL(ev.BodyText) + @",
	                        [is_editors_choice] = " + DataLayer.ToSqlMSSQL(ev.IsEditorsChoice) + @",
	                        [favorites_count] = " + DataLayer.ToSqlMSSQL(ev.FavoritesCount) + @",
	                        [genres] = " + ((ev.Genres != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Genres.Select(x => x.Name))) : "''") + @",
	                        [comments_count] = " + DataLayer.ToSqlMSSQL(ev.CommentsCount) + @",
	                        [original_title] = " + DataLayer.ToSqlMSSQL(ev.OriginalTitle) + @",
	                        [locale] = " + DataLayer.ToSqlMSSQL(ev.Locale) + @",
	                        [country] = " + DataLayer.ToSqlMSSQL(ev.Country) + @",
	                        [year] = " + DataLayer.ToSqlMSSQL(ev.Year) + @",
	                        [language] = " + DataLayer.ToSqlMSSQL(ev.Language.ToLower()) + @",
	                        [running_time] = " + DataLayer.ToSqlMSSQL(ev.RunningTime) + @",
	                        [budget_currency] = " + DataLayer.ToSqlMSSQL(ev.BudgetCurrency) + @",
	                        [budget] = " + DataLayer.ToSqlMSSQL(ev.Budget) + @",
	                        [mpaa_rating] = " + DataLayer.ToSqlMSSQL(ev.MpaaRating) + @",
	                        [age_restriction] = " + DataLayer.ToSqlMSSQL(ev.AgeRestriction) + @",
                            [stars] = " + DataLayer.ToSqlMSSQL(ev.Stars) + @",
                            [director] = " + DataLayer.ToSqlMSSQL(ev.Director) + @",
                            [writer] = " + DataLayer.ToSqlMSSQL(ev.Writer) + @",
                            [awards] = " + DataLayer.ToSqlMSSQL(ev.Awards) + @",
                            [trailer] = " + DataLayer.ToSqlMSSQL(ev.Trailer) + @",
                            [type_vacation] = " + DataLayer.ToSqlMSSQL(GetTypeVacationByFilm(ev)) + @",
                            [images] = " + ((ev.Images != null) ? DataLayer.ToSqlMSSQL(String.Join(", ", ev.Images.Select(x => x.Image))) : "''") + @",
                            [poster] = " + ((ev.Poster != null) ? DataLayer.ToSqlMSSQL(ev.Poster.Image) : "''") + @",
                            [url] = " + DataLayer.ToSqlMSSQL(ev.Url) + @",
                            [imdb_url] = " + DataLayer.ToSqlMSSQL(ev.ImdbUrl) + @",
                            [imdb_rating] = " + DataLayer.ToSqlMSSQL(ev.ImdbRating) + @"
                            WHERE slug = " + DataLayer.ToSqlMSSQL(ev.Slug) + "AND id = " + DataLayer.ToSqlMSSQL(ev.Id), con);

                        string tags = "кино," + ev.AgeRestriction + "," + ev.Language + "," + ((ev.Genres != null) ? String.Join(", ", ev.Genres.Select(x => x.Name)) : "");
                        tags = tags.Replace(" ", "").Trim().ToLower();
                        while (tags.IndexOf(",,") >= 0)
                            tags = tags.Replace(",,", ",");

                        List<Tag> currentTags = AddTags(tags.Split(new char[] { ',' }).ToList());
                        AddTagFilm(currentTags, ev.FilmId, false);

                        List<Genres> currentGenres = AddGenres(ev.Genres);
                        AddGenreFilm(currentGenres, ev.Id, false);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                    }
                }
                con.Close();
                if (errorCount > 0)
                    return -1;

                return 0;
            }
        }

        private static int DeleteFilm(List<Film> listToDelete)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var cat in listToDelete)
                    {
                        SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[Film]
                            WHERE slug = " + DataLayer.ToSqlMSSQL(cat.Slug), con);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion

        #region Tag

        private static List<Tag> GetTagListFromDb()
        {
            List<Tag> ourList = new List<Tag>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT *
                          FROM [test].[dbo].[Tag]";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Tag tempTag = new Tag();
                    tempTag.TagId = (int)reader["tag_id"];
                    tempTag.Text = reader["text"].ToString();
                    tempTag.UserId = (int)reader["user_id"];
                    ourList.Add(tempTag);
                }
            }

            return ourList;
        }

        #endregion

        #region Show


        private static List<Show> GetShowListFromDb()
        {
            List<Show> ourList = new List<Show>();
            //...заполнение из базы - SELECT ...
            String sql = @"SELECT *
                          FROM [test].[dbo].[Show]";
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Show tempShow = new Show();
                    tempShow.KudagoId = (int)reader["kudago_id"];
                    tempShow.ShowId = (int)reader["show_id"];
                    ourList.Add(tempShow);
                }
            }

            return ourList;
        }

        private static List<Show> GetShowListWithKudago()
        {
            List<Show> kugaGoList = new List<Show>();
            // вызов сервиса 
            // https://kudago.com/public-api/v1.3/locations/?lang=&fields=&order_by=

            string URL = "https://kudago.com/public-api/v1.3/movie-showings/";
            Int64 unixTimestamp = (Int64)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string urlParameters = "?actual_since=" + unixTimestamp + "&fields=id,movie,place,datetime,three_d,imax,four_dx,original_language,price";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);

            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var dataObjects = response.Content.ReadAsAsync<ShowResult>().Result;
                foreach (var d in dataObjects.ResultShow)
                {
                    kugaGoList.Add(d);
                }
                // dataObjects.Next != null
                //int j = 0;
                while (dataObjects.Next != null)
                {
                    urlParameters = dataObjects.Next.Replace(URL, "");
                    response = client.GetAsync(urlParameters).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        dataObjects = response.Content.ReadAsAsync<ShowResult>().Result;
                        foreach (var d in dataObjects.ResultShow)
                        {
                            kugaGoList.Add(d);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                //Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            }

            return kugaGoList;
        }

        public static int SyncShowListWithKudago()
        {
            List<Show> ourList = GetShowListFromDb();
            List<Show> kugaGoList = GetShowListWithKudago();

            List<Show> listToAdd = new List<Show>();
            List<Show> listToDelete = new List<Show>();
            List<Show> listToUpdate = new List<Show>();


            foreach (var k in kugaGoList)
            {
                var show = ourList.FirstOrDefault(x => x.ShowId == k.ShowId);
                if (show == null)
                    listToAdd.Add(k);
                else
                {
                    listToUpdate.Add(k);
                }

            }

            // обработка - логика слияния (мерджа) списков

            return SaveShowListToDb(listToAdd, listToDelete, listToUpdate);
        }

        private static int SaveShowListToDb(List<Show> listToAdd, List<Show> listToDelete, List<Show> listToUpdate)
        {
            // записать в базу
            AddShow(listToAdd);
            UpdateShow(listToUpdate);
            //DeleteCity(listToDelete);
            return 0;
        }

        private static int AddShow(List<Show> listToAdd)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var show in listToAdd)
                    {
                        SqlCommand command = new SqlCommand(@"INSERT INTO [dbo].[Show]
                           ([kudago_id]
                              ,[movie_id]
                              ,[place_id]
                              ,[datetime]
                              ,[three_d]
                              ,[imax]
                              ,[four_dx]
                              ,[original_language]
                              ,[price])
                     VALUES
                           (" + DataLayer.ToSqlMSSQL(show.KudagoId) + @"
                           ," + ((show.MovieId != null) ? DataLayer.ToSqlMSSQL(show.MovieId.Id) : "''") + @" 
                           ," + ((show.PlaceId != null) ? DataLayer.ToSqlMSSQL(show.PlaceId.Id) : "''") + @"
                           ," + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(show.DatetimeKudago).ToUniversalTime()) + @"
                            ," + DataLayer.ToSqlMSSQL(show.ThreeD) + @"
                            ," + DataLayer.ToSqlMSSQL(show.Imax) + @"
                            ," + DataLayer.ToSqlMSSQL(show.FourDx) + @"
                            ," + DataLayer.ToSqlMSSQL(show.OriginalLanguage) + @"
                           ," + DataLayer.ToSqlMSSQL(show.Price) + @")", con);


                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int UpdateShow(List<Show> listToUpdate)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var show in listToUpdate)
                    {
                        SqlCommand command = new SqlCommand(@"UPDATE [dbo].[Show]
                           SET [kudago_id] = " + DataLayer.ToSqlMSSQL(show.KudagoId) + @"
                           ,[movie_id] = " + ((show.MovieId != null) ? DataLayer.ToSqlMSSQL(show.MovieId.Id) : "''") + @"
                           ,[place_id] = " + ((show.PlaceId != null) ? DataLayer.ToSqlMSSQL(show.PlaceId.Id) : "''") + @"
                           ,[datetime] = " + DataLayer.ToSqlMSSQL(UnixTimeStampToDateTime(show.DatetimeKudago).ToUniversalTime()) + @"
                            ,[three_d] = " + DataLayer.ToSqlMSSQL(show.ThreeD) + @"
                            ,[imax] = " + DataLayer.ToSqlMSSQL(show.Imax) + @"
                            ,[four_dx] = " + DataLayer.ToSqlMSSQL(show.FourDx) + @"
                            ,[original_language] = " + DataLayer.ToSqlMSSQL(show.OriginalLanguage) + @"
                           ,[price] = " + DataLayer.ToSqlMSSQL(show.Price) + @"
                            WHERE kudago_id = " + DataLayer.ToSqlMSSQL(show.KudagoId), con);


                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        private static int DeleteShow(List<Show> listToDelete)
        {
            //            String sql = @"SELECT *
            //                          FROM [test].[dbo].[City]";
            try
            {
                using (SqlConnection con = new SqlConnection("Data Source=USERPC;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;Initial Catalog=test;"))
                {
                    con.Open();
                    foreach (var city in listToDelete)
                    {
                        SqlCommand command = new SqlCommand(@"DELETE FROM [dbo].[Show]
                            WHERE kudago_id = " + DataLayer.ToSqlMSSQL(city.KudagoId), con);
                        command.ExecuteNonQuery();
                    }
                    con.Close();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        #endregion
    }
}
