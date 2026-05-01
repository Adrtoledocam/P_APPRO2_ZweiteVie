using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.ViewModels;

namespace P_APPRO2_ZweiteVieApp.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ProfileViewModel vm)
        {
            vm.IsLoggedIn = Preferences.Get("user_id", 0) > 0;
            if (vm.IsLoggedIn)
                await vm.LoadUserDataAsync();
            else
            {
                vm.CurrentUser = ProfileViewModel.GuestUser();
                vm.MyPublications.Clear();
            }
        }
    }

    private async void OnGoToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
