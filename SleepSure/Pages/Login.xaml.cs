namespace SleepSure.Pages;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();

        btnLogin.Clicked += OnbtnLogin_Clicked;
	}

    private void OnbtnLogin_Clicked(object? sender, EventArgs e) //Object uses the null condition operator to verify that the sender object is not null before proceeding, prevents code CS8622 
    {
        Application.Current.MainPage = new AppShell(); /* Retrieved from Stackoverflow "https://stackoverflow.com/questions/75935306/shell-navigation-in-net-maui"
                                                          Replaces the current main page with the appshell, allowing for further navigation to be handled by that */
    }
}