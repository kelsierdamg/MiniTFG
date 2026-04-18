namespace MiniTFG;

public partial class UpdatePasswordPage : ContentPage
{
	public UpdatePasswordPage()
	{
		InitializeComponent();
	}

	private async void GuardarPasswordClicked(object sender, EventArgs e)
	{
		var currPass = CurrentPasswordEntry.Text;
		var newPass = NewPasswordEntry.Text;
		var confirmPass = ConfirmPasswordEntry.Text;

		if (newPass != confirmPass)
		{
			await DisplayAlertAsync("Error", "Las contraseñas no coinciden.", "OK");
			return;
		}

        // Asier, pon aquí el método de tu bbdd para actualizar la contraseña, usando currPass y newPass
		await Shell.Current.GoToAsync("//home");
    }
}