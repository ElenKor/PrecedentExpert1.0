using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrecedentExpert.Data;
using PrecedentExpert.Models;
using PrecedentExpert.Views;
using Microsoft.EntityFrameworkCore;

namespace PrecedentExpert.ViewModels
{
    public class ObjectInput
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static implicit operator ObjectInput?(string? v)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ObjectsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<ObjectInput> userInputs = new();

        private readonly AppDbContext _context;

        public IAsyncRelayCommand LoadObjectsCommand { get; }

        [ObservableProperty]
        private string newObjectName;

        public ObjectsViewModel(AppDbContext context)
        {
            _context = context;
            LoadObjectsCommand = new AsyncRelayCommand(LoadObjectsAsync);
        }

        public async Task LoadObjectsAsync()
        {
            var objectsFromDb = await _context.Objects.ToListAsync();
            foreach (var obj in objectsFromDb)
            {
                UserInputs.Add(new ObjectInput
                {
                    Id = obj.Id,
                    Name = obj.Name
                });
            }
        }

          [RelayCommand]
        public async Task AddObjectAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewObjectName))
            {
                var newObjectInput = new ObjectInput
                {
                    Name = NewObjectName
                };

                UserInputs.Add(newObjectInput);

                // Очищаем имя нового объекта после добавления
                NewObjectName = string.Empty;

                // Передаем UserInputs в SituationVariantsForObjectViewModel
                var situationVariantsForObjectViewModel = MauiProgram.Services.GetRequiredService<SituationVariantsForObjectViewModel>();
                situationVariantsForObjectViewModel.AddObject(newObjectInput);

                // Переходим на новую страницу
                await Application.Current.MainPage.Navigation.PushAsync(new AddSituationVariantsForObjectView(situationVariantsForObjectViewModel));
            }
        }
    }
}


