using PrecedentExpert.ViewModels;

namespace PrecedentExpert.Views;

public partial class SelectObjectView : ContentPage
{
	private readonly SelectObjectViewModel _objectViewModel = MauiProgram.Services.GetRequiredService<SelectObjectViewModel>();


	public SelectObjectView(SelectObjectViewModel objectViewModel)
	{
		InitializeComponent();
		_objectViewModel = objectViewModel;
        BindingContext = _objectViewModel;
		// Загрузка объектов при создании страницы
        _objectViewModel.LoadObjectsCommand.Execute(null);
	}

    private async void OnBackBtnlicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим обратно на предыдушую страницу
        await Navigation.PopAsync();
	}

}