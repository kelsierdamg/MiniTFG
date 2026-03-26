namespace MiniTFG;

public partial class ShopPage : ContentPage
{
	public ShopPage()
	{
		InitializeComponent();

        foreach (var banner in SkinsManager.BannersDisponibles)
            BannersContainer.Children.Add(CrearItemSkin(banner, true));

        foreach (var foto in SkinsManager.FotosDisponibles)
            FotosContainer.Children.Add(CrearItemSkin(foto, false));
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