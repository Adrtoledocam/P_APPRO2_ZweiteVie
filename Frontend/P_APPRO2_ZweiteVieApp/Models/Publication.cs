using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace P_APPRO2_ZweiteVieApp.Models
{
    public class Publication : INotifyPropertyChanged
    {
        [JsonProperty("pubId")]
        public int PubId { get; set; }

        [JsonProperty("pubTitle")]
        public string PubTitle { get; set; }

        [JsonProperty("pubDescription")]
        public string PubDescription { get; set; }

        [JsonProperty("conId")]
        public int ConId { get; set; }

        [JsonProperty("conName")]
        public string ConName { get; set; }

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

        private bool _isFavorited;
        public bool IsFavorited
        {
            get => _isFavorited;
            set { _isFavorited = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotFavorited)); }
        }
        public bool IsNotFavorited => !_isFavorited;
        public bool IsAvailable => PubStatus == "Disponible";

        public string RelativeDate
        {
            get
            {
                var diff = DateTime.Now - PubCreatedAt;
                if (diff.TotalDays >= 1) return $"{(int)diff.TotalDays} j";
                if (diff.TotalHours >= 1) return $"{(int)diff.TotalHours} h";
                return "maintenant";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
