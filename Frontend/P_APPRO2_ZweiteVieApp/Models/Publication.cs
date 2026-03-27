using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_APPRO2_ZweiteVieApp.Models
{
    public class Publication
    {
        [JsonProperty("pubId")]
        public int PubId { get; set; }

        [JsonProperty("pubTitle")]
        public string PubTitle { get; set; }

        [JsonProperty("pubDescription")]
        public string PubDescription { get; set; }

        [JsonProperty("pubCondition")]
        public string PubCondition { get; set; }

        [JsonProperty("pubImage")]
        public string PubImage { get; set; }

        [JsonProperty("pubStatus")]
        public string PubStatus { get; set; }

        [JsonProperty("pubLocation")]
        public string PubLocation { get; set; }

        [JsonProperty("pubCreatedAt")]
        public DateTime PubCreatedAt { get; set; }

        [JsonProperty("catName")]
        public string CatName { get; set; }

        [JsonProperty("donorName")]
        public string DonorName { get; set; }

        [JsonProperty("useEmail")]
        public string UseEmail { get; set; }

        [JsonProperty("usePhone")]
        public string UsePhone { get; set; }
    }
}
