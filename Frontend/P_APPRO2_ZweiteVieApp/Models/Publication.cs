using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P_APPRO2_ZweiteVieApp.Models
{
    public class Publication
    {
        public int PubId { get; set; }
        public string PubTitle { get; set; }
        public string PubDescription { get; set; }
        public string PubCondition { get; set; } 
        public string PubImage { get; set; }     
        public string PubStatus { get; set; }    
        public string PubLocation { get; set; }
        public DateTime PubCreatedAt { get; set; }
        public string CatName { get; set; }      
        public string DonorName { get; set; }    

        public string UseEmail { get; set; }
        public string UsePhone { get; set; }
    }
}
