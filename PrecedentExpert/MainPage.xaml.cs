using Microsoft.Maui.Controls;
using PrecedentExpert.Views;
using PrecedentExpert.ViewModels;


namespace PrecedentExpert;

public partial class MainPage : ContentPage
{
	private readonly INavigation navigation;
	int count = 0;
	public ObjectsViewModel _objectsViewModel = MauiProgram.Services.GetRequiredService<ObjectsViewModel>();
	public PrecedentSearchViewModel _precedentSearchView = MauiProgram.Services.GetRequiredService<PrecedentSearchViewModel>();

	public SelectObjectViewModel _selectObjectViewModel= MauiProgram.Services.GetRequiredService<SelectObjectViewModel>();
	public ObjectViewModel _objectViewModel = MauiProgram.Services.GetRequiredService<ObjectViewModel>();



	public MainPage()
	{
		InitializeComponent();
		// Проверяем, зарегистрирован ли сервис SituationVariablesViewModel
    	if (MauiProgram.Services.GetService(typeof(SituationVariablesViewModel)) == null)
    {
        // Если сервис не зарегистрирован, выводим сообщение об ошибке
        Console.WriteLine("Ошибка: сервис SituationVariablesViewModel не зарегистрирован.");
        return;
    }
	}

	private async void OnAddObjectClicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим на другую страницу
        await Navigation.PushAsync(new AddObjectView(_objectsViewModel));
	}

	private async void OnAddPrecedentClicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим на другую страницу
        await Navigation.PushAsync(new AddPrecedentForObjectView(_selectObjectViewModel));
	}

	private async void OnFindSolutionClicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим на другую страницу
        await Navigation.PushAsync(new SelectObjectView(_selectObjectViewModel));
	}
	private async void OnViewDbPrecedentClicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим на другую страницу
        await Navigation.PushAsync(new ObjectView(_objectViewModel));
	}
	private async void OnAboutAppClicked(object sender, EventArgs e)
	{
		// Здесь откройте модальное окно или новую страницу с описанием приложения
		string description = "ПО предназначено для поиска решений по устранению отказов в технических системах методом прецедентов";
		await Application.Current.MainPage.DisplayAlert("О приложении", description, "OK");
	}
	
}

