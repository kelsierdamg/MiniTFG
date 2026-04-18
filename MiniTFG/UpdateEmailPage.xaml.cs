namespace MiniTFG;

public partial class UpdateEmailPage : ContentPage
{
	public UpdateEmailPage()
	{
		InitializeComponent();
	}

	private async void GuardarEmailClicked(object sender, EventArgs e)
	{
		string newEmail = EmailEntry.Text?.Trim();
		// Asier, pon aquí la lógica para actualizar el email en tu base de datos
		await Shell.Current.GoToAsync("//home");
    }
}