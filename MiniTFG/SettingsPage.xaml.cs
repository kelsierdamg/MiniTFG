namespace MiniTFG;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

	private async void CambiarUsernameClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("updateusername");
    }

	private async void CambiarEmailClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("updateemail");
    }

	private async void CambiarPasswordClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("updatepassword");
	}

	private async void CambiarPreferenciasClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("updatepreferences");
    } 

	private async void CerrarSesionClicked(object sender, EventArgs e)
	{
		var answer = await DisplayAlertAsync("Cerrar sesión", "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");
		if (answer)
		{
			Preferences.Remove("userId");
			Preferences.Remove("userName");
			Preferences.Remove("userCorreo");
			App.UsuarioActual = null;
			App.UsuarioTemporal = null;
			await Shell.Current.GoToAsync("//login");
		}
    }
}