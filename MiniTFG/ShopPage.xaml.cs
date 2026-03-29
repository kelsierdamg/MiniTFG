using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniTFG;

public partial class ShopPage : ContentPage, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private int _likesTotales;

    public int LikesTotales
    {
        get => _likesTotales;
        set
        {
            if (_likesTotales != value)
            {
                _likesTotales = value;
                OnPropertyChanged();
            }
        }
    }

    public ShopPage()
	{
		InitializeComponent();
        BindingContext = this;
        CargarLikesTotales();

        foreach (var banner in SkinsManager.BannersDisponibles)
            BannersContainer.Children.Add(CrearItemSkin(banner, true));

        foreach (var foto in SkinsManager.FotosDisponibles)
            FotosContainer.Children.Add(CrearItemSkin(foto, false));
    }

    private async void CargarLikesTotales()
    {
        int usuarioId = App.UsuarioActual?.Id ?? 0;

        if (usuarioId == 0)
        {
            LikesTotales = 0;
            LikesLabel.Text = "0"; // o lo que uses en XAML
            return;
        }

        var api = new ApiService();
        var likesUsuario = await api.GetLikesUsuarioAsync(usuarioId);

        LikesTotales = likesUsuario.Count();
        LikesLabel.Text = $"Likes: {LikesTotales}";
    }

    private View CrearItemSkin(Skin skin, bool esBanner)
    {
        var imagen = new Image
        {
            Source = skin.Image,
            HeightRequest = esBanner ? 120 : 80,
            WidthRequest = esBanner ? 220 : 80,
            Aspect = Aspect.AspectFill
        };

        var precio = new Label
        {
            Text = $"{skin.Precio} likes",
            FontSize = 14,
            HorizontalOptions = LayoutOptions.Center
        };

        var boton = new Button
        {
            Text = skin.Comprado ? "Usar" : "Comprar",
            BackgroundColor = skin.Comprado ? Color.FromArgb("#4CAF50") : Color.FromArgb("#9C40F7"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 40,
            WidthRequest = 120
        };
        /*
        boton.Clicked += (s, e) =>
        {
            if (!skin.Comprado)
                ComprarSkin(skin);
            else
                UsarSkin(skin, esBanner);
        };
        */
        return new VerticalStackLayout
        {
            Spacing = 5,
            Children = { imagen, precio, boton }
        };
    }

}