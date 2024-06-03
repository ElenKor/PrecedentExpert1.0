using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrecedentExpert.Data;
using PrecedentExpert.Models;
using Microsoft.EntityFrameworkCore;

namespace PrecedentExpert.ViewModels
{
       public class SolutionVariableInput
    {
        public int Id {get; set; }
        public string? Name { get; set; }
        public int Value { get; set; }
        public int[] Values { get; set; } //массив значений параметров ситуации  
    }
    public class SolutionVariablesViewModel : ObservableObject, IDisposable, INotifyPropertyChanged
    {
        private readonly AppDbContext _context;
        private int[] _newSituationVariableParams;

        private ObservableCollection<SolutionVariableInput> _userInputs = new ObservableCollection<SolutionVariableInput>();


         public ObservableCollection<SolutionVariableInput> UserInputs
            {
                get => _userInputs;
                set => SetProperty(ref _userInputs, value);
            }
        private int _newObjectId;


        public SolutionVariablesViewModel(AppDbContext context)
        {
            // Проверяем, что context не null
            _context = context ?? throw new ArgumentNullException(nameof(context)); 
        }

        public ObservableCollection<SolutionVariable> SolutionVariables { get; } = new ObservableCollection<SolutionVariable>();

        private SolutionVariable _selectedSolutionVariable;

        public SolutionVariable SelectedSolutionVariable
        {
            get => _selectedSolutionVariable;
            set => SetProperty(ref _selectedSolutionVariable, value);
        }

        public int NewSolutionVariableId { get; set; }
        public string NewSolutionVariableName { get; set; }
        public string NewSolutionVariableValue { get; set; }

        private ICommand _addSolutionVariableCommand;
        public ICommand AddSolutionVariableCommand => _addSolutionVariableCommand ??= new RelayCommand(() => AddSolutionVariable(UserInputs));
        
        public void AddSituationVariables(FinalTransitionData transitionData)
        {
            try
            {
                // Проверяем, что значение ObjectId не null и больше нуля
                if (transitionData.ObjectId <= 0)
                {
                    throw new ArgumentException("Object ID is invalid.");
                }

                _newObjectId = transitionData.ObjectId; // Предполагается, что это ID уже назначен

                // Проверяем, что значения в массиве SituationVariables не null
                if (transitionData.SituationVariablesValues == null)
                {
                    throw new ArgumentException("SituationVariables array is null.");
                }
                // Инициализируем массив _newSituationVariableParams значениями 0
                _newSituationVariableParams = transitionData.SituationVariablesValues.ToArray();
            
                // Загружаем переменные ситуации
                LoadSolutionVariables(_newObjectId);
            }
            catch (Exception ex)
            {
                // Выводим сообщение об ошибке
                Console.WriteLine($"Error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Ошибка", $"{ex.Message}", "OK");
            }
        }
        private async void AddSolutionVariable(ObservableCollection<SolutionVariableInput> userInputs)
        {
            try{
                var sotutionVariablesInput = new SolutionVariableInput
                {
                    Values = UserInputs.Select(input => input.Value).ToArray()
                };
             var precedent = new Precedent
            {
                ObjectId = _newObjectId,
                SituationParams = _newSituationVariableParams,
                SolutionParams = sotutionVariablesInput.Values
            };
            _context.Precedents.Add(precedent);
            await _context.SaveChangesAsync(); // Асинхронно сохраняем изменения в базе данных

            await Application.Current.MainPage.DisplayAlert("Успех", "Все данные успешно сохранены", "OK");
            await Application.Current.MainPage.Navigation.PushAsync(new MainPage());
            }
            catch (Exception ex)
            {
                // Получаем внутреннее исключение
                Exception innerException = ex.InnerException;
                // Если есть внутреннее исключение, выводим его сообщение
                string errorMessage = (innerException != null) ? innerException.Message : ex.Message;
                // Выводим сообщение об ошибке с подробностями
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Ошибка сохранения данных: {errorMessage}", "OK");
            }
        }
       private async void LoadSolutionVariables(int selectObjectId)
        {
            UserInputs.Clear(); // Очищаем коллекцию перед загрузкой новых данных
            try
            {
                var solutionVariables = await _context.SolutionVariables
                .Where(sv => sv.ObjectId == selectObjectId) // Выполняем фильтрацию по object_id
                .ToListAsync(); // Асинхронно загружаем переменные из базы данных 
                
                 foreach (var variable in solutionVariables)
                {
                    UserInputs.Add(new SolutionVariableInput
                    {
                        Id = variable.SolutionId,
                        Name = variable.Name,
                        Value = 0 // Значение по умолчанию или загрузить сохраненные значения, если они есть
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Ошибка загрузки данных: {ex.Message}", "OK");
            }
        }

        public void Dispose()
        {
            _context.Dispose(); // Уничтожаем контекст при уничтожении ViewModel
        }
    }
}