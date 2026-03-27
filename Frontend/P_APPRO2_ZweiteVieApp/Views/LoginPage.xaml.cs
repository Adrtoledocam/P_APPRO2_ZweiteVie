namespace P_APPRO2_ZweiteVieApp.Views;
using P_APPRO2_ZweiteVieApp.ViewModels;


public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel();

    }
}