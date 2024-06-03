using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrecedentExpert.Data;
using PrecedentExpert.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace PrecedentExpert.ViewModels
{
    public partial class SelectObjectViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ObjectInput> userInputs = new();
        private ObjectInput _selectedObject;
        public ObjectInput SelectedObject
        {
            get => _selectedObject;
            set => SetProperty(ref _selectedObject, value);
        }
        private readonly AppDbContext _context;

        public IAsyncRelayCommand LoadObjectsCommand { get; }

        [ObservableProperty]
        private string newObjectName;

        public SelectObjectViewModel(AppDbContext context, ILogger<SelectObjectViewModel> logger)
        {
            _context = context;
            LoadObjectsCommand = new AsyncRelayCommand(LoadObjectsAsync);
            _selectedObject = null;
        }

    
       public async Task LoadObjectsAsync()
        {
            try
            {
                UserInputs.Clear();
                var objectsFromDb = await _context.Objects.ToListAsync();
                foreach (var obj in objectsFromDb)
                {
                    UserInputs.Add(new ObjectInput { Id = obj.Id, Name = obj.Name });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.ToString()}");
            }
        }

    private IAsyncRelayCommand _addObjectCommand;
    public IAsyncRelayCommand AddObjectCommand => _addObjectCommand ??= new AsyncRelayCommand(AddObjectAsync);

        public async Task AddObjectAsync()
        {
            if (SelectedObject != null)
            {
                // Передача выбранного объекта в следующий ViewModel
                var situationVariablesViewModel = MauiProgram.Services.GetRequiredService<SituationVariablesViewModel>();
                
                situationVariablesViewModel.AddObject(new ObjectInput { Id = SelectedObject.Id, Name = SelectedObject.Name });

                // Переход на новую страницу
                await Application.Current.MainPage.Navigation.PushAsync(new AddSituationVariableView(situationVariablesViewModel));
            }
            else
            {
                // Если объект не выбран, отображаем сообщение об ошибке
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Список введенных значений параметров пуст. Пожалуйста, повторите попытку", "OK");
            }
        }

        private IAsyncRelayCommand _selectObjectCommand;
        public IAsyncRelayCommand SelectObjectCommand => _selectObjectCommand ??= new AsyncRelayCommand(SelectObjectAsync);

        // Команда в вашем ViewModel, который управляет выбором объектов
        public async Task SelectObjectAsync()
        {
            if (SelectedObject != null)
            {
                // Создание нового экземпляра PrecedentSearchViewModel и инициализация его с идентификатором выбранного объекта.
                var precedentSearchViewModel = MauiProgram.Services.GetRequiredService<PrecedentSearchViewModel>();
                precedentSearchViewModel.SelectObject(SelectedObject.Id);

                // Передача ViewModel в PrecedentSearchView.
                var precedentSearchView = new PrecedentSearchView(precedentSearchViewModel);

                // Переход на страницу поиска прецедентов.
                await Application.Current.MainPage.Navigation.PushAsync(precedentSearchView);
            }
            else
            {
                // Если объект не выбран, отображаем сообщение об ошибке.
                await Application.Current.MainPage.DisplayAlert("Ошибка", "Объект не выбран.", "OK");
            }
        }


     }
 }
