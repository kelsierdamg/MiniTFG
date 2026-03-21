namespace MiniTFG;

public partial class NamePage : ContentPage
{
	public NamePage()
	{
		InitializeComponent();
	}

    private void ShowCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (chkShow.IsChecked)
        {
            passwordEntry.IsPassword = false;
            passwordRepeatEntry.IsPassword = false;
        }
        else
        {
            passwordEntry.IsPassword = true;
            passwordRepeatEntry.IsPassword = true;
        }
    }

    public void NextClicked(object sender, EventArgs e)
	{
        if (passwordEntry.Text != passwordRepeatEntry.Text)
        {
            DisplayAlert("Error", "Las contraseñas no coinciden", "OK");
            return;
        }
        else
        {
            Navigation.PushAsync(new AllergiesPage());
        }

        }
}