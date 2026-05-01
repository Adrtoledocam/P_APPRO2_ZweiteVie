using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.ViewModels;

namespace P_APPRO2_ZweiteVieApp.Views;

public partial class FavoritesPage : ContentPage
{
    public FavoritesPage()
    {
        InitializeComponent();
        BindingContext = new FavoritesViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is FavoritesViewModel vm)
        {
            vm.IsLoggedIn = Preferences.Get("user_id", 0) > 0;
            Task.Run(async () => await vm.LoadFavoritesAsync());
        }
    }

    private async void OnContacterClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Publication pub)
        {
            Preferences.Set("selected_pub_id", pub.PubId);
            Preferences.Set("selected_pub_is_fav", 1);
            await Shell.Current.GoToAsync("PublicationDetailPage");
        }
    }

    private async void OnGoToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
