
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrecedentExpert.Data;
using PrecedentExpert.Models;
using Microsoft.EntityFrameworkCore;

namespace PrecedentExpert.ViewModels
{
    public partial class ObjectViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ObjectInput> userInputs = new();

        [ObservableProperty]
        private ObservableCollection<string> situationParams = new();

        [ObservableProperty]
        private ObservableCollection<string> solutionParams = new();

        [ObservableProperty]
        private ObservableCollection<Precedent> precedents = new();

        [ObservableProperty]
        private ObservableCollection<string> precedentDetails = new();

        private ObjectInput _selectedObject;
        public ObjectInput SelectedObject
        {
            get => _selectedObject;
            set
            {
                SetProperty(ref _selectedObject, value);
                LoadSituationSolutionVariablesCommand.Execute(null);
            }
        }

        private Precedent _selectedPrecedent;
        public Precedent SelectedPrecedent
        {
            get => _selectedPrecedent;
            set
            {
                SetProperty(ref _selectedPrecedent, value);
                if (value != null)
                {
                    LoadPrecedentDetails();
                }
            }
        }

        private readonly AppDbContext _context;

        public IAsyncRelayCommand LoadObjectsCommand { get; }
        public IAsyncRelayCommand SaveObjectCommand { get; }
        public IAsyncRelayCommand DeleteObjectCommand { get; }
        public IAsyncRelayCommand LoadPrecedentsCommand { get; }
        public IAsyncRelayCommand LoadSituationSolutionVariablesCommand { get; }

