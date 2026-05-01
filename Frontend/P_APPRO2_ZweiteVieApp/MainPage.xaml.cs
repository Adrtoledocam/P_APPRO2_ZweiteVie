using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.ViewModels;

namespace P_APPRO2_ZweiteVieApp
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is MainViewModel vm && Preferences.Get("user_id", 0) == 0)
            {
                foreach (var pub in vm.Publications)
                    pub.IsFavorited = false;
            }
        }

        private async void OnContacterClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Publication pub)
            {
                Preferences.Set("selected_pub_id", pub.PubId);
                Preferences.Set("selected_pub_is_fav", pub.IsFavorited ? 1 : 0);
                await Shell.Current.GoToAsync("PublicationDetailPage");
            }
        }
    }
}
