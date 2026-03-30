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
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStoreAsync();
    }


    private async Task LoadStoreAsync()
    {
        if (App.UsuarioActual == null) return;

        var api = new ApiService();
        var skins = await api.GetSkinsAsync(); // List<Skin>

        // 2. Separar banners y fotos (según tu criterio)
        var banners = skins.Where(s => s.Nombre.Contains("banner", StringComparison.OrdinalIgnoreCase)).ToList();
        var fotos = skins.Where(s => s.Nombre.Contains("foto", StringComparison.OrdinalIgnoreCase)).ToList();

        // 3. Obtener skins compradas
        var owned = await api.GetPurchasedUserSkinsAsync(App.UsuarioActual.Id); // List<int>
        var ownedSet = new HashSet<int>(owned);

        // 4. Marcar skins compradas
        foreach (var s in skins)
            s.Comprado = ownedSet.Contains(s.Id);

        // 5. Pintar en pantalla
        BannersContainer.Children.Clear();
        foreach (var b in banners)
            BannersContainer.Children.Add(CrearItemSkin(b, true));

        FotosContainer.Children.Clear();
        foreach (var f in fotos)
            FotosContainer.Children.Add(CrearItemSkin(f, false));
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
            Source = skin.Imagen,
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
                await DisplayAlertAsync("Inicia sesión", "Debes iniciar sesión para comprar o usar skins.", "OK");
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
            await DisplayAlertAsync("Likes insuficientes", 
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
            
            await DisplayAlertAsync("Comprado", $"Has comprado {skin.Id}. Te quedan {LikesTotales} likes.", "OK");
            
            // ✅ OPCIONAL: Refrescar desde servidor para sincronizar
        }
        else
        {
            await DisplayAlertAsync("Error", "No se pudo completar la compra. Intenta de nuevo.", "OK");
            
            // ✅ Refrescar desde servidor si falla
        }
    }

    private async Task UsarSkinAsync(Skin skin, bool esBanner)
    {
        var api = new ApiService();
        bool ok = await api.ActivateSkinAsync(App.UsuarioActual.Id, skin.Id, esBanner ? "banner" : "foto");

        if (!ok)
        {
            await DisplayAlertAsync("Error", "No se pudo activar la skin.", "OK");
            return;
        }

        // Actualiza App.UsuarioActual para que la UI del perfil muestre la skin
        // Decide si guardas SkinId o Image en Usuario.Foto/Banner. Aquí usamos Image (nombre de archivo).
        if (esBanner)
            App.UsuarioActual.Banner = skin.Imagen;
        else
            App.UsuarioActual.Foto = skin.Imagen;

        // Si Usuario implementa INotifyPropertyChanged, la UI se actualizará.
        await DisplayAlertAsync("Activada", "Skin activada correctamente.", "OK");
    }

}