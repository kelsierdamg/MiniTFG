using Microsoft.Maui.Controls.Shapes;

namespace MiniTFG;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();

       // double media = PerfilManager.ObtenerMedia(usuarioId);
        //MostrarEstrellas(media);

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

    private Grid CrearEstrella(double porcentaje)
    {
        var grid = new Grid
        {
            WidthRequest = 30,
            HeightRequest = 30
        };

        var empty = new Image
        {
            Source = "star_empty.png",
            Aspect = Aspect.Fill
        };

        var full = new Image
        {
            Source = "star_full.png",
            Aspect = Aspect.Fill
        };

        full.Clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, porcentaje, 1)
        };

        grid.Children.Add(empty);
        grid.Children.Add(full);

        return grid;
    }

    private void MostrarEstrellas(double media)
    {
        EstrellasContainer.Children.Clear();

        for (int i = 1; i <= 5; i++)
        {
            double porcentaje;

            if (media >= i)
                porcentaje = 1;
            else if (media <= i - 1)
                porcentaje = 0;
            else
                porcentaje = media - (i - 1);

            EstrellasContainer.Children.Add(CrearEstrella(porcentaje));
        }
    }

}