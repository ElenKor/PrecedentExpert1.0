using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrecedentExpert.Data;
using PrecedentExpert.Views;

namespace PrecedentExpert.ViewModels
{
    public partial class SituationVariantsForObjectViewModel : ObservableObject
    {
        private readonly AppDbContext _context;

        [ObservableProperty]
        private ObservableCollection<ObjSituationVariable> situationVariableNames = new ObservableCollection<ObjSituationVariable>();

        [ObservableProperty]
        private string newObjectName;

        public ICommand AddSituationVariableCommand { get; }
        public ICommand PrepareForNextStepCommand { get; }

        public SituationVariantsForObjectViewModel(AppDbContext context)
        {
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            AddSituationVariableCommand = new RelayCommand(AddSituationVariable);
            PrepareForNextStepCommand = new AsyncRelayCommand(PrepareForNextStep);
        }
        public void OnNameTextChanged(ObjSituationVariable variable, string newName)
        {
            if (variable != null)
            {
                variable.Name = newName;
            }
        }
        private void AddSituationVariable()
        {
            SituationVariableNames.Add(new ObjSituationVariable());
        }

        public void AddObject(ObjectInput input)
        {
            NewObjectName = input.Name;
        }

        public async Task PrepareForNextStep()
        {
            var transitionData = new TransitionData
            {
                ObjectName = NewObjectName,
                SituationVariableNames = SituationVariableNames.Select(v => v.Name).ToList()
            };

            var solutionVariantsForObjectViewModel = MauiProgram.Services.GetRequiredService<SolutionVariantsForObjectViewModel>();
            solutionVariantsForObjectViewModel.AddTransitionData(transitionData);
            await Application.Current.MainPage.Navigation.PushAsync(new AddSolutionVariantsForObjectView(solutionVariantsForObjectViewModel));
        }
    }

    public class TransitionData
    {
        public string ObjectName { get; set; }
        public List<string> SituationVariableNames { get; set; } = new List<string>();
    }

    public class ObjSituationVariable : ObservableObject
    {
        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
    }
}
