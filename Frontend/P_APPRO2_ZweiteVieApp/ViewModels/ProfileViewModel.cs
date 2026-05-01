using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UserPhone));
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(UserEmail));
                OnPropertyChanged(nameof(UserTotalPubs));
                OnPropertyChanged(nameof(UserTotalDonated));
                OnPropertyChanged(nameof(UserTotalCo2));
            }
        }

        public string UserPhone    => _currentUser?.UsePhone ?? "Non renseigné";
        public string UserName     => _currentUser?.UseName;
        public string UserEmail    => _currentUser?.UseEmail;
        public int    UserTotalPubs     => _currentUser?.TotalPubs ?? 0;
        public int    UserTotalDonated  => _currentUser?.TotalDonated ?? 0;
        public decimal UserTotalCo2    => _currentUser?.TotalCo2 ?? 0;

        public ObservableCollection<Publication> MyPublications { get; } = new();

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set { _isLoggedIn = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotLoggedIn)); }
        }
        public bool IsNotLoggedIn => !_isLoggedIn;

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set { _isRefreshing = value; OnPropertyChanged(); }
        }

        public ICommand EditProfileCommand { get; }
        public ICommand ShareProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand DeletePublicationCommand { get; }
        public ICommand MarkAsDonatedCommand { get; }
        public ICommand RefreshCommand { get; }

        public ProfileViewModel()
        {
            _apiService = new ApiService();
            CurrentUser = new User
            {
                UseName = Preferences.Get("user_name", "Utilisateur"),
                UseEmail = Preferences.Get("user_email", ""),
                UseId = Preferences.Get("user_id", 0)
            };

            EditProfileCommand = new Command(async () => await ExecuteEditProfile());
            ShareProfileCommand = new Command(async () => await ExecuteShareProfile());
            MarkAsDonatedCommand = new Command<Publication>(async (pub) => await ExecuteMarkAsDonated(pub));
            LogoutCommand = new Command(async () => await ExecuteLogout());
            DeletePublicationCommand = new Command<Publication>(async (pub) => await ExecuteDeletePublication(pub));
            RefreshCommand = new Command(async () =>
            {
                IsRefreshing = true;
                try { await LoadUserDataAsync(); }
                finally { IsRefreshing = false; }
            });

            IsLoggedIn = Preferences.Get("user_id", 0) > 0;
        }

        public async Task LoadUserDataAsync()
        {
            try
            {
                string token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                var userTask = _apiService.GetMyProfileAsync(token);
                var pubsTask = _apiService.GetMyPublicationsAsync(token);

                await Task.WhenAll(userTask, pubsTask);

                var user = userTask.Result;
                var pubs = pubsTask.Result;

                if (user != null)
                {
                    CurrentUser = user;
                    Preferences.Set("user_name", user.UseName ?? "");
                }

                MyPublications.Clear();
                foreach (var p in pubs)
                    MyPublications.Add(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileViewModel Error]: {ex.Message}");
            }
        }

        private async Task ExecuteEditProfile()
        {
            string choice = await Application.Current.MainPage.DisplayActionSheet(
                "Modifier le profil", "Annuler", null,
                "✏️  Nom d'utilisateur", "📞  Numéro de téléphone");

            if (choice == null || choice == "Annuler") return;

            string token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return;

            try
            {
                if (choice.Contains("Nom"))
                {
                    var newName = await Application.Current.MainPage.DisplayPromptAsync(
                        "Modifier le nom", "Nouveau nom d'utilisateur",
                        initialValue: CurrentUser?.UseName ?? "");
                    if (string.IsNullOrWhiteSpace(newName)) return;

                    bool ok = await _apiService.UpdateProfileAsync(token, newName, CurrentUser?.UsePhone ?? "");
                    if (ok)
                    {
                        Preferences.Set("user_name", newName);
                        await LoadUserDataAsync();
                        await Application.Current.MainPage.DisplayAlert("✓", "Nom mis à jour !", "OK");
                    }
                }
                else
                {
                    var newPhone = await Application.Current.MainPage.DisplayPromptAsync(
                        "Modifier le téléphone", "Nouveau numéro (ex: +41 76 000 00 00)",
                        initialValue: CurrentUser?.UsePhone ?? "",
                        keyboard: Keyboard.Telephone);
                    if (newPhone == null) return;

                    bool ok = await _apiService.UpdateProfileAsync(token, CurrentUser?.UseName ?? "", newPhone);
                    if (ok)
                    {
                        await LoadUserDataAsync();
                        await Application.Current.MainPage.DisplayAlert("✓", "Téléphone mis à jour !", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EditProfile Error]: {ex.Message}");
            }
        }

        private async Task ExecuteShareProfile()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Mon profil ZweiteVie",
                Text = $"Découvrez le profil de {CurrentUser?.UseName} sur ZweiteVie !\n" +
                       $"{CurrentUser?.TotalPubs} annonces · {CurrentUser?.TotalDonated} objets donnés · {CurrentUser?.TotalCo2}g CO₂ économisé"
            });
        }

        private async Task ExecuteDeletePublication(Publication pub)
        {
            if (pub == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Supprimer", $"Supprimer \"{pub.PubTitle}\" ?", "Oui", "Non");
            if (!confirm) return;

            try
            {
                string token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                bool success = await _apiService.DeletePublicationAsync(pub.PubId, token);
                if (success)
                    MainThread.BeginInvokeOnMainThread(() => MyPublications.Remove(pub));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DeletePublication Error]: {ex.Message}");
            }
        }

        private async Task ExecuteMarkAsDonated(Publication pub)
        {
            if (pub == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Marquer comme donné", $"Retirer \"{pub.PubTitle}\" du catalogue ?", "Oui", "Non");
            if (!confirm) return;

            try
            {
                string token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Token manquant.", "OK");
                    return;
                }

                Console.WriteLine($"[MarkAsDonated] pubId={pub.PubId}");
                bool success = await _apiService.MarkAsDonatedAsync(pub.PubId, token);
                Console.WriteLine($"[MarkAsDonated] success={success}");

                if (success)
                    MainThread.BeginInvokeOnMainThread(() => MyPublications.Remove(pub));
                else
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Impossible de marquer comme donné. Vérifiez les logs.", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MarkAsDonated Error]: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Erreur", ex.Message, "OK");
            }
        }

        private async Task ExecuteLogout()
        {
            SecureStorage.Remove("auth_token");
            Preferences.Clear();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsLoggedIn = false;
                MyPublications.Clear();
                CurrentUser = GuestUser();
            });
            await Shell.Current.GoToAsync("//LoginPage");
        }

        public static User GuestUser() => new User
        {
            UseName = "Invité",
            UseEmail = "invité@email.com",
            TotalPubs = 0,
            TotalDonated = 0,
            TotalCo2 = 0
        };
    }
}
