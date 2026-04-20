using CommunityToolkit.Maui.Views;

namespace MiniTFG;

public partial class RecipeDetailPopup : Popup
{
    private readonly int _recetaId;
    private readonly int _usuarioId;

    public RecipeDetailPopup(Receta receta)
    {
        InitializeComponent();

        _recetaId = receta.Id;
        _usuarioId = receta.UsuarioId;
        TituloLabel.Text = receta.Titulo;
        RecetaImage.Source = receta.ImagenSource;
        DescripcionLabel.Text = receta.Descripcion;
    }

    private void OnLetMeCookClicked(object sender, EventArgs e)
    {
        Close(new int[] { _recetaId, _usuarioId });
    }
}
