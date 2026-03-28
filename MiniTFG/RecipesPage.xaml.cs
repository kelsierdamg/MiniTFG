using System.Collections.ObjectModel;

namespace MiniTFG;

public partial class RecipesPage : ContentPage
{
    public ObservableCollection<AlergenosPreferencias> Alergenos { get; set; }
    public ObservableCollection<AlergenosPreferencias> Preferencias { get; set; }
    double lastScrollY = 0;
    bool isBarHidden = false;
    private string imagenBase64 = null;
    public RecipesPage()
	{
		InitializeComponent();

        Alergenos = new ObservableCollection<AlergenosPreferencias>
        {
            new AlergenosPreferencias { Nombre = "Gluten", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Leche", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Frutos secos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Marisco", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Huevos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Soja", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Pescado", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Cacahuetes", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Sésamo", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Sulfitos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Mostaza", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Altramuces", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Moluscos", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Apio", Seleccion = false }
        };

        Preferencias = new ObservableCollection<AlergenosPreferencias>
        {
            new AlergenosPreferencias { Nombre = "Vegano", Seleccion = false },
            new AlergenosPreferencias { Nombre = "Vegetariano", Seleccion = false },
        };
        BindingContext = this;
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
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                imagenBase64 = Convert.ToBase64String(imageBytes);
                
                ImagenPreview.Source = ImageSource.FromFile(foto.FullPath);
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
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                imagenBase64 = Convert.ToBase64String(imageBytes);
                
                ImagenPreview.Source = ImageSource.FromFile(foto.FullPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", "No se pudo seleccionar la imagen", "OK");
        }
    }

    private async void GuardarRecetaClicked(object sender, EventArgs e)
    {
        if (App.UsuarioActual == null)
        {
            await DisplayAlertAsync("Error", "Debes iniciar sesión para crear una receta.", "OK");
            return;
        }

        var receta = new Receta
        {
            UsuarioId = App.UsuarioActual.Id,
            Titulo = NombreEntry.Text,
            Imagen = imagenBase64,
            Comensales = int.Parse(ComensalesEntry.Text),
            OrigenDelPlato = OrigenEntry.Text,
            TiempoPreparacion = TiempoEntry.Text,
            TipoCocina = CocinaPicker.SelectedItem?.ToString(),
            IngredientePrincipal = MainIngredientEntry.Text
        };

        foreach (var alergeno in Alergenos)
        {
            switch (alergeno.Nombre)
            {
                case "Gluten":
                    receta.Gluten = alergeno.Seleccion;
                    break;
                case "Leche":
                    receta.Lactosa = alergeno.Seleccion;
                    break;
                case "Frutos secos":
                    receta.FrutosSecos = alergeno.Seleccion;
                    break;
                case "Marisco":
                    receta.Mariscos = alergeno.Seleccion;
                    break;
                case "Huevos":
                    receta.Huevo = alergeno.Seleccion;
                    break;
                case "Soja":
                    receta.Soja = alergeno.Seleccion;
                    break;
                case "Pescado":
                    receta.Pescado = alergeno.Seleccion;
                    break;
                case "Cacahuetes":
                    receta.Cacahuetes = alergeno.Seleccion;
                    break;
                case "Sésamo":
                    receta.Sesamo = alergeno.Seleccion;
                    break;
                case "Sulfitos":
                    receta.Sulfitos = alergeno.Seleccion;
                    break;
                case "Mostaza":
                    receta.Mostaza = alergeno.Seleccion;
                    break;
                case "Altramuces":
                    receta.Altramuces = alergeno.Seleccion;
                    break;
                case "Moluscos":
                    receta.Moluscos = alergeno.Seleccion;
                    break;
                case "Apio":
                    receta.Apio = alergeno.Seleccion;
                    break;
            }
        }

        foreach (var preferencia in Preferencias)
        {
            switch (preferencia.Nombre)
            {
                case "Vegano":
                    receta.Vegano = preferencia.Seleccion;
                    break;
                case "Vegetariano":
                    receta.Vegetariano = preferencia.Seleccion;
                    break;
            }
        }

        var api = new ApiService();
        var creada = await api.PostRecetaAsync(receta);

        if (creada == null)
        {
            await DisplayAlertAsync("Error", "No se pudo guardar la receta.", "OK");
            return;
        }

        await DisplayAlertAsync("Éxito", "Receta creada correctamente.", "OK");
    }
}