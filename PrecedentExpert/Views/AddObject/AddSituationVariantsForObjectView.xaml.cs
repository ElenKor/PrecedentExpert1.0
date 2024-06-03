using PrecedentExpert.ViewModels;

namespace PrecedentExpert.Views;

public partial class AddSituationVariantsForObjectView : ContentPage
{
    private readonly SituationVariantsForObjectViewModel _situationVariantsForObjectViewModel;

    public AddSituationVariantsForObjectView(SituationVariantsForObjectViewModel situationVariantsForObjectViewModel)
    {
        InitializeComponent();
        _situationVariantsForObjectViewModel = situationVariantsForObjectViewModel;
        BindingContext = _situationVariantsForObjectViewModel;
    }

    private async void OnBackBtnlicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnParameterCountChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int count) && count > 0)
        {
            // Увеличение размера коллекции, если указанное количество больше текущего
            while (_situationVariantsForObjectViewModel.SituationVariableNames.Count < count)
            {
                _situationVariantsForObjectViewModel.SituationVariableNames.Add(new ObjSituationVariable { Name = string.Empty });
            }
            // Уменьшение размера коллекции, если указанное количество меньше текущего
            while (_situationVariantsForObjectViewModel.SituationVariableNames.Count > count)
            {
                _situationVariantsForObjectViewModel.SituationVariableNames.RemoveAt(_situationVariantsForObjectViewModel.SituationVariableNames.Count - 1);
            }
        }
        else
        {
            // Очистка коллекции, если введенное значение не является положительным числом
            _situationVariantsForObjectViewModel.SituationVariableNames.Clear();
        }
    }

    private void OnSituationVariableTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry && entry.BindingContext is ObjSituationVariable variable)
        {
            _situationVariantsForObjectViewModel.OnNameTextChanged(variable, e.NewTextValue);
        }
    }
      private async void OnCancelBtnlicked(object sender, EventArgs e)
	{
		// При нажатии на кнопку переходим на главную страницу
        await Navigation.PushAsync(new MainPage());
	}
}
