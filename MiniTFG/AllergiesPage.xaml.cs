namespace MiniTFG;

public partial class AllergiesPage : ContentPage
{
	public List<AlergenosPreferencias> Alergenos { get; set; }

    public AllergiesPage()
	{
		InitializeComponent();

		Alergenos = new List<AlergenosPreferencias>
		{
			new AlergenosPreferencias { Nombre = "Gluten", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Leche", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Frutos secos", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Mariscos", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Huevos", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Soja", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Pescado", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Cacahuetes", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Sésamo", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Sulfitos", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Mostaza", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Altramuces", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Moluscos", Seleccion = false },
			new AlergenosPreferencias { Nombre = "Apio", Seleccion = false }
		};

        BindingContext = this;
    }

	private void NextClicked(object sender, EventArgs e)
	{
		// Aquí puedes guardar las preferencias seleccionadas en una base de datos o en la configuración de la aplicación
		// Por ejemplo, podrías usar Preferences.Set("Alergenos", JsonSerializer.Serialize(Alergenos));
		// Luego, navega a la siguiente página
		Navigation.PushAsync(new PreferencesPage());
    }
}