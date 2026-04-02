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

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); }
        }


        public ICommand RefreshCommand { get; }
        public ICommand SearchCommand { get; }

        public ICommand SelectPublicationCommand => new Command<Publication>(async (pub) =>
        {
            var param = new Dictionary<string, object> { { "SelectedPublication", pub } };
            await Shell.Current.GoToAsync("PublicationDetailPage", param);
        });
        public MainViewModel()
        {
            _apiService = new ApiService();
            RefreshCommand = new Command(async () => await LoadPublicationsAsync());
            SearchCommand = new Command(async () => await LoadPublicationsAsync(SearchQuery));

            Task.Run(async () => await LoadPublicationsAsync());
        }

        public async Task LoadPublicationsAsync(string search = "")
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var items = await _apiService.GetPublicationsAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Publications == null)
                        Publications = new ObservableCollection<Publication>();

                    Publications.Clear(); // Es mejor limpiar antes de recargar

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
