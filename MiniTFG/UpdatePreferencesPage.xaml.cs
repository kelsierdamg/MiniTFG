namespace MiniTFG;

public partial class UpdatePreferencesPage : ContentPage
{
    public List<AlergenosPreferencias> Preferencias { get; set; }
    public UpdatePreferencesPage()
	{
		InitializeComponent();

        Preferencias = new List<AlergenosPreferencias>
        {
            new AlergenosPreferencias { Nombre = "Gluten", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Leche", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Frutos secos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Marisco", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Huevos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Soja", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Pescado", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Cacahuetes", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Sésamo", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Sulfitos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Mostaza", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Altramuces", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Moluscos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Apio", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Vegetariano", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Vegano", Seleccion = false }
        };
    }

    private async void UpdateCLicked(object sender, EventArgs e)
    {
        // Asier, aquí pon los metodos de tu bbdd para actualizar las preferencias del usuario, usando la lista Preferencias que se ha actualizado con los cambios del usuario
        await DisplayAlertAsync("Preferencias actualizadas", "Tus preferencias han sido actualizadas correctamente.", "OK");
        await Shell.Current.GoToAsync("//home");
    }
}