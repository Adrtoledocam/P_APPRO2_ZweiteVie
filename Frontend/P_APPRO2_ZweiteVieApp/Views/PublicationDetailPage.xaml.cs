using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.Services;

namespace P_APPRO2_ZweiteVieApp.Views;

public partial class PublicationDetailPage : ContentPage
{
    private readonly ApiService _apiService;

    public PublicationDetailPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        ContentArea.IsVisible = false;

        int pubId = Preferences.Get("selected_pub_id", 0);
        bool isFav = Preferences.Get("selected_pub_is_fav", 0) == 1;

        if (pubId == 0)
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            return;
        }

        try
        {
            var token = await SecureStorage.GetAsync("auth_token");
            var pub = await _apiService.GetPublicationByIdAsync(pubId, token);
            if (pub != null)
            {
                pub.IsFavorited = isFav;
                this.BindingContext = pub;
                HeartEmpty.IsVisible = !isFav;
                HeartFilled.IsVisible = isFav;
                ContentArea.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DetailPage Error]: {ex.Message}");
            await DisplayAlert("Erreur", "Impossible de charger les détails.", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnToggleFavoriteTapped(object sender, EventArgs e)
    {
        if (BindingContext is not Publication pub) return;

        var token = await SecureStorage.GetAsync("auth_token");
        if (string.IsNullOrEmpty(token))
        {
            await DisplayAlert("Non connecté", "Connectez-vous pour gérer vos favoris.", "OK");
            return;
        }

        if (pub.IsFavorited)
        {
            bool ok = await _apiService.RemoveFromFavoritesAsync(pub.PubId, token);
            if (ok)
            {
                pub.IsFavorited = false;
                HeartEmpty.IsVisible = true;
                HeartFilled.IsVisible = false;
            }
        }
        else
        {
            bool ok = await _apiService.AddToFavoritesAsync(pub.PubId, token);
            if (ok)
            {
                pub.IsFavorited = true;
                HeartEmpty.IsVisible = false;
                HeartFilled.IsVisible = true;
            }
        }
    }

    private async void OnEmailClicked(object sender, EventArgs e)
    {
        if (BindingContext is not Publication pub) return;
        var email = pub.UseEmail;
        if (string.IsNullOrEmpty(email) || email.StartsWith("Connectez"))
        {
            await DisplayAlert("Non disponible", "Connectez-vous pour contacter le donneur.", "OK");
            return;
        }
        await Launcher.Default.OpenAsync($"mailto:{email}?subject=ZweiteVie: {pub.PubTitle}");
    }

    private async void OnPhoneClicked(object sender, EventArgs e)
    {
        if (BindingContext is not Publication pub) return;
        var phone = pub.UsePhone;
        if (string.IsNullOrEmpty(phone) || phone.StartsWith("Connectez"))
        {
            await DisplayAlert("Non disponible", "Connectez-vous pour voir le numéro.", "OK");
            return;
        }
        await Launcher.Default.OpenAsync($"tel:{phone}");
    }
}
