using P_APPRO2_ZweiteVieApp.Models;

namespace P_APPRO2_ZweiteVieApp.Pages
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage(ProjectDetailPageModel model)
        {
            InitializeComponent();

            BindingContext = model;
        }
    }
}
