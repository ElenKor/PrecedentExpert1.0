namespace PrecedentExpert.Views;

using PrecedentExpert.ViewModels;

public partial class PrecedentSearchView : ContentPage
{
	private readonly PrecedentSearchViewModel _precedentSearchViewModel = MauiProgram.Services.GetRequiredService<PrecedentSearchViewModel>();


	public PrecedentSearchView(PrecedentSearchViewModel PrecedentSearchViewModel)
	{
		InitializeComponent();
		_precedentSearchViewModel = PrecedentSearchViewModel;
        BindingContext = _precedentSearchViewModel;
	}
	
	private async void OnBackBtnlicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим обратно на главную страницу
    	await Navigation.PopAsync();
	}
	  private async void OnCancelBtnlicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим на главную страницу
        await Navigation.PushAsync(new MainPage());
	}


}

