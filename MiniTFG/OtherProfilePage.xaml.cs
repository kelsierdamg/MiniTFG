using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;

namespace MiniTFG;

[QueryProperty(nameof(UsuarioId), "usuarioId")]
public partial class OtherProfilePage : ContentPage
{
	public ObservableCollection<Receta> MisRecetas { get; set; } = new();

	private int _usuarioId;
	public int UsuarioId
	{
		get => _usuarioId;
		set
		{
			_usuarioId = value;
			CargarPerfil(value);
		}
	}

	public OtherProfilePage()
	{
		InitializeComponent();
		ListaMisRecetas.BindingContext = this;
	}

	private async void CargarPerfil(int usuarioId)
	{
		var api = new ApiService();
		var usuario = await api.GetUsuarioByIdAsync(usuarioId);

		if (usuario == null)
			return;

		UsernameLabel.BindingContext = usuario;
		MostrarEstrellas(usuario.ValoracionMedia);

		// Cargar recetas primero (provoca cambios de layout en la CollectionView)
		await CargarRecetas(api, usuarioId);

		// Poner imágenes AL FINAL, después de que el layout esté estable
		ProfileImage.Source = await api.GetImageSourceAsync(usuario.Foto, "profile_default.png");
		BannerImage.Source = await api.GetImageSourceAsync(usuario.Banner, "banner_default.png");
	}

	private async Task CargarRecetas(ApiService api, int usuarioId)
	{
		var lista = await api.GetRecetasAsync();

		if (lista == null)
			return;

		MisRecetas.Clear();
		foreach (var receta in lista.Where(r => r.UsuarioId == usuarioId))
		{
			if (!string.IsNullOrEmpty(receta.Imagen))
			{
				byte[] bytes = Convert.FromBase64String(receta.Imagen);
				receta.Imagen = null;
				receta.ImagenSource = ImageSource.FromStream(() => new MemoryStream(bytes));
			}
			MisRecetas.Add(receta);
		}
	}

	private Grid CrearEstrella(double porcentaje)
	{
		const double STAR_SIZE = 30;

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
			Rect = new Rect(0, 0, STAR_SIZE * porcentaje, STAR_SIZE)
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