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
		foreach (var item in Preferencias)
		{
			switch (item.Nombre)
			{
				case "Vegetariano":
					App.UsuarioTemporal.Vegetariano = item.Seleccion;
					break;
				case "Vegano":
					App.UsuarioTemporal.Vegano = item.Seleccion;
					break;
			}
        }

        var api = new ApiService();

        var creado = await api.PostUsuarioAsync(App.UsuarioTemporal);

        if (creado == null)
        {
            await DisplayAlertAsync("Error", "No se pudo crear la cuenta", "OK");
            return;
        }

        var usuarioLogueado = await api.LoginAsync(
            App.UsuarioTemporal.Correo,
            App.UsuarioTemporal.Contrasena
        );

        if (usuarioLogueado == null)
        {
            await DisplayAlertAsync("Error", "La cuenta se creó, pero no se pudo iniciar sesión", "OK");
            return;
        }

        App.UsuarioActual = usuarioLogueado;

        await Shell.Current.GoToAsync("//home");
    }
}