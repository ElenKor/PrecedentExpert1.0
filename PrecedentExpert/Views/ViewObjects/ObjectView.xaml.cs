// using PrecedentExpert.ViewModels;

// namespace PrecedentExpert.Views;

// public partial class ObjectView : ContentPage
// {
// 	private readonly ObjectViewModel _objectViewModel = MauiProgram.Services.GetRequiredService<ObjectViewModel>();


// 	public ObjectView(ObjectViewModel objectViewModel)
// 	{
// 		InitializeComponent();
// 		_objectViewModel = objectViewModel;
//         BindingContext = _objectViewModel;
// 		// Загрузка объектов при создании страницы
//         _objectViewModel.LoadObjectsCommand.Execute(null);
// 	}
//     private async void OnBackBtnlicked(object sender, EventArgs e)
// 	{
// 		// При нажатии на кнопку переходим обратно на главную страницу
//         await Navigation.PushAsync(new MainPage());
// 	}

// }

using PrecedentExpert.ViewModels;

namespace PrecedentExpert.Views;

public partial class ObjectView : ContentPage
{
	private readonly ObjectViewModel _objectViewModel = MauiProgram.Services.GetRequiredService<ObjectViewModel>();
	
    public ObjectView(ObjectViewModel objectViewModel)
    {
        InitializeComponent();
        BindingContext = objectViewModel;
		_objectViewModel = objectViewModel;
        // Загрузка объектов при создании страницы
        objectViewModel.LoadObjectsCommand.Execute(null);
    }

    private async void OnBackBtnClicked(object sender, EventArgs e)
    {
        // При нажатии на кнопку переходим обратно на главную страницу
        await Navigation.PushAsync(new MainPage());
    }
}
