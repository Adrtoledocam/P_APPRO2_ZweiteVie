using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using P_APPRO2_ZweiteVieApp.Services;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }


        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }   
        public ICommand GuestCommand { get; }
        public ICommand GoToRegisterCommand { get; }


        public LoginViewModel()
        {
            _apiService = new ApiService();
            LoginCommand = new Command(async () => await ExecuteLogin(), () => !IsBusy);
            GuestCommand = new Command(async () => await Shell.Current.GoToAsync("//MainPage"));
            GoToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("RegisterPage"));
        }
        private async Task ExecuteLogin()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password)) 
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Champs requis", "Veuillez remplir l'email et le mot de passe.", "OK");
                return;
            }            

            IsBusy = true;

            try
            {
                var loginResult = await _apiService.LoginAsync(Email, Password);

                if (loginResult != null && !string.IsNullOrEmpty(loginResult.Token))
                {
                    await SecureStorage.SetAsync("auth_token", loginResult.Token);

                    Preferences.Set("user_id", loginResult.User.UseId);
                    Preferences.Set("user_name", loginResult.User.UseName);
                    Preferences.Set("user_email", loginResult.User.UseEmail);

                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", "Email ou mot de passe incorrect", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginViewModel Error]: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Erreur réseau", "Impossible de contacter le serveur.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
