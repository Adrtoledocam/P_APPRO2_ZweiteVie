using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_APPRO2_ZweiteVieApp.Models
{
    public class Category
    {
        [JsonProperty("catId")]
        public int CatId { get; set; }

        [JsonProperty("catName")]
        public string CatName { get; set; }

        [JsonProperty("parentId")]
        public int? ParentId { get; set; }

        [JsonProperty("catCo2Impact")]
        public decimal CatCo2Impact { get; set; }
    }
}
