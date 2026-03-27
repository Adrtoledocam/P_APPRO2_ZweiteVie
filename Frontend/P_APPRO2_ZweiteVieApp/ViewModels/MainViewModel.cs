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

        public ObservableCollection<Publication> Publications { get; set; }        

        public ICommand RefreshCommand { get; }
        /*
        public ICommand SelectPublicationCommand => new Command<Publication>(async (pub) =>
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedPublication", pub }
            };
            await Shell.Current.GoToAsync("PublicationDetailPage", navigationParameter);
        });
        */
        public ICommand SelectPublicationCommand => new Command<Publication>(async (pub) =>
        {
            if (pub == null) return;

            // 1. Guardamos el ID en los ajustes de la app (Preferences)
            Preferences.Set("last_selected_publication_id", pub.PubId);

            // 2. Navegamos pasando el objeto (esto es lo que ya tenías y es correcto)
            var navigationParameter = new Dictionary<string, object>
            {
                { "SelectedPublication", pub }
             };
            await Shell.Current.GoToAsync("PublicationDetailPage", navigationParameter);
        });
        public MainViewModel()
        {
            _apiService = new ApiService(); 
            Publications = new ObservableCollection<Publication>();
            RefreshCommand = new Command(async () => await LoadPublicationsAsync());

            Task.Run(async () => await LoadPublicationsAsync());
        }

        public async Task LoadPublicationsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var items = await _apiService.GetPublicationsAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Publications.Clear();
                    foreach (var item in items)
                    {
                        Publications.Add(item);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MainViewModel Error]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
