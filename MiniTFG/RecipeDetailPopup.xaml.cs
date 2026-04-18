using CommunityToolkit.Maui.Views;

namespace MiniTFG;

public partial class RecipeDetailPopup : Popup
{
    private readonly int _recetaId;

    public RecipeDetailPopup(Receta receta)
    {
        InitializeComponent();

        _recetaId = receta.Id;
        TituloLabel.Text = receta.Titulo;
        RecetaImage.Source = receta.ImagenSource;
        DescripcionLabel.Text = receta.Descripcion;
    }

    private void OnLetMeCookClicked(object sender, EventArgs e)
    {
        Close(_recetaId);
    }
}
