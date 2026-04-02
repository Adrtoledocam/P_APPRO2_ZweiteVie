using P_APPRO2_ZweiteVieApp.ViewModels;

namespace P_APPRO2_ZweiteVieApp.Views;

public partial class AddPublicationPage : ContentPage
{
	public AddPublicationPage()
	{
		InitializeComponent();
        BindingContext = new AddPublicationViewModel();

    }
}