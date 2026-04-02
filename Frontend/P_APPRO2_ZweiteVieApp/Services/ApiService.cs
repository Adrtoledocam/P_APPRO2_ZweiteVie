using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace P_APPRO2_ZweiteVieApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string _baseUrl = "http://10.0.2.2:8080/api/";
        //private const string _baseUrl = "http://10.195.74.6/api/" ; 
        //private const string _baseUrl = " http://127.0.0.1:4040/api/";
        //private const string _baseUrl = " https://climant-esthetically-laraine.ngrok-free.dev/api/";
        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        //Authentification
        public class LoginResponse
        {
            public string Token { get; set; }
            public User User { get; set; }
        }
        public async Task<LoginResponse> LoginAsync(string email, string password)
        {
            var loginData = new {email, password };
            //var json = JsonConvert.SerializeObject(loginData);
            var content = Serialize(loginData); 
            //var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[DEBUG] JSON Recibido: {result}");

                    var json = JObject.Parse(result);

                    return new LoginResponse
                    {
                        Token = json["token"]?.ToString(),
                        User = new User
                        {
                            // Mapeamos según el JSON de tu Postman
                            UseId = json["user"]?["id"]?.Value<int>() ?? 0,
                            UseName = json["user"]?["username"]?.ToString(),
                            UseEmail = json["user"]?["email"]?.ToString()
                        }
                    };

                    var token = json["token"]?.ToString();
                    //return token;
                    //var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                    //return data["token"];
                }
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[LoginAsync] Échec {response.StatusCode}: {errorBody}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error Login]: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var data = new { username, email, password };
            var content = Serialize(data);

            var response = await _httpClient.PostAsync($"{_baseUrl}auth/register", content);
            return response.IsSuccessStatusCode;
        }

        //Profil
        public async Task<User> GetMyProfileAsync(string token)
        {
            SetAuthHeader(token);

            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}user/stats");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserStatsResponse>(json);

                return new User
                {
                    UseName = result.Profile?.UseName,
                    UseEmail = result.Profile?.UseEmail,
                    UsePhone = result.Profile?.UsePhone,
                    TotalPubs = result.Stats?.TotalPubs ?? 0,
                    TotalDonated = result.Stats?.TotalDonated ?? 0,
                    TotalCo2 = result.Stats?.TotalCo2 ?? 0
                };

                /*
                var wrapper = JsonConvert.DeserializeObject<UserStatsResponse>(json);
                if (wrapper == null) return null;

                return new User
                {
                    UseName = wrapper.Profile?.UseName,
                    UseEmail = wrapper.Profile?.UseEmail,
                    UsePhone = wrapper.Profile?.UsePhone,
                    TotalPubs = wrapper.Stats?.TotalPubs ?? 0,
                    TotalDonated = wrapper.Stats?.TotalDonated ?? 0,
                    TotalCo2 = wrapper.Stats?.TotalCo2 ?? 0
                };*/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error GetMyProfile]: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateProfileAsync(string token, string name, string phone)
        {
            SetAuthHeader(token);

            var data = new { username = name, phone };
            var content = Serialize(data);

            try
            {
                var response = await _httpClient.PutAsync($"{_baseUrl}user/profile", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error UpdateProfile]: {ex.Message}");
                return false;
            }
        }

        //PUBLICATIOns
        public async Task<List<Publication>> GetPublicationsAsync(string searchQuery = "")
        {
            try
            {
                var url = string.IsNullOrEmpty(searchQuery)
                    ? $"{_baseUrl}publications"
                    : $"{_baseUrl}publications?search={searchQuery}";

                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Publication>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error GetPublications]: {ex.Message}");
            }
            return new List<Publication>();
        }

        public async Task<bool> CreatePublicationAsync(string token, string title, string description, int conId, string location, int catId, string imageBase64)
        {

            try
            {
                SetAuthHeader(token);
                var data = new { title, description, conId, location, catId, imageBase64 };
                var content = Serialize(data);

                var response = await _httpClient.PostAsync(_baseUrl + "publications", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error CreatePub]: {ex.Message}");
                return false;
            }
        }

        //Favoris
        public async Task<bool> AddToFavoritesAsync(int pubId, string token)
        {
            SetAuthHeader(token);
            var content = Serialize(new { pubId });
            var response = await _httpClient.PostAsync($"{_baseUrl}favorites", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Publication>> GetMyFavoritesAsync(string token)
        {
            SetAuthHeader(token);

            var response = await _httpClient.GetAsync($"{_baseUrl}favorites");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Publication>>(json);
            }
            return new List<Publication>();
        }

        //Categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}categories");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Category>>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error GetCategories]: {ex.Message}");
            }
            return new List<Category>();
        }

        private void SetAuthHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private static StringContent Serialize(object obj) =>
            new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }

    internal class UserStatsResponse
    {
        [JsonProperty("profile")]
        public UserProfile Profile { get; set; }

        [JsonProperty("stats")]
        public UserStats Stats { get; set; }
    }

    internal class UserProfile
    {
        [JsonProperty("useName")]
        public string UseName { get; set; }

        [JsonProperty("useEmail")]
        public string UseEmail { get; set; }

        [JsonProperty("usePhone")]
        public string UsePhone { get; set; }
    }

    internal class UserStats
    {
        [JsonProperty("totalPubs")]
        public int TotalPubs { get; set; }

        [JsonProperty("totalDonated")]
        public int TotalDonated { get; set; }

        [JsonProperty("totalCo2")]
        public decimal TotalCo2 { get; set; }
    }
}
