namespace P_APPRO2_ZweiteVieApp.Pages
{
    public partial class ManageMetaPage : ContentPage
    {
        public ManageMetaPage(ManageMetaPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}