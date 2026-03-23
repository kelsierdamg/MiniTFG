namespace MiniTFG;

public partial class HomePage : ContentPage
{
    double lastScrollY = 0;
    bool isBarHidden = false;

    public HomePage()
	{
		InitializeComponent();
    }

    private async void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        double currentScrollY = e.VerticalOffset;
        if (currentScrollY > lastScrollY +5 && !isBarHidden) // +5 to add a small threshold before hiding the bar
        {
            isBarHidden = true;
            await BottomBar.TranslateToAsync(0, 80, 250, Easing.CubicIn); // x, y, duration, easing
            await TopBar.TranslateToAsync(0, -80, 250, Easing.CubicIn); // x, y, duration, easing
        }
        else if (currentScrollY < lastScrollY -5 && isBarHidden) // -5 to add a small threshold before showing the bar again
        {
            isBarHidden = false;
            await BottomBar.TranslateToAsync(0, 0, 250, Easing.CubicOut); // x, y, duration, easing
            await TopBar.TranslateToAsync(0, 0, 250, Easing.CubicOut); // x, y, duration, easing
        }
        lastScrollY = currentScrollY;
    }

    private async void RecetasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//recipes");
    }
}