using System;
using System.Collections.Generic;
using System.Windows.Input;
using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{
    public class PublicationDetailViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private Publication _selectedPublication;
        public Publication SelectedPublication
        {
            get => _selectedPublication;
            set { _selectedPublication = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public ICommand ContactEmailCommand { get; }
        public ICommand ContactPhoneCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }

        public PublicationDetailViewModel()
        {
            _apiService = new ApiService();
            ContactEmailCommand = new Command(async () => await ExecuteContactEmail());
            ContactPhoneCommand = new Command(async () => await ExecuteContactPhone());
            ToggleFavoriteCommand = new Command(async () => await ExecuteToggleFavorite());
        }

        public async Task LoadFromIdAsync(int pubId, bool isFav = false)
        {
            IsLoading = true;
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                var pub = await _apiService.GetPublicationByIdAsync(pubId, token);
                if (pub != null)
                {
                    pub.IsFavorited = isFav;
                    SelectedPublication = pub;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PublicationDetail Error]: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ExecuteContactEmail()
        {
            if (SelectedPublication == null) return;
            var email = SelectedPublication.UseEmail;
            if (string.IsNullOrEmpty(email) || email.StartsWith("Connectez"))
            {
                await Application.Current.MainPage.DisplayAlert("Non disponible", "Connectez-vous pour contacter le donneur.", "OK");
                return;
            }
            await Launcher.Default.OpenAsync($"mailto:{email}?subject=ZweiteVie: {SelectedPublication.PubTitle}");
        }

        private async Task ExecuteContactPhone()
        {
            if (SelectedPublication == null) return;
            var phone = SelectedPublication.UsePhone;
            if (string.IsNullOrEmpty(phone) || phone.StartsWith("Connectez"))
            {
                await Application.Current.MainPage.DisplayAlert("Non disponible", "Connectez-vous pour voir le numéro.", "OK");
                return;
            }
            await Launcher.Default.OpenAsync($"tel:{phone}");
        }

        private async Task ExecuteToggleFavorite()
        {
            if (SelectedPublication == null) return;
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
            {
                await Application.Current.MainPage.DisplayAlert("Non connecté", "Connectez-vous pour gérer vos favoris.", "OK");
                return;
            }
            if (SelectedPublication.IsFavorited)
            {
                bool success = await _apiService.RemoveFromFavoritesAsync(SelectedPublication.PubId, token);
                if (success) SelectedPublication.IsFavorited = false;
            }
            else
            {
                bool success = await _apiService.AddToFavoritesAsync(SelectedPublication.PubId, token);
                if (success) SelectedPublication.IsFavorited = true;
            }
        }
    }
}
