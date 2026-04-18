namespace MiniTFG;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void SessionClicked(object sender, EventArgs e)
	{
		// Asier, borra esto y hazlo con tu base de datos. Este botón es para iniciar sesión
		var api = new ApiService();

        if (string.IsNullOrWhiteSpace(emailEntry.Text) || string.IsNullOrWhiteSpace(passwordEntry.Text))
        {
            await DisplayAlertAsync("Error", "Rellena todos los campos", "OK");
            return;
        }

        var usuario = await api.LoginAsync(emailEntry.Text, passwordEntry.Text);

		if (usuario != null)
		{
            App.UsuarioActual = usuario; // No borres esto, es para guardar el usuario actual en la aplicación (usuario rellenalo con los datos que te da tu base de datos)
            if (chkRemember.IsChecked)
			{
				Preferences.Set("userId", usuario.Id);
				Preferences.Set("userName", usuario.Nombre);
				Preferences.Set("userCorreo", usuario.Correo);
            }
			await Shell.Current.GoToAsync("//home");
		}
		else
		{
			await DisplayAlertAsync("Error", "Correo o contraseña incorrectos.", "OK");
		}
    }

    private async void GuestClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//home");
    }

    private async void CreateClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("name");
    }

	private void ShowCheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		if (chkShow.IsChecked)
		{
			passwordEntry.IsPassword = false;
		}
		else
		{
			passwordEntry.IsPassword = true;
		}
	}
}