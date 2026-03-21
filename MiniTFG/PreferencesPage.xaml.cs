namespace MiniTFG;

public partial class PreferencesPage : ContentPage
{
	public List<AlergenosPreferencias> Preferencias { get; set; }
	public PreferencesPage()
	{
		InitializeComponent();

		Preferencias = new List<AlergenosPreferencias>
		{
			new AlergenosPreferencias { Nombre = "Vegetariano", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Vegano", Seleccion = false }
		};

		BindingContext = this;
    }

	private void FinishClicked(object sender, EventArgs e)
	{
		Application.Current.MainPage = new NavigationPage(new HomePage());
    }
}