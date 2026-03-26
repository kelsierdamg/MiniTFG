namespace MiniTFG;

public partial class RecipesPage : ContentPage
{
    double lastScrollY = 0;
    bool isBarHidden = false;
    public RecipesPage()
	{
		InitializeComponent();
	}

	private async void InicioClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//home");
    }

    private async void PerfilClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//profile");
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

    private async void SeleccionarImagenClicked(object sender, EventArgs e)
    {
        string opcion = await DisplayActionSheetAsync(
        "Seleccionar imagen",
        "Cancelar",
        null,
        "Hacer foto",
        "Elegir de la galería");

        if (opcion == "Hacer foto")
        {
            await TomarFoto();
        }
        else if (opcion == "Elegir de la galería")
        {
            await ElegirDeGaleria();
        }
    }

    private async Task TomarFoto()
    {
        try
        {
            var foto = await MediaPicker.CapturePhotoAsync();

            if (foto != null)
            {
                using var stream = await foto.OpenReadAsync();
                ImagenPreview.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo abrir la cámara", "OK");
        }
    }

    private async Task ElegirDeGaleria()
    {
        try
        {
            var foto = await MediaPicker.PickPhotoAsync();

            if (foto != null)
            {
                using var stream = await foto.OpenReadAsync();
                ImagenPreview.Source = ImageSource.FromStream(() => stream);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo seleccionar la imagen", "OK");
        }
    }


}