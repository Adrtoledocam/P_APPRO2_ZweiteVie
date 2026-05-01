using P_APPRO2_ZweiteVieApp.ViewModels;

namespace P_APPRO2_ZweiteVieApp.Views;

public partial class AddPublicationPage : ContentPage
{
    public AddPublicationPage()
    {
        InitializeComponent();
        var vm = new AddPublicationViewModel();
        vm.OnFormCleared = () =>
        {
            TitleEntry.Text = "";
            DescriptionEditor.Text = "";
            LocationEntry.Text = "";
            CategoryPicker.SelectedIndex = -1;
            ConditionPicker.SelectedIndex = -1;
            PreviewImage.Source = null;
            PreviewImage.IsVisible = false;
            NoPhotoLabel.IsVisible = true;
        };
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AddPublicationViewModel vm)
            vm.RefreshLoginState();
    }

    private async void OnGoToLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            string action = await DisplayActionSheet(
                "Choisir une source", "Annuler", null,
                "Prendre une photo", "Choisir depuis la galerie");

            if (action == null || action == "Annuler") return;

            FileResult photo;
            if (action == "Prendre une photo")
                photo = await MediaPicker.Default.CapturePhotoAsync();
            else
                photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo == null) return;

            // Preview — stream stays open (same as other project)
            var stream = await photo.OpenReadAsync();
            PreviewImage.Source = ImageSource.FromStream(() => stream);
            PreviewImage.IsVisible = true;
            NoPhotoLabel.IsVisible = false;

            // Base64 for the API — separate stream
            using var ms = new MemoryStream();
            using var streamForBytes = await photo.OpenReadAsync();
            await streamForBytes.CopyToAsync(ms);
            string base64 = $"data:{photo.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

            if (BindingContext is AddPublicationViewModel vm)
                vm.SetImageBase64(base64);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Impossible de charger la photo : {ex.Message}", "OK");
        }
    }
}
