namespace MiniTFG
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("name", typeof(NamePage));
            Routing.RegisterRoute("allergies", typeof(AllergiesPage));
            Routing.RegisterRoute("preferences", typeof(PreferencesPage));
            Routing.RegisterRoute("shop", typeof(ShopPage));
            Routing.RegisterRoute("other", typeof(OtherProfilePage));
        }
    }
}
