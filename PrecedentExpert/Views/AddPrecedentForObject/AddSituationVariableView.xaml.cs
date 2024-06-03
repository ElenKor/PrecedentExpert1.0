using PrecedentExpert.ViewModels;

namespace PrecedentExpert.Views;

public partial class AddSituationVariableView : ContentPage
{
	private readonly SituationVariablesViewModel _situationVariablesViewModel = MauiProgram.Services.GetRequiredService<SituationVariablesViewModel>();
    private SituationVariantsForObjectViewModel situationVariantsForObjectViewModel;

    public AddSituationVariableView(SituationVariablesViewModel situationVariablesViewModel)
	{
		InitializeComponent();
		_situationVariablesViewModel = situationVariablesViewModel;
        BindingContext = _situationVariablesViewModel;
	}

    public AddSituationVariableView(SituationVariantsForObjectViewModel situationVariantsForObjectViewModel)
    {
        this.situationVariantsForObjectViewModel = situationVariantsForObjectViewModel;
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