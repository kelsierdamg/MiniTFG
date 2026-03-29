using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MiniTFG;

public partial class HomePage : ContentPage
{
    public ObservableCollection<Receta> Recetas { get; set; } = new();
    private List<Receta> TodasLasRecetas;
    private List<Receta> RecetasFiltradasBase;
    public int Likes { get; set; }
    public bool UsuarioHaDadoLike { get; set; }

    // almacena localmente los ids de recetas que el usuario ya ha marcado como like
    private HashSet<int> _likesUsuario = new();
    private HashSet<int> _usuariosValorados = new();

    double lastScrollY = 0;
    bool isBarHidden = false;

    public HomePage()
    {
        InitializeComponent();
        BindingContext = this;
        CargarRecetas();
    }

    private async void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        double currentScrollY = e.VerticalOffset;
        if (currentScrollY > lastScrollY + 5 && !isBarHidden)
        {
            isBarHidden = true;
            await BottomBar.TranslateToAsync(0, 80, 250, Easing.CubicIn);
            await TopBar.TranslateToAsync(0, -80, 250, Easing.CubicIn);
        }
        else if (currentScrollY < lastScrollY - 5 && isBarHidden)
        {
            isBarHidden = false;
            await BottomBar.TranslateToAsync(0, 0, 250, Easing.CubicOut);
            await TopBar.TranslateToAsync(0, 0, 250, Easing.CubicOut);
        }
        lastScrollY = currentScrollY;
    }

    private List<Receta> FiltrarPorPreferencias(List<Receta> recetas, Usuario usuario)
    {
        var filtradas = recetas.AsEnumerable();

        if (usuario.Gluten) filtradas = filtradas.Where(r => !r.Gluten);
        if (usuario.Lactosa) filtradas = filtradas.Where(r => !r.Lactosa);
        if (usuario.Huevo) filtradas = filtradas.Where(r => !r.Huevo);
        if (usuario.FrutosSecos) filtradas = filtradas.Where(r => !r.FrutosSecos);
        if (usuario.Marisco) filtradas = filtradas.Where(r => !r.Mariscos);
        if (usuario.Soja) filtradas = filtradas.Where(r => !r.Soja);
        if (usuario.Pescado) filtradas = filtradas.Where(r => !r.Pescado);
        if (usuario.Cacahuetes) filtradas = filtradas.Where(r => !r.Cacahuetes);
        if (usuario.Sesamo) filtradas = filtradas.Where(r => !r.Sesamo);
        if (usuario.Sulfitos) filtradas = filtradas.Where(r => !r.Sulfitos);
        if (usuario.Mostaza) filtradas = filtradas.Where(r => !r.Mostaza);
        if (usuario.Altramuces) filtradas = filtradas.Where(r => !r.Altramuces);
        if (usuario.Moluscos) filtradas = filtradas.Where(r => !r.Moluscos);
        if (usuario.Apio) filtradas = filtradas.Where(r => !r.Apio);

        // Preferencias dietéticas
        if (usuario.Vegano) filtradas = filtradas.Where(r => r.Vegano);
        else if (usuario.Vegetariano) filtradas = filtradas.Where(r => r.Vegetariano);

        return filtradas.ToList();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var texto = e.NewTextValue?.Trim() ?? string.Empty;

        var baseList = RecetasFiltradasBase ?? new List<Receta>();
        var resultado = string.IsNullOrWhiteSpace(texto)
            ? baseList
            : baseList.Where(r => r.Titulo != null && r.Titulo.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();

        // actualizar ObservableCollection de forma eficiente
        Recetas.Clear();
        foreach (var r in resultado)
            Recetas.Add(r);
    }

    private async void CargarRecetas()
    {
        var api = new ApiService();
        int usuarioId = App.UsuarioActual?.Id ?? 0;
        var lista = await api.GetRecetasAsync();

        if (lista == null)
            return;

        TodasLasRecetas = lista.ToList();

        _likesUsuario.Clear();
        _usuariosValorados.Clear();

        if (usuarioId != 0)
        {
            try
            {
                var likesUsuario = await api.GetLikesUsuarioAsync(usuarioId);
                if (likesUsuario != null)
                    _likesUsuario = new HashSet<int>(likesUsuario.Select(l => l.RecetaId));

                var valoracionesHechas = await api.GetValoracionesPorUsuarioAsync(usuarioId);
                if (valoracionesHechas != null)
                    _usuariosValorados = new HashSet<int>(valoracionesHechas.Select(v => v.UsuarioValoradoId));
            }
            catch
            {
                _likesUsuario = new HashSet<int>();
                _usuariosValorados = new HashSet<int>();
            }
        }

        if (App.UsuarioActual == null || (App.UsuarioActual != null))
        {
            RecetasFiltradasBase = TodasLasRecetas.ToList(); // invitado ve todo
        }
        else
        {
            RecetasFiltradasBase = FiltrarPorPreferencias(TodasLasRecetas, App.UsuarioActual);
        }

        Recetas.Clear();

        foreach (var r in lista)
        {
            if (_likesUsuario.Contains(r.Id))
            {
                r.UsuarioHaDadoLike = true;
                r.Likes++;
            }

            if (_usuariosValorados.Contains(r.UsuarioId))
            {
                r.UsuarioHaValorado = true;
            }

            if (!string.IsNullOrEmpty(r.Imagen))
            {
                byte[] bytes = Convert.FromBase64String(r.Imagen);
                r.Imagen = null;
                r.ImagenSource = ImageSource.FromStream(() => new MemoryStream(bytes));
            }

            Recetas.Add(r);
        }
    }

    private async void LikeClicked(object sender, EventArgs e)
    {
        var api = new ApiService();
        var img = (Image)sender;
        var receta = (Receta)img.BindingContext;

        if (App.UsuarioActual == null)
        {
            await DisplayAlertAsync("Debe iniciar sesión", "Inicie sesión para dar like.", "OK");
            return;
        }

        int usuarioId = App.UsuarioActual.Id;
        int recetaId = receta.Id;

        if (!receta.UsuarioHaDadoLike)
        {
            try
            {
                await api.PostLikeAsync(new Like
                {
                    UsuarioId = usuarioId,
                    RecetaId = recetaId
                });

                receta.UsuarioHaDadoLike = true;
                receta.Likes++;
                _likesUsuario.Add(recetaId);
            }
            catch (HttpRequestException)
            {
                // posible conflicto/duplicado: sincronizar desde servidor
                await SafeRefreshLikes(api, usuarioId);
                receta.UsuarioHaDadoLike = _likesUsuario.Contains(recetaId);
            }
            catch
            {
                // error genérico: refrescar como medida defensiva
                await SafeRefreshLikes(api, usuarioId);
                receta.UsuarioHaDadoLike = _likesUsuario.Contains(recetaId);
            }
        }
        else
        {
            try
            {
                await api.DeleteLikeAsync(usuarioId, recetaId);

                receta.UsuarioHaDadoLike = false;
                receta.Likes = Math.Max(0, receta.Likes - 1);
                _likesUsuario.Remove(recetaId);
            }
            catch
            {
                // si falla eliminando, re-sincroniza
                await SafeRefreshLikes(api, usuarioId);
                receta.UsuarioHaDadoLike = _likesUsuario.Contains(recetaId);
            }
        }

        // Refrescar icono y contador
        img.Source = receta.IconoLike;
    }

    private async void PuntuarClicked(object sender, EventArgs e)
    {
        var img = (Image)sender;
        var receta = (Receta)img.BindingContext;

        if (App.UsuarioActual == null)
        {
            await DisplayAlertAsync("Debe iniciar sesión", "Inicie sesión para valorar.", "OK");
            return;
        }

        var popup = new StarRatingPopup(receta.UsuarioId);
        var resultado = await this.ShowPopupAsync(popup);

        if (resultado is int estrellas)
        {
            receta.UsuarioHaValorado = true;
            _usuariosValorados.Add(receta.UsuarioId);
            img.Source = receta.IconoEstrella; // ← opcional, la UI se refresca sola
            await DisplayAlertAsync("Gracias", $"Has valorado con {estrellas} estrellas", "OK");
        }
    }


    // helper para refrescar likes del servidor sin lanzar excepciones visibles
    private async Task SafeRefreshLikes(ApiService api, int usuarioId)
    {
        try
        {
            var likesFromServer = await api.GetLikesUsuarioAsync(usuarioId);
            _likesUsuario = new HashSet<int>(likesFromServer.Select(l => l.RecetaId));
        }
        catch
        {
            // ignoramos errores en refresh para no bloquear la UI
        }
    }

    private async void RecetasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//recipes");
    }

    private async void PerfilClicked(object sender, EventArgs e)
    {
        if (App.UsuarioActual == null)
        {
            bool irLogin = await DisplayAlertAsync(
                "Inicia sesión",
                "Para acceder a tu perfil necesitas iniciar sesión.",
                "Iniciar sesión",
                "Cancelar"
            );

            if (irLogin)
            {
                await Shell.Current.GoToAsync("//login");
            }

            return;
        }
        await Shell.Current.GoToAsync("//profile");
    }
}