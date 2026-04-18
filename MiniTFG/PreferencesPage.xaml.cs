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
		foreach (var item in Preferencias) // Recorremos las preferencias para asignarlas al usuario temporal
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

        // De nuevo aquí uso mi api, cambialo por los métodos de tu bbdd
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

        App.UsuarioActual = usuarioLogueado; // Asignamos el usuario logueado a la variable global

        await Shell.Current.GoToAsync("//home");
    }
}