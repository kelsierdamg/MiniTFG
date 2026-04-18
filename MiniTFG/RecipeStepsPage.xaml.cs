namespace MiniTFG;

[QueryProperty(nameof(RecetaId), "recetaId")]
public partial class RecipeStepsPage : ContentPage
{
    private int _recetaId;
    public int RecetaId
    {
        get => _recetaId;
        set
        {
            _recetaId = value;
            CargarPasos();
        }
    }

    public RecipeStepsPage()
    {
        InitializeComponent();
    }

    private async void CargarPasos()
    {
        var api = new ApiService();

        var receta = await api.GetRecetaByIdAsync(_recetaId);
        if (receta != null)
            TituloLabel.Text = receta.Titulo;

        var pasos = await api.GetPasosRecetaAsync(_recetaId);

        foreach (var paso in pasos)
        {
            if (!string.IsNullOrEmpty(paso.Video))
            {
                var bytes = Convert.FromBase64String(paso.Video);
                var filePath = Path.Combine(FileSystem.CacheDirectory, $"paso_{paso.Id}.mp4");
                await File.WriteAllBytesAsync(filePath, bytes);
                paso.VideoFilePath = filePath;
            }
        }

        PasosCarousel.ItemsSource = pasos.OrderBy(p => p.NumeroPaso).ToList();
    }

    private async void OnVideoTapped(object sender, EventArgs e)
    {
        var grid = (Grid)sender;
        var paso = (PasoReceta)grid.BindingContext;

        if (!string.IsNullOrEmpty(paso.VideoFilePath))
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(paso.VideoFilePath)
            });
        }
    }
}
