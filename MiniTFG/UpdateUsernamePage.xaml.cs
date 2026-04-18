namespace MiniTFG;

public partial class UpdateUsernamePage : ContentPage
{
	public UpdateUsernamePage()
	{
		InitializeComponent();
	}

	private async void GuardarUsernameClicked(object sender, EventArgs e)
	{
		string newUsername = UsernameEntry.Text?.Trim();
		// Asier, pon aquí la lógica para actualizar el username en tu base de datos o servicio
		await Shell.Current.GoToAsync("//home");
    }
}