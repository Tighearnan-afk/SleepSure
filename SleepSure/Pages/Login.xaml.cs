namespace SleepSure.Pages;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();

        btnLogin.Clicked += OnbtnLogin_Clicked;
	}

    private void OnbtnLogin_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new AppShell();
    }
}