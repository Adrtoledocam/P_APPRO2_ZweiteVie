using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using P_APPRO2_ZweiteVieApp.Models;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{

    [QueryProperty(nameof(SelectedPublication), "SelectedPublication")]
    public class PublicationDetailViewModel : BaseViewModel
    {
        private Publication _selectedPublication;
        public Publication SelectedPublication
        {
            get => _selectedPublication;
            set { _selectedPublication = value; OnPropertyChanged(); }
        }

        public ICommand ContactEmailCommand { get; }
        public ICommand ContactPhoneCommand { get; }

        public PublicationDetailViewModel()
        {
            CheckAndRecoverData();
            ContactEmailCommand = new Command(async () => await ExecuteContactEmail());
            ContactPhoneCommand = new Command(async () => await ExecuteContactPhone());
        }
        private void CheckAndRecoverData()
        {
            if (SelectedPublication == null)
            {
                
                int savedId = Preferences.Get("last_selected_publication_id", -1);

                if (savedId != -1)
                {
                    
                    Console.WriteLine($"Cargando datos para la publicación ID: {savedId}");
                }
            }
        }

        private async Task ExecuteContactEmail()
        {
            if (SelectedPublication == null) return;

            var email = SelectedPublication.UseEmail;

            if (string.IsNullOrEmpty(email) || email.StartsWith("Connectez"))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Non disponible",
                    "Connectez-vous pour contacter le donneur.",
                    "OK");
                return;
            }

            await Launcher.Default.OpenAsync(
                $"mailto:{email}?subject=ZweiteVie: {SelectedPublication.PubTitle}");
        }

        private async Task ExecuteContactPhone()
        {
            if (SelectedPublication == null) return;

            var phone = SelectedPublication.UsePhone;

            if (string.IsNullOrEmpty(phone) || phone.StartsWith("Connectez"))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Non disponible",
                    "Connectez-vous pour voir le numéro de téléphone.",
                    "OK");
                return;
            }

            await Launcher.Default.OpenAsync($"tel:{phone}");
        }
    }
}
