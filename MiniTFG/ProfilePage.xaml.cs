namespace MiniTFG;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}

	private async void InicioClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//home");
	}
	private async void RecetasClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//recipes");
    }

	private async void AbrirTiendaClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("shop");
    }
}