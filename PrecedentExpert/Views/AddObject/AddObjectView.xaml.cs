using System.Security.Cryptography.X509Certificates;
using PrecedentExpert.ViewModels;

namespace PrecedentExpert.Views;

public partial class AddObjectView : ContentPage
{
	public readonly ObjectsViewModel _objectViewModel = MauiProgram.Services.GetRequiredService<ObjectsViewModel>();


	public AddObjectView(ObjectsViewModel objectViewModel)
	{
		InitializeComponent();
		_objectViewModel = objectViewModel;
        BindingContext = _objectViewModel;
	}
    private async void OnBackBtnlicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим обратно на главную страницу
        await Navigation.PushAsync(new MainPage());
	}
}