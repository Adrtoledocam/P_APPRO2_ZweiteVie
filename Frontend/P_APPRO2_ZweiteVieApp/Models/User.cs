using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_APPRO2_ZweiteVieApp.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int UseId { get; set; }

        [JsonProperty("username")]
        public string UseName { get; set; }

        [JsonProperty("email")]
        public string UseEmail { get; set; }

        [JsonProperty("phone")]
        public string? UsePhone { get; set; }

        [JsonProperty("totalPubs")]
        public int TotalPubs { get; set; }

        [JsonProperty("totalDonated")]
        public int TotalDonated { get; set; }

        [JsonProperty("totalCo2")]
        public decimal TotalCo2 { get; set; }
    }
}
