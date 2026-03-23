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

	private async void FinishClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//home");
    }
}