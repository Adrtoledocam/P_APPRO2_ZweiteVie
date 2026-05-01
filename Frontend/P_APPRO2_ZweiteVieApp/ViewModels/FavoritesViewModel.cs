using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{
    public class FavoritesViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        public ObservableCollection<Publication> Favorites { get; } = new();

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNotLoggedIn));
            }
        }
        public bool IsNotLoggedIn => !_isLoggedIn;

        public ICommand RefreshCommand { get; }
        public ICommand SelectPublicationCommand { get; }
        public ICommand RemoveFromFavoritesCommand { get; }

        public FavoritesViewModel()
        {
            _apiService = new ApiService();
            RefreshCommand = new Command(async () => await LoadFavoritesAsync(showSpinner: true));
            SelectPublicationCommand = new Command<Publication>(async (pub) =>
            {
                var param = new Dictionary<string, object>
                {
                    { "PubId", pub.PubId },
                    { "IsFav", true }
                };
                await Shell.Current.GoToAsync("PublicationDetailPage", param);
            });
            RemoveFromFavoritesCommand = new Command<Publication>(async (pub) =>
            {
                if (pub == null) return;
                try
                {
                    var token = await SecureStorage.GetAsync("auth_token");
                    if (string.IsNullOrEmpty(token)) return;
                    bool success = await _apiService.RemoveFromFavoritesAsync(pub.PubId, token);
                    if (success)
                        MainThread.BeginInvokeOnMainThread(() => Favorites.Remove(pub));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[RemoveFavorite Error]: {ex.Message}");
                }
            });

            IsLoggedIn = Preferences.Get("user_id", 0) > 0;
            if (IsLoggedIn)
                Task.Run(async () => await LoadFavoritesAsync());
        }

        public async Task LoadFavoritesAsync(bool showSpinner = false)
        {
            if (IsBusy) { IsRefreshing = false; return; }
            IsBusy = true;
            if (showSpinner) IsRefreshing = true;

            try
            {
                // Synchronous quick check first
                if (Preferences.Get("user_id", 0) == 0)
                {
                    MainThread.BeginInvokeOnMainThread(() => { IsLoggedIn = false; Favorites.Clear(); });
                    return;
                }

                var token = await SecureStorage.GetAsync("auth_token");

                if (string.IsNullOrEmpty(token))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        IsLoggedIn = false;
                        Favorites.Clear();
                    });
                    return;
                }

                MainThread.BeginInvokeOnMainThread(() => IsLoggedIn = true);

                var items = await _apiService.GetMyFavoritesAsync(token);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Favorites.Clear();
                    foreach (var item in items)
                        Favorites.Add(item);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FavoritesViewModel Error]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
    }
}
