using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;
using System.Windows.Input;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{
    public class ProfileViewModel: BaseViewModel
    {
        private readonly ApiService _apiService;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        private string _editName;
        public string EditName
        {
            get => _editName;
            set { _editName = value; OnPropertyChanged(); }
        }

        private string _editPhone;
        public string EditPhone
        {
            get => _editPhone;
            set { _editPhone = value; OnPropertyChanged(); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ICommand SaveProfileCommand { get; }
        public ICommand LogoutCommand { get; }


        public ProfileViewModel()
        {
            _apiService = new ApiService();
            CurrentUser = new User {
                UseName = Preferences.Get("user_name", "User"),
                UseEmail = Preferences.Get("user_email", "Email"),
                UseId = Preferences.Get("user_id", 0)
            };
            SaveProfileCommand = new Command(async () => await ExecuteSaveProfile());
            LogoutCommand = new Command(async () => await ExecuteLogout());
            //Task.Run(async () => await LoadProfileAsync());
            Task.Run(async () => await LoadUserDataAsync());
            //_ = LoadUserDataAsync();
        }

        private async Task LoadUserDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                string token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                var user = await _apiService.GetMyProfileAsync(token);

                if (user != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CurrentUser = user;
                        EditName = user.UseName;
                        EditPhone = user.UsePhone;

                        Preferences.Set("user_name", user.UseName);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileViewModel] Error al cargar: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        /*
        public async Task LoadProfileAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                string token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                var user = await _apiService.GetMyProfileAsync(token);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CurrentUser = user;

                    EditName = user?.UseName;
                    EditPhone = user?.UsePhone;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileViewModel Error]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        */
        private async Task ExecuteSaveProfile()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = string.Empty;

            try
            {
                string token = await SecureStorage.GetAsync("auth_token");
                if (string.IsNullOrEmpty(token)) return;

                bool success = await _apiService.UpdateProfileAsync(token, EditName, EditPhone);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (success)
                    {
                        if (CurrentUser != null)
                        {
                            CurrentUser.UseName = EditName;
                            CurrentUser.UsePhone = EditPhone;
                            OnPropertyChanged(nameof(CurrentUser));
                        }
                        StatusMessage = "Profil mis à jour !";
                    }
                    else
                    {
                        StatusMessage = "Erreur lors de la mise à jour.";
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileViewModel] ExecuteSaveProfile error: {ex.Message}");
                StatusMessage = "Une erreur est survenue.";
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task ExecuteLogout()
        {

            SecureStorage.Remove("auth_token");
            Preferences.Clear();
            CurrentUser = null;

            /**SecureStorage.Remove("auth_token");
            Preferences.Remove("user_id");
            Preferences.Remove("user_name");
            Preferences.Remove("user_email");
            Preferences.Clear();**/

            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
