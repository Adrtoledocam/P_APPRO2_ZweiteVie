using P_APPRO2_ZweiteVieApp.ViewModels;

namespace P_APPRO2_ZweiteVieApp.Views;

public partial class PublicationDetailPage : ContentPage
{
	public PublicationDetailPage()
	{
		InitializeComponent();
        BindingContext = new PublicationDetailViewModel();

    }
}