using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrecedentExpert.Data;
using PrecedentExpert.Models;
using Microsoft.EntityFrameworkCore;
using PrecedentExpert.Views;
using PrecedentExpert;
using PrecedentExpert.ViewModels;


namespace PrecedentExpert.ViewModels
{
    public class SituationVariableInput
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Value { get; set; }
    }
    public class FinalTransitionData
    {
        public int ObjectId { get; set; }
        public List<int> SituationVariablesValues { get; set; }
    }
}
public class SituationVariablesViewModel : ObservableObject, IDisposable, INotifyPropertyChanged
{
    private readonly AppDbContext _context;

    private string? _newObjectName;
    private int _newObjectId;

    public SituationVariablesViewModel(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context)); // Проверяем, что context не null
        LoadSituationVariables(_newObjectId); // Загружаем начальные данные при создании
    }

    private int _value;
    public int Value
    {
        get => _value;
        set => SetProperty(ref _value, value);
    }

    public ObservableCollection<SituationVariable> SituationVariables { get; } = new ObservableCollection<SituationVariable>();

    private SituationVariable _selectedSituationVariable;

    public SituationVariable SelectedSituationVariable
    {
        get => _selectedSituationVariable;
        set => SetProperty(ref _selectedSituationVariable, value);
    }

    public int NewSituationVariableId { get; set; }
    public string NewSituationVariableName { get; set; }
    public double NewSituationVariableValue { get; set; }

    private ICommand _addSituationVariableCommand;

    public ICommand AddSituationVariableCommand => _addSituationVariableCommand ??= new RelayCommand(() => AddSituationVariable());

    private ObservableCollection<SituationVariableInput> _userInputs = new ObservableCollection<SituationVariableInput>();
    public ObservableCollection<SituationVariableInput> UserInputs
    {
        get => _userInputs;
        set => SetProperty(ref _userInputs, value);
    }

    public void AddObject(ObjectInput userInput)
    {
        {
            // Присваиваем значения новому объекту и ID
            _newObjectName = userInput.Name;
            _newObjectId = userInput.Id;

            // Создаем новый экземпляр SituationVariableInput
            var newSituationVariable = new SituationVariableInput
            {
                Id = UserInputs.Count + 1,
                Name = _newObjectName, // Используем имя объекта
            };
            // Добавляем новый объект в коллекцию UserInputs
            UserInputs.Add(newSituationVariable);
            // Загружаем переменные ситуации, соответствующие выбранному объекту
            LoadSituationVariables(_newObjectId);
        }
    }

    private async void AddSituationVariable()
    {
        FinalTransitionData transitionData = new FinalTransitionData
        {
            ObjectId = _newObjectId,
            SituationVariablesValues = UserInputs.Select(input => input.Value).ToList()
        };

        var solutionVariablesViewModel = MauiProgram.Services.GetRequiredService<SolutionVariablesViewModel>();
        solutionVariablesViewModel.AddSituationVariables(transitionData);
        await Application.Current.MainPage.Navigation.PushAsync(new AddSolutionVariableView(solutionVariablesViewModel));
    }

    private async void LoadSituationVariables(int selectedObjectId)
    {
        UserInputs.Clear(); // Очищаем коллекцию перед загрузкой новых данных

        var situationVariables = await _context.SituationVariables
            .Where(sv => sv.ObjectId == selectedObjectId)
            .ToListAsync();

        foreach (var variable in situationVariables)
        {
            UserInputs.Add(new SituationVariableInput
            {
                Id = variable.Id,
                Name = variable.Name,
                Value = 0 // Значение по умолчанию или загрузить сохраненные значения, если они есть
            });
        }
    }
    public void Dispose()
    {
        _context.Dispose(); // Уничтожаем контекст при уничтожении ViewModel
    }
}
