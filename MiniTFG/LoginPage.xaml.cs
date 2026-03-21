namespace MiniTFG;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private void GuestClicked(object sender, EventArgs e)
	{
		Application.Current.MainPage = new NavigationPage(new HomePage());
    }

    private void CreateClicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new NamePage());
    }

	private void ShowCheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		if (chkShow.IsChecked)
		{
			passwordEntry.IsPassword = false;
		}
		else
		{
			passwordEntry.IsPassword = true;
		}
	}

	private void RememberCheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		if (chkRemember.IsChecked)
		{
			// Implement logic to remember the user's credentials
		}
		else
		{
			// Implement logic to forget the user's credentials
		}
    }
}