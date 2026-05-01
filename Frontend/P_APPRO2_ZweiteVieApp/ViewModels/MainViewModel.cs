using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        public ObservableCollection<Publication> Publications { get; } = new ObservableCollection<Publication>();

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                var previous = _searchQuery;
                _searchQuery = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(previous) && string.IsNullOrEmpty(value))
                    Task.Run(async () => await LoadPublicationsAsync());
            }
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand SelectPublicationCommand { get; }
        public ICommand AddToFavoritesCommand { get; }

        public MainViewModel()
        {
            _apiService = new ApiService();
            RefreshCommand = new Command(async () => await LoadPublicationsAsync(showSpinner: true));
            SearchCommand = new Command(async () => await LoadPublicationsAsync(search: SearchQuery));

            SelectPublicationCommand = new Command<Publication>(async (pub) =>
            {
                var param = new Dictionary<string, object>
                {
                    { "PubId", pub.PubId },
                    { "IsFav", pub.IsFavorited }
                };
                await Shell.Current.GoToAsync("PublicationDetailPage", param);
            });

            AddToFavoritesCommand = new Command<Publication>(async (pub) =>
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Non connecté", "Connectez-vous pour gérer vos favoris.", "OK");
                    return;
                }
                if (pub.IsFavorited)
                {
                    bool success = await _apiService.RemoveFromFavoritesAsync(pub.PubId, token);
                    if (success) pub.IsFavorited = false;
                }
                else
                {
                    bool success = await _apiService.AddToFavoritesAsync(pub.PubId, token);
                    if (success) pub.IsFavorited = true;
                }
            });

            Task.Run(async () => await LoadPublicationsAsync());
        }

        public async Task LoadPublicationsAsync(string search = "", bool showSpinner = false)
        {
            if (IsBusy)
            {
                IsRefreshing = false;
                return;
            }

            IsBusy = true;
            if (showSpinner) IsRefreshing = true;

            try
            {
                var items = await _apiService.GetPublicationsAsync(search);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Publications.Clear();
                    foreach (var item in items)
                        Publications.Add(item);
                    IsRefreshing = false;
                });

                if (Preferences.Get("user_id", 0) > 0)
                    _ = MarkFavoritesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainViewModel Error]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
        private async Task MarkFavoritesAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;
                var favs = await _apiService.GetMyFavoritesAsync(token);
                var favIds = new HashSet<int>(favs.Select(f => f.PubId));
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var pub in Publications)
                        pub.IsFavorited = favIds.Contains(pub.PubId);
                });
            }
            catch { }
        }
    }
}
