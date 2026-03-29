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

        _ = LoadStoreAsync();
    }

    private async Task LoadStoreAsync()
    {
        if (App.UsuarioActual == null) return;

        var api = new ApiService();
        var owned = await api.GetUserSkinsAsync(App.UsuarioActual.Id); // List<string> de SkinId
        var ownedSet = new HashSet<string>(owned);

        // marcar en SkinsManager
        foreach (var s in SkinsManager.BannersDisponibles)
            s.Comprado = ownedSet.Contains(s.Id);

        foreach (var s in SkinsManager.FotosDisponibles)
            s.Comprado = ownedSet.Contains(s.Id);

        // actualizar los controles visuales: recorre los children y actualiza bindings si hace falta
        // si usas Skin observable y bindings, no hace falta; si no, refresca manualmente:
        foreach (var child in BannersContainer.Children)
            (child as VisualElement)?.InvalidateMeasure();

        foreach (var child in FotosContainer.Children)
            (child as VisualElement)?.InvalidateMeasure();
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

        // Si Skin implementa INotifyPropertyChanged, actualizamos el botón cuando cambie Comprado
        if (skin is INotifyPropertyChanged npc)
        {
            npc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Skin.Comprado))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        boton.Text = skin.Comprado ? "Usar" : "Comprar";
                        boton.BackgroundColor = skin.Comprado ? Color.FromArgb("#4CAF50") : Color.FromArgb("#9C40F7");
                    });
                }
            };
        }

        boton.Clicked += async (s, e) =>
        {
            if (App.UsuarioActual == null)
            {
                await DisplayAlert("Inicia sesión", "Debes iniciar sesión para comprar o usar skins.", "OK");
                return;
            }

            if (!skin.Comprado)
                await ComprarSkinAsync(skin);
            else
                await UsarSkinAsync(skin, esBanner);
        };

        return new VerticalStackLayout
        {
            Spacing = 5,
            Children = { imagen, precio, boton }
        };
    }

    private async Task ComprarSkinAsync(Skin skin)
    {
        // ✅ VALIDAR LIKES
        if (LikesTotales < skin.Precio)
        {
            await DisplayAlert("Likes insuficientes", 
                $"Necesitas {skin.Precio} likes, tienes {LikesTotales}", "OK");
            return;
        }

        var api = new ApiService();
        bool ok = await api.PurchaseSkinAsync(App.UsuarioActual.Id, skin.Id);

        if (ok)
        {
            skin.Comprado = true;
            
            // ✅ DECREMENTAR LIKES LOCALMENTE
            LikesTotales -= skin.Precio;
            LikesLabel.Text = $"Likes: {LikesTotales}";
            
            await DisplayAlert("Comprado", $"Has comprado {skin.Id}. Te quedan {LikesTotales} likes.", "OK");
            
            // ✅ OPCIONAL: Refrescar desde servidor para sincronizar
        }
        else
        {
            await DisplayAlert("Error", "No se pudo completar la compra. Intenta de nuevo.", "OK");
            
            // ✅ Refrescar desde servidor si falla
        }
    }

    private async Task UsarSkinAsync(Skin skin, bool esBanner)
    {
        var api = new ApiService();
        bool ok = await api.ActivateSkinAsync(App.UsuarioActual.Id, skin.Id, esBanner ? "banner" : "foto");

        if (!ok)
        {
            await DisplayAlert("Error", "No se pudo activar la skin.", "OK");
            return;
        }

        // Actualiza App.UsuarioActual para que la UI del perfil muestre la skin
        // Decide si guardas SkinId o Image en Usuario.Foto/Banner. Aquí usamos Image (nombre de archivo).
        if (esBanner)
            App.UsuarioActual.Banner = skin.Image;
        else
            App.UsuarioActual.Foto = skin.Image;

        // Si Usuario implementa INotifyPropertyChanged, la UI se actualizará.
        await DisplayAlert("Activada", "Skin activada correctamente.", "OK");
    }

}