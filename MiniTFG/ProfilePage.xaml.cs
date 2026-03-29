using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;

namespace MiniTFG;

public partial class ProfilePage : ContentPage
{
    public ObservableCollection<Receta> MisRecetas { get; set; } = new();
    double lastScrollY = 0;
    bool isBarHidden = false;
    public ProfilePage()
	{
		InitializeComponent();
        UsernameLabel.BindingContext = App.UsuarioActual;
        ListaMisRecetas.BindingContext = this;
        CargarRecetas();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var api = new ApiService();
        App.UsuarioActual = await api.GetUsuarioByIdAsync(App.UsuarioActual.Id);

        MostrarEstrellas(App.UsuarioActual.ValoracionMedia);

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

    private async void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        double currentScrollY = e.VerticalOffset;
        if (currentScrollY > lastScrollY + 5 && !isBarHidden) // +5 to add a small threshold before hiding the bar
        {
            isBarHidden = true;
            await BottomBar.TranslateToAsync(0, 80, 250, Easing.CubicIn); // x, y, duration, easing
        }
        else if (currentScrollY < lastScrollY - 5 && isBarHidden) // -5 to add a small threshold before showing the bar again
        {
            isBarHidden = false;
            await BottomBar.TranslateToAsync(0, 0, 250, Easing.CubicOut); // x, y, duration, easing
        }
        lastScrollY = currentScrollY;
    }

    private async void CargarRecetas()
    {
        var api = new ApiService();
        var lista = await api.GetRecetasAsync();

        if (lista == null)
            return;
        MisRecetas.Clear();
        foreach (var mia in lista)
        {
            if (mia.UsuarioId == App.UsuarioActual.Id)
                if (!string.IsNullOrEmpty(mia.Imagen))
                {
                    byte[] bytes = Convert.FromBase64String(mia.Imagen);
                    mia.Imagen = null; // evitamos usar el string enorme como Source
                    mia.ImagenSource = ImageSource.FromStream(() => new MemoryStream(bytes));
                }
            MisRecetas.Add(mia);
        }
    }

    private Grid CrearEstrella(double porcentaje)
    {
        const double STAR_SIZE = 30; // Debe coincidir con WidthRequest/HeightRequest
        
        var grid = new Grid
        {
            WidthRequest = STAR_SIZE,
            HeightRequest = STAR_SIZE
        };

        var empty = new Image
        {
            Source = "starempty.png",
            Aspect = Aspect.Fill
        };

        var full = new Image
        {
            Source = "starfull.png",
            Aspect = Aspect.Fill
        };

        full.Clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, STAR_SIZE * porcentaje, STAR_SIZE)  // ✅ CORRECTO
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