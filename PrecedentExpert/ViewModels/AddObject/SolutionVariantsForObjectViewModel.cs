using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrecedentExpert.Data;
using PrecedentExpert.Models;

namespace PrecedentExpert.ViewModels
{
    public partial class SolutionVariantsForObjectViewModel : ObservableObject
    {
        private ObservableCollection<ObjSolutionVariable> _solutionNames = new();
        private readonly AppDbContext _context;
        private TransitionData _transitionData;

        public ObservableCollection<ObjSolutionVariable> SolutionNames
        {
            get => _solutionNames;
            set => SetProperty(ref _solutionNames, value);
        }

        public SolutionVariantsForObjectViewModel(AppDbContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            AddSolutionVariableCommand = new AsyncRelayCommand(SaveAllDataAsync);
        }

        [ObservableProperty]
        private IAsyncRelayCommand addSolutionVariableCommand;

        public void AddTransitionData(TransitionData data)
        {
            _transitionData = data;
        }

        private async Task SaveAllDataAsync()
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var newObject = new Objects { Name = _transitionData.ObjectName };
                    _context.Objects.Add(newObject);
                    await _context.SaveChangesAsync();

                    foreach (var name in _transitionData.SituationVariableNames)
                    {
                        var situationVariable = new SituationVariable
                        {
                            Name = name,
                            ObjectId = newObject.Id
                        };
                        _context.SituationVariables.Add(situationVariable);
                    }

                    foreach (var solutionVariable in SolutionNames)
                    {
                        var solution = new SolutionVariable
                        {
                            Name = solutionVariable.Name,
                            ObjectId = newObject.Id
                        };
                        _context.SolutionVariables.Add(solution);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await Application.Current.MainPage.DisplayAlert("Успех", "Все данные успешно сохранены", "OK");
                    await Application.Current.MainPage.Navigation.PushAsync(new MainPage());
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    await Application.Current.MainPage.DisplayAlert("Ошибка", "Произошла ошибка при сохранении данных: " + ex.Message, "OK");
                }
            }
        }
    }

    public class ObjSolutionVariable : ObservableObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