        public ObjectViewModel(AppDbContext context)
        {
            _context = context;
            LoadObjectsCommand = new AsyncRelayCommand(LoadObjectsAsync);
            SaveObjectCommand = new AsyncRelayCommand(SaveObjectAsync);
            DeleteObjectCommand = new AsyncRelayCommand(DeleteObjectAsync);
            LoadPrecedentsCommand = new AsyncRelayCommand(LoadPrecedentsAsync);
            LoadSituationSolutionVariablesCommand = new AsyncRelayCommand(LoadSituationSolutionVariablesAsync);
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
                Console.WriteLine($"Ошибка: {ex}");
            }
        }

        private async Task<bool> SaveObjectAsync()
        {
            try
            {
                if (SelectedObject != null)
                {
                    var objFromDb = await _context.Objects.FindAsync(SelectedObject.Id);
                    if (objFromDb != null)
                    {
                        objFromDb.Name = SelectedObject.Name;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении объекта: {ex}");
                return false;
            }
        }

        private async Task LoadPrecedentsAsync()
        {
            try
            {
                Precedents.Clear();
                if (SelectedObject != null)
                {
                    var precedentsFromDb = await _context.Precedents
                        .Where(p => p.ObjectId == SelectedObject.Id)
                        .ToListAsync();
                    foreach (var precedent in precedentsFromDb)
                    {
                        Precedents.Add(precedent);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке прецедентов: {ex}");
            }
        }

        private void LoadPrecedentDetails()
        {
            try
            {
                PrecedentDetails.Clear();
                if (SelectedPrecedent != null)
                {
                    var situationParams = SelectedPrecedent.SituationParams;
                    var solutionParams = SelectedPrecedent.SolutionParams;

                    var situationVariables = _context.SituationVariables
                        .Where(sv => sv.ObjectId == SelectedObject.Id)
                        .ToList();

                    for (int i = 0; i < situationParams.Length; i++)
                    {
                        if (i < situationVariables.Count)
                        {
                            PrecedentDetails.Add($"{situationVariables[i].Name}: {situationParams[i]}");
                        }
                    }

                    var solutionVariables = _context.SolutionVariables
                        .Where(sv => sv.ObjectId == SelectedObject.Id)
                        .ToList();

                    for (int i = 0; i < solutionParams.Length; i++)
                    {
                        if (solutionParams[i] == 1 && i < solutionVariables.Count)
                        {
                            PrecedentDetails.Add($"{solutionVariables[i].Name}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке деталей прецедента: {ex}");
            }
        }

        private async Task LoadSituationSolutionVariablesAsync()
        {
            try
            {
                SituationParams.Clear();
                SolutionParams.Clear();
                Precedents.Clear();

                if (SelectedObject != null)
                {
                    var situationVariables = await _context.SituationVariables
                        .Where(sv => sv.ObjectId == SelectedObject.Id)
                        .ToListAsync();
                    foreach (var variable in situationVariables)
                    {
                        SituationParams.Add(variable.Name);
                    }

                    var solutionVariables = await _context.SolutionVariables
                        .Where(sv => sv.ObjectId == SelectedObject.Id)
                        .ToListAsync();
                    foreach (var variable in solutionVariables)
                    {
                        SolutionParams.Add(variable.Name);
                    }

                    var precedents = await _context.Precedents
                        .Where(p => p.ObjectId == SelectedObject.Id)
                        .ToListAsync();
                    foreach (var precedent in precedents)
                    {
                        Precedents.Add(precedent);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex}");
            }
        }

        public IAsyncRelayCommand EditObjectNameCommand => new AsyncRelayCommand(EditObjectNameAsync);
        public IAsyncRelayCommand<SituationVariable> EditSituationNameCommand => new AsyncRelayCommand<SituationVariable>(EditSituationNameAsync);
        public IAsyncRelayCommand<SolutionVariable> EditSolutionNameCommand => new AsyncRelayCommand<SolutionVariable>(EditSolutionNameAsync);

        private async Task EditObjectNameAsync()
        {
            if (SelectedObject != null)
            {
                var newName = await Application.Current.MainPage.DisplayPromptAsync("Редактировать название", "Введите новое название", "Сохранить", "Отмена", SelectedObject.Name);
                if (newName != null)
                {
                    SelectedObject.Name = newName;
                    bool savedSuccessfully = await SaveObjectAsync();
                    if (savedSuccessfully)
                    {
                        await LoadObjectsAsync();
                        await Application.Current.MainPage.DisplayAlert("Успех", "Название успешно изменено", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Попробуйте снова", "OK");
                    }
                }
            }
        }

         private async Task<bool> SaveSituationVariableAsync(SituationVariable situationVariable)
        {
            try
            {
                if (situationVariable != null)
                {
                    var variableFromDb = await _context.SituationVariables.FindAsync(situationVariable.Id);
                    if (variableFromDb != null)
                    {
                        variableFromDb.Name = situationVariable.Name;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении параметра ситуации: {ex}");
                return false;
            }
        }

        private async Task<bool> SaveSolutionVariableAsync(SolutionVariable solutionVariable)
        {
            try
            {
                if (solutionVariable != null)
                {
                    var variableFromDb = await _context.SolutionVariables.FindAsync(solutionVariable.SolutionId);
                    if (variableFromDb != null)
                    {
                        variableFromDb.Name = solutionVariable.Name;
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении параметра решения: {ex}");
                return false;
            }
        }


        private async Task EditSituationNameAsync(SituationVariable situationVariable)
        {
            if (situationVariable != null)
            {
                var newName = await Application.Current.MainPage.DisplayPromptAsync("Редактировать название параметра ситуации", "Введите новое название", "Сохранить", "Отмена", situationVariable.Name);
                if (newName != null)
                {
                    situationVariable.Name = newName;
                    bool savedSuccessfully = await SaveSituationVariableAsync(situationVariable);
                    if (savedSuccessfully)
                    {
                        await LoadSituationSolutionVariablesAsync();
                        await Application.Current.MainPage.DisplayAlert("Успех", "Название параметра ситуации успешно изменено", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Попробуйте снова", "OK");
                    }
                }
            }
        }

        private async Task EditSolutionNameAsync(SolutionVariable solutionVariable)
        {
            if (solutionVariable != null)
            {
                var newName = await Application.Current.MainPage.DisplayPromptAsync("Редактировать название параметра решения", "Введите новое название", "Сохранить", "Отмена", solutionVariable.Name);
                if (newName != null)
                {
                    solutionVariable.Name = newName;
                    bool savedSuccessfully = await SaveSolutionVariableAsync(solutionVariable);
                    if (savedSuccessfully)
                    {
                        await LoadSituationSolutionVariablesAsync();
                        await Application.Current.MainPage.DisplayAlert("Успех", "Название параметра решения успешно изменено", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Попробуйте снова", "OK");
                    }
                }
            }
        }



        private async Task DeleteObjectAsync()
        {
            if (SelectedObject != null)
            {
                var confirmed = await Application.Current.MainPage.DisplayAlert("Подтверждение", $"Вы уверены, что хотите удалить объект '{SelectedObject.Name}'?\nЭто также приведет к удалению всех связанных с ним данных.", "Да", "Отмена");

                if (confirmed)
                {
                    try
                    {
                        var precedentsToDelete = await _context.Precedents.Where(p => p.ObjectId == SelectedObject.Id).ToListAsync();
                        _context.Precedents.RemoveRange(precedentsToDelete);
                        await _context.SaveChangesAsync();

                        var situationVariablesToDelete = await _context.SituationVariables.Where(sv => sv.ObjectId == SelectedObject.Id).ToListAsync();
                        _context.SituationVariables.RemoveRange(situationVariablesToDelete);

                        var solutionVariablesToDelete = await _context.SolutionVariables.Where(sv => sv.ObjectId == SelectedObject.Id).ToListAsync();
                        _context.SolutionVariables.RemoveRange(solutionVariablesToDelete);
                        await _context.SaveChangesAsync();

                        var objectToRemove = await _context.Objects.FindAsync(SelectedObject.Id);
                        if (objectToRemove != null)
                        {
                            _context.Objects.Remove(objectToRemove);
                            await _context.SaveChangesAsync();
                        }
                        await LoadObjectsAsync();
                        await Application.Current.MainPage.DisplayAlert("Успех", "Объект успешно удален", "OK");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при удалении объекта: {ex}");
                        await Application.Current.MainPage.DisplayAlert("Ошибка", "Попробуйте снова", "OK");
                    }
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
