using P_APPRO2_ZweiteVieApp.Models;
using P_APPRO2_ZweiteVieApp.PageModels;

namespace P_APPRO2_ZweiteVieApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}