using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using P_APPRO2_ZweiteVieApp.Services;

namespace P_APPRO2_ZweiteVieApp.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }
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
        public ICommand RegisterCommand { get; }

        public RegisterViewModel()
        {
            _apiService = new ApiService();
            RegisterCommand = new Command(async () => await ExecuteRegister(),() => !IsBusy);
        }

        private async Task ExecuteRegister()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Champs requis", "Veuillez remplir tous les champs.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                bool success = await _apiService.RegisterAsync(Username, Email, Password);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Succès", "Compte créé ! Connectez-vous.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Erreur", "Cet email est déjà utilisé ou les données sont invalides.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RegisterViewModel Error]: {ex.Message}");
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
