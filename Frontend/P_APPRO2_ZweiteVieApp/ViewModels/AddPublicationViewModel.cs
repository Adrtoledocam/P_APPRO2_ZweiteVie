
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
    public class AddPublicationViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;

        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }        
        private string _location;
        public string Location
        {
            get => _location;
            set { _location = value; OnPropertyChanged(); }
        }

        private string _imageBase64;
        public void SetImageBase64(string base64) => _imageBase64 = base64;


        public List<ItemCondition> ItemConditions { get; } = new List<ItemCondition>
        {
        new ItemCondition { ConId = 1, ConName = "Neuf" },
        new ItemCondition { ConId = 2, ConName = "Bon état" },
        new ItemCondition { ConId = 3, ConName = "Dommages superficiels" },
        new ItemCondition { ConId = 4, ConName = "Endommagé mais fonctionnel" },
        new ItemCondition { ConId = 5, ConName = "Cassé / Pour pièces" }
         };

        private ItemCondition _selectedCondition;
        public ItemCondition SelectedCondition
        {
            get => _selectedCondition;
            set { _selectedCondition = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Category> Categories { get; set; } = new();

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); }
        }        

        public ICommand SubmitCommand { get; }
        public ICommand ClearFormCommand { get; }

        public Action OnFormCleared { get; set; }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set { _isLoggedIn = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsNotLoggedIn)); }
        }
        public bool IsNotLoggedIn => !_isLoggedIn;

        public AddPublicationViewModel()
        {
            _apiService = new ApiService();
            SubmitCommand = new Command(async () => await ExecuteSubmit());
            ClearFormCommand = new Command(ExecuteClearForm);

            IsLoggedIn = Preferences.Get("user_id", 0) > 0;
            if (IsLoggedIn)
                Task.Run(async () => await LoadCategoriesAsync());
        }

        public void RefreshLoginState()
        {
            IsLoggedIn = Preferences.Get("user_id", 0) > 0;
            if (IsLoggedIn && Categories.Count == 0)
                Task.Run(async () => await LoadCategoriesAsync());
        }

        private async Task LoadCategoriesAsync()
        {
            var items = await _apiService.GetCategoriesAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Categories.Clear();
                foreach (var item in items)
                    Categories.Add(item);
            });
        }

        private void ExecuteClearForm()
        {
            Title = string.Empty;
            Description = string.Empty;
            Location = string.Empty;
            SelectedCategory = null;
            SelectedCondition = null;
            _imageBase64 = null;
            OnFormCleared?.Invoke();
        }

        private async Task ExecuteSubmit()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(Title))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Champ requis", "Veuillez entrer un titre.", "OK");
                return;
            }
            if (SelectedCategory == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Champ requis", "Veuillez choisir une catégorie.", "OK");
                return;
            }
            if (SelectedCondition == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Champ requis", "Veuillez choisir l'état du produit.", "OK");
                return;
            }
            if (string.IsNullOrEmpty(_imageBase64))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Image requise", "Veuillez ajouter une photo.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                string token = await SecureStorage.GetAsync("auth_token");

                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Non connecté", "Veuillez vous connecter pour publier.", "OK");
                    return;
                }

                // ✅ On envoie conId (INT) — cohérent avec la nouvelle DB et le controller
                bool success = await _apiService.CreatePublicationAsync(
                    token,
                    Title,
                    Description,
                    SelectedCondition.ConId,  
                    Location,
                    SelectedCategory.CatId,
                    _imageBase64
                );

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Succès", "Votre annonce a été publiée !", "OK");
                    ExecuteClearForm();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Erreur", "La publication a échoué. Vérifiez votre connexion.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AddPublicationViewModel Error]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
