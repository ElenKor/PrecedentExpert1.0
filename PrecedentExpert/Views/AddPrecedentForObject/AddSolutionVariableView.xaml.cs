namespace PrecedentExpert.Views;

using PrecedentExpert.ViewModels;

public partial class AddSolutionVariableView : ContentPage
{
	private readonly SolutionVariablesViewModel _solutionVariablesViewModel = MauiProgram.Services.GetRequiredService<SolutionVariablesViewModel>();

	public AddSolutionVariableView(SolutionVariablesViewModel solutionVariablesViewModel)
	{
		InitializeComponent();
		_solutionVariablesViewModel = solutionVariablesViewModel;
        BindingContext = _solutionVariablesViewModel;
	}

    private async void OnBackBtnlicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим обратно на предыдущую страницу
    	await Navigation.PopAsync();
	}
	  private async void OnCancelBtnlicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим на главную страницу
        await Navigation.PushAsync(new MainPage());
	}

}