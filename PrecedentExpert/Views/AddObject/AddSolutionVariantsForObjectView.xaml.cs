using PrecedentExpert.ViewModels;

namespace PrecedentExpert.Views
{
    public partial class AddSolutionVariantsForObjectView : ContentPage
    {
        public AddSolutionVariantsForObjectView(SolutionVariantsForObjectViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnBackBtnClicked(object sender, EventArgs e)
        {
            // Навигация обратно к предыдущей странице
            await Navigation.PopAsync();
        }

        private void OnSolutionCountChanged(object sender, TextChangedEventArgs e)
        {
            if (!(BindingContext is SolutionVariantsForObjectViewModel viewModel)) return;

            if (int.TryParse(e.NewTextValue, out int count) && count > 0)
            {
                while (viewModel.SolutionNames.Count < count)
                {
                    viewModel.SolutionNames.Add(new ObjSolutionVariable { Name = string.Empty });
                }

                while (viewModel.SolutionNames.Count > count)
                {
                    viewModel.SolutionNames.RemoveAt(viewModel.SolutionNames.Count - 1);
                }
            }
            else
            {
                viewModel.SolutionNames.Clear();
            }
        }
          private async void OnCancelBtnlicked(object sender, EventArgs e)
        {
            // При нажатии на кнопку переходим на главную страницу
            await Navigation.PushAsync(new MainPage());
        }
    }
}
