using System.Collections.ObjectModel;

namespace MiniTFG;

public partial class RecipesPage : ContentPage
{
    public ObservableCollection<AlergenosPreferencias> Alergenos { get; set; }
    public ObservableCollection<AlergenosPreferencias> Preferencias { get; set; }
    double lastScrollY = 0;
    bool isBarHidden = false;
    private string imagenBase64 = null;
    private List<PasoReceta> _pasos = new();
    private int _contadorPasos = 0;
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

    private async void AgregarPasoClicked(object sender, EventArgs e)
    {
        _contadorPasos++;
        int numeroPaso = _contadorPasos;

        var paso = new PasoReceta { NumeroPaso = numeroPaso };
        _pasos.Add(paso);

        var pasoLayout = new VerticalStackLayout { Spacing = 5, Padding = new Thickness(0, 5) };

        var label = new Label
        {
            Text = $"Paso {numeroPaso}",
            FontAttributes = FontAttributes.Bold,
            FontSize = 16
        };

        var descripcionEntry = new Entry
        {
            Placeholder = $"Descripción del paso {numeroPaso}",
            FontSize = 16
        };
        descripcionEntry.TextChanged += (s, args) => paso.Descripcion = args.NewTextValue;

        var videoLabel = new Label
        {
            Text = "Sin vídeo seleccionado",
            FontSize = 14,
            TextColor = Colors.Gray
        };

        var videoButton = new Button
        {
            Text = "Seleccionar vídeo",
            BackgroundColor = Color.FromArgb("#9C40F7"),
            TextColor = Colors.White,
            CornerRadius = 8
        };
        videoButton.Clicked += async (s, args) =>
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = $"Selecciona el vídeo del paso {numeroPaso}",
                    FileTypes = FilePickerFileType.Videos
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    paso.Video = Convert.ToBase64String(ms.ToArray());
                    videoLabel.Text = result.FileName;
                }
            }
            catch
            {
                await DisplayAlertAsync("Error", "No se pudo seleccionar el vídeo", "OK");
            }
        };

        pasoLayout.Children.Add(label);
        pasoLayout.Children.Add(descripcionEntry);
        pasoLayout.Children.Add(videoButton);
        pasoLayout.Children.Add(videoLabel);

        PasosContainer.Children.Add(pasoLayout);
    }

    private async void GuardarRecetaClicked(object sender, EventArgs e)
    {
        if (App.UsuarioActual == null)
        {
            await DisplayAlertAsync("Error", "Debes iniciar sesión para crear una receta.", "OK");
            return;
        }

        // Como siempre, borra las referencias a la api y pon tus métodos
        // Aquí rellenamos el objeto Receta con los datos del formulario
        var receta = new Receta
        {
            UsuarioId = App.UsuarioActual.Id,
            Titulo = NombreEntry.Text,
            Imagen = imagenBase64,
            Descripcion = DescripcionEntry.Text,
            Comensales = int.Parse(ComensalesEntry.Text),
            OrigenDelPlato = OrigenEntry.Text,
            TiempoPreparacion = TiempoEntry.Text,
            TipoCocina = CocinaPicker.SelectedItem?.ToString(),
            IngredientePrincipal = MainIngredientEntry.Text
        };

        // Asignamos los alérgenos y preferencias seleccionados
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

        foreach (var paso in _pasos)
        {
            paso.RecetaId = creada.Id;
            await api.PostPasoRecetaAsync(paso);
        }

        _pasos.Clear();
        _contadorPasos = 0;
        PasosContainer.Children.Clear();

        await DisplayAlertAsync("Éxito", "Receta creada correctamente.", "OK");
    }
}