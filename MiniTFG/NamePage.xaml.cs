namespace MiniTFG;

public partial class NamePage : ContentPage
{
	public NamePage()
	{
		InitializeComponent();
	}

    private void ShowCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (chkShow.IsChecked)
        {
            passwordEntry.IsPassword = false;
            passwordRepeatEntry.IsPassword = false;
        }
        else
        {
            passwordEntry.IsPassword = true;
            passwordRepeatEntry.IsPassword = true;
        }
    }

    public async void NextClicked(object sender, EventArgs e)
	{
        if (passwordEntry.Text != passwordRepeatEntry.Text)
        {
            await DisplayAlertAsync("Error", "Las contraseñas no coinciden", "OK");
            return;
        }
        else
        {
            App.UsuarioTemporal = new Usuario
            {
                Nombre = nameEntry.Text,
                Correo = emailEntry.Text,
                Contrasena = passwordEntry.Text
            };

            await Shell.Current.GoToAsync("allergies");
        }

    }
}