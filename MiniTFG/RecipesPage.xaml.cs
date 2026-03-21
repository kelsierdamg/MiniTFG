namespace MiniTFG;

public partial class RecipesPage : ContentPage
{
	public RecipesPage()
	{
		InitializeComponent();
	}

	private void InicioClicked(object sender, EventArgs e)
	{
		Application.Current.MainPage = new NavigationPage(new HomePage());
    }
}