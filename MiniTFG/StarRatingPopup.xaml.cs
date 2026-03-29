using CommunityToolkit.Maui.Views;

namespace MiniTFG;

public partial class StarRatingPopup : Popup
{
    private readonly int _usuarioValoradoId;
    public StarRatingPopup(int usuarioValoradoId)
	{
		InitializeComponent();
        _usuarioValoradoId = usuarioValoradoId;

        Star1.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { ActualizarEstrellas(1); Seleccionar(1); }) });
        Star2.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { ActualizarEstrellas(2); Seleccionar(2); }) });
        Star3.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { ActualizarEstrellas(3); Seleccionar(3); }) });
        Star4.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { ActualizarEstrellas(4); Seleccionar(4); }) });
        Star5.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { ActualizarEstrellas(5); Seleccionar(5); }) });

    }

    private async void Seleccionar(int estrellas)
    {
        var api = new ApiService();
        int usuarioQueValoraId = App.UsuarioActual.Id;

        await api.PostValoracionAsync(new Valoracion
        {
            UsuarioValoradoId = _usuarioValoradoId,
            UsuarioQueValoraId = usuarioQueValoraId,
            Puntuacion = estrellas
        });

        Close(estrellas);
    }

    private void ActualizarEstrellas(int estrellas)
    {
        var lista = new List<Image> { Star1, Star2, Star3, Star4, Star5 };

        for (int i = 0; i < lista.Count; i++)
        {
            lista[i].Source = i < estrellas ? "starfull.png" : "star.png";
        }
    }

}