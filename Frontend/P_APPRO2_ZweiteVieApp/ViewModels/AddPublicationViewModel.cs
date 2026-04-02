
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
        private ImageSource _previewImage;
        public ImageSource PreviewImage
        {
            get => _previewImage;
            set { _previewImage = value; OnPropertyChanged(nameof(HasImage)); }
        }
        public bool HasImage => _previewImage != null;


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

        public ICommand PickImageCommand { get; }
        public ICommand SubmitCommand { get; }

        public AddPublicationViewModel()
        {
            _apiService = new ApiService();
            PickImageCommand = new Command(async () => await ExecutePickImage());
            SubmitCommand = new Command(async () => await ExecuteSubmit());
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

        private async Task ExecutePickImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result == null) return;

                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                byte[] bytes = ms.ToArray();

                _imageBase64 = $"data:{result.ContentType};base64,{Convert.ToBase64String(bytes)}";
                PreviewImage = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PickImage Error]: {ex.Message}");
            }
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
                    await Shell.Current.GoToAsync("..");
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
