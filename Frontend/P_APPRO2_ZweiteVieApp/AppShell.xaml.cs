using P_APPRO2_ZweiteVieApp.Views;

namespace P_APPRO2_ZweiteVieApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("RegisterPage", typeof(Views.RegisterPage));
            Routing.RegisterRoute("PublicationDetailPage", typeof(PublicationDetailPage));
        }
    }
}
