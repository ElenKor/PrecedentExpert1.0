
using System.Collections.ObjectModel;
using PrecedentExpert.Models;
using PrecedentExpert.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace PrecedentExpert.ViewModels
{
    public enum DistanceMetric
    {
        Euclidean,
        Hamming,
        Cosine,
        Manhattan
    }
    public class AnalysisResult
    {
        public int PrecedentNumber { get; set; }
        public double Distance { get; set; }
    }

    public partial class PrecedentSearchViewModel : ObservableObject
    {
        private List<List<AnalysisResult>> analysisResults;

        [ObservableProperty]
        private ObservableCollection<SituationVariableInput> userInputs = new();

        [ObservableProperty]
        private ObservableCollection<Precedent> precedents = new();
        private readonly AppDbContext _context;

        public IAsyncRelayCommand FindSolutionCommand { get; }
        private int _selectedObjectId;

        private string _selectedMetric;
        public string SelectedMetric
        {
            get => _selectedMetric;
            set
            {
                SetProperty(ref _selectedMetric, value);
            }
        }
        public PrecedentSearchViewModel(AppDbContext context)
        {
            _context = context;
            FindSolutionCommand = new AsyncRelayCommand(ExecuteFindSolutionCommand);
            Metrics = new List<string>
            {
                "Евклидово расстояние",
                "Хеммингово расстояние",
                "Косинусное расстояние",
                "Манхэттенское расстояние"
            };
        }
         [ObservableProperty]
        List<string> metrics;

        // Метод для преобразования строки в DistanceMetric
        private DistanceMetric ConvertStringToMetric(string metricName)
        {
            switch (metricName)
            {
                case "Евклидово расстояние":
                    return DistanceMetric.Euclidean;
                case "Хеммингово расстояние":
                    return DistanceMetric.Hamming;
                case "Косинусное расстояние":
                    return DistanceMetric.Cosine;
                case "Манхэттенское расстояние":
                    return DistanceMetric.Manhattan;
                default:
                    throw new NotSupportedException($"Unknown metric: {metricName}");
            }
        }
        public async Task InitializeAsync()
        {
            await LoadSituationVariablesAsync(_selectedObjectId);
            await LoadPrecedentsAsync(_selectedObjectId);
        }
        private async Task LoadSituationVariablesAsync(int selectedObjectId)
        {
            UserInputs.Clear();
            var situationVariables = await _context.SituationVariables
                                                   .Where(variable => variable.ObjectId == selectedObjectId)
                                                   .ToListAsync();

            foreach (var variable in situationVariables)
            {
                UserInputs.Add(new SituationVariableInput
                {
                    Id = variable.Id,
                    Name = variable.Name
                });
            }
        }
        private async Task LoadPrecedentsAsync(int selectedObjectId)
        {
            Precedents.Clear();
            var loadedPrecedents = await _context.Precedents
                                                 .Where(p => p.ObjectId == selectedObjectId)
                                                 .Select(p => new { p.Id, p.SolutionParams, p.SituationParams })
                                                 .ToListAsync();

            foreach (var item in loadedPrecedents)
            {
                Precedents.Add(new Precedent
                {
                    Id = item.Id,
                    SolutionParams = item.SolutionParams,
                    SituationParams = item.SituationParams 
                });
            }
        }

        public List <int >allSelectedSolutionsIndexes;
     
    private async Task ExecuteFindSolutionCommand()
    {
        int[] situationParams = UserInputs.Select(ui => ui.Value).ToArray();
        var (solutionsParamsList, analysisResults) = FindSolutions(situationParams, SelectedMetric);

        allSelectedSolutionsIndexes = solutionsParamsList
            .SelectMany(solutionParams => solutionParams
                .Select((value, index) => new { Value = value, Index = index })
                .Where(x => x.Value == 1)
                .Select(x => x.Index + 1))
            .Distinct()
            .ToList();

        if (!allSelectedSolutionsIndexes.Any())
        {
            await Application.Current.MainPage.DisplayAlert("Ошибка", "Список решений не найден в Базе данных для данного объекта", "Ok");
            return;
        }

        var solutionsForSelectedObject = await _context.SolutionVariables
            .Where(sv => sv.ObjectId == _selectedObjectId)
            .OrderBy(sv => sv.SolutionId)
            .ToListAsync();

        var solutionNames = allSelectedSolutionsIndexes
            .Where(index => index <= solutionsForSelectedObject.Count)
            .Select(index => solutionsForSelectedObject.ElementAtOrDefault(index - 1)?.Name)
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();

        if (solutionNames.Any())
        {
            // Группируем решения по наборам параметров
            var groupedSolutions = solutionsParamsList
                .Select((solutionParams, index) => new
                {
                    Params = solutionParams,
                    Names = solutionParams
                        .Select((value, paramIndex) => new { Value = value, Index = paramIndex })
                        .Where(x => x.Value == 1)
                        .Select(x => solutionsForSelectedObject.ElementAtOrDefault(x.Index)?.Name)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList()
                })
                .ToList();

            // Форматируем список решений
            var formattedSolutions = groupedSolutions
                .Select(g => string.Join(", ", g.Names))
                .Distinct()
                .ToList();

            var solutionsText = string.Join("\nИЛИ\n", formattedSolutions);

            // Добавляем кнопку "Анализ результатов"
            string[] options = { "Анализ результатов", "Ok" };
            string analysisButtonText = options[0];
            string okButtonText = options[1];
            var result = await Application.Current.MainPage.DisplayAlert("Найденные решения", $"{solutionsText}", analysisButtonText, okButtonText);

            // Если пользователь выбрал "Анализ результатов"
            if (result)
            {
                // Вызываем метод DisplayAnalysisResults, передавая результаты анализа
                string metricName = SelectedMetric; // Здесь нужно получить выбранную метрику
                string analysisResult = DisplayAnalysisResults(metricName, analysisResults, situationParams);
                await Application.Current.MainPage.DisplayAlert("Анализ результатов", analysisResult, okButtonText);
            }

            bool isSolutionHelpful = await AskUserIfSolutionHelpful($"Помогло ли решение достичь цели?");
            if (isSolutionHelpful)
            {
                bool askSaveSolution = await AskUserSaveSolution($"Хотите сохранить данный прецедент?");
                if (askSaveSolution)
                {
                    int[] solutionsArray = await CreateSolutionsParamsList(situationParams, _selectedObjectId);
                    await AddSolutionToDatabase(situationParams, solutionsArray, _selectedObjectId);
                }
            }
            else
            {
                await SuggestOtherMetrics();
            }
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Неудача", "Для выбранного объекта не найдено подходящих решений", "Ok");
        }
    }
        private async Task<bool> AskUserIfSolutionHelpful(string message)
        {
            return await Application.Current.MainPage.DisplayAlert("Проверка решения", message, "Да", "Нет");
        }
          private async Task<bool> AskUserSaveSolution(string message)
        {
            return await Application.Current.MainPage.DisplayAlert("Сохранение", message, "Да", "Нет");
        }
       private async Task AddSolutionToDatabase(int[] situationParams, int[] solutionParams, int objectId)
        {
              // Проверяем, существует ли уже такой прецедент в базе данных
            bool precedentExists = await _context.Precedents.AnyAsync(p => 
                p.ObjectId == objectId && 
                p.SituationParams.SequenceEqual(situationParams) &&
                p.SolutionParams.SequenceEqual(solutionParams));

            // Если прецедент уже существует, выходим из метода
            if (precedentExists)
            {
                await Application.Current.MainPage.DisplayAlert("Сохранение", "Данный прецедент уже существует ", "Ok");
                return;
            }
            // Создаем новый экземпляр Precedent
            var newPrecedent = new Precedent
            {
                ObjectId = objectId,
                SituationParams = situationParams,
                SolutionParams = solutionParams
            };

            // Добавляем новый Precedent в контекст базы данных
            await _context.Precedents.AddAsync(newPrecedent);

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            await Application.Current.MainPage.DisplayAlert("Сохранение", "Прецедент успешно сохранен", "Ok");

        }
        private async Task SuggestOtherMetrics()
        {
            await Application.Current.MainPage.DisplayAlert("Предложение", "Воспользуйтесь другим методом поиска решений и затем добавьте прецедент вручную", "Ok");

        }
        private async Task<int[]> CreateSolutionsParamsList(int[] solutionsParamsList, int objectId)
        {
            // Получаем общее количество решений для объекта из базы данных
            var totalSolutionsCount = await _context.SolutionVariables
                                                    .CountAsync(sv => sv.ObjectId == objectId);

            // Создаем массив для хранения параметров решения с нужным количеством элементов
            var solutionsParams = new int[totalSolutionsCount];

            // Инициализируем все элементы массива как 0
            Array.Fill(solutionsParams, 0);

            // Устанавливаем 1 для индексов, которые были найдены как решения
            foreach (var index in allSelectedSolutionsIndexes)
            {
                if (index <= totalSolutionsCount)
                {
                    solutionsParams[index - 1] = 1; // Уменьшаем индекс на 1, потому что индексы в базе данных начинаются с 1
                }
            }

            return solutionsParams;
        }
     private Tuple<List<int[]>, List<AnalysisResult>> FindSolutions(int[] situationParams, string selectedMetric)
        {
            var metric = ConvertStringToMetric(selectedMetric);
            double minDistance = double.MaxValue;
            List<Precedent> bestMatches = new List<Precedent>();
            List<AnalysisResult> analysisResults = new List<AnalysisResult>();

            foreach (var precedent in Precedents)
            {
                if (precedent.SituationParams != null)
                {
                    double distance = CalculateDistance(situationParams, precedent.SituationParams, metric);
                    if (distance < minDistance)
                    {
                        bestMatches.Clear();
                        minDistance = distance;
                        bestMatches.Add(precedent);
                    }
                    else if (distance == minDistance)
                    {
                        bestMatches.Add(precedent);
                    }

                    analysisResults.Add(new AnalysisResult { PrecedentNumber = precedent.Id, Distance = distance });
                }
            }

            var solutions = bestMatches.Select(p => p.SolutionParams).ToList();
            return Tuple.Create(solutions, analysisResults);
        }
        private double CalculateDistance(int[] inputValues, int[] precedentValues, DistanceMetric selectedMetric)
        {
            switch (selectedMetric)
            {
                case DistanceMetric.Euclidean:
                    return CalculateEuclideanDistance(inputValues, precedentValues);
                case DistanceMetric.Manhattan:
                    return CalculateManhattanDistance(inputValues, precedentValues);
                case DistanceMetric.Hamming:
                    return CalculateHammingDistance(inputValues, precedentValues);
                case DistanceMetric.Cosine:
                    return CalculateCosineDistance(inputValues, precedentValues);
                default:
                    throw new NotSupportedException($"The metric {selectedMetric} is not supported.");
            }
        }
        private double CalculateEuclideanDistance(int[] inputValues, int[] precedentValues)
        {
            double distance = 0;
            for (int i = 0; i < inputValues.Length && i < precedentValues.Length; i++)
            {
                distance += Math.Pow(inputValues[i] - precedentValues[i], 2);
            }
            return Math.Sqrt(distance);
        }
        private double CalculateManhattanDistance(int[] inputValues, int[] precedentValues)
        {
            double distance = 0;
            for (int i = 0; i < inputValues.Length && i < precedentValues.Length; i++)
            {
                distance += Math.Abs(inputValues[i] - precedentValues[i]);
            }
            return distance;
        }
        private int CalculateHammingDistance(int[] inputValues, int[] precedentValues)
        {
            int distance = 0;
            for (int i = 0; i < inputValues.Length && i < precedentValues.Length; i++)
            {
                if (inputValues[i] != precedentValues[i]) distance++;
            }
            return distance;
        }
        private double CalculateCosineDistance(int[] inputValues, int[] precedentValues)
        {
            double dotProduct = 0;
            double normA = 0;
            double normB = 0;
            for (int i = 0; i < inputValues.Length && i < precedentValues.Length; i++)
            {
                dotProduct += inputValues[i] * precedentValues[i];
                normA += Math.Pow(inputValues[i], 2);
                normB += Math.Pow(precedentValues[i], 2);
            }
            double cosineSimilarity = dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
            return 1 - cosineSimilarity; // Преобразование сходства в расстояние
        }
         public async void SelectObject(int objectId)
        {
            _selectedObjectId = objectId;
            await InitializeAsync(); // Инициализация данных
        }
        public string DisplayAnalysisResults(string metricName, List<AnalysisResult> results, int[] situationParams)
        {
            var index = Metrics.IndexOf(metricName);

            StringBuilder builder = new StringBuilder();
            // Формируем строку для числа прецедентов
            string precedentsCountText = results.Count switch
            {
                1 => "прецедент",
                int count when count >= 2 && count <= 4 => "прецедента",
                _ => "прецедентов"
            };
            builder.AppendLine($"Было проанализировано {results.Count} {precedentsCountText}");
            builder.AppendLine($"Метрика: {metricName}");
            builder.AppendLine($"Введенный вектор параметров ситуации: [{string.Join(",", situationParams)}]");
            builder.AppendLine("Результат:");

            // Формируем заголовок таблицы
            builder.AppendLine("Номер прецедента в БД | Расстояние ");
            var sortedResults = results.OrderBy(r => r.PrecedentNumber);

            foreach (var result in sortedResults)
            {
                builder.AppendLine($"{result.PrecedentNumber,-30}                   {result.Distance,-11} ");
            }
            // Находим минимальное расстояние
            var minDistance = sortedResults.Min(r => r.Distance);
        
            // Формируем строку с ближайшими решениями
            var nearestSolutions = sortedResults.Where(r => r.Distance == minDistance).Select(r => r.PrecedentNumber);

            // Формируем строку в зависимости от числа ближайших прецедентов
            builder.AppendLine(nearestSolutions.Count() switch
            {
                1 => $"Номер ближайшего прецедента: {string.Join(", ", nearestSolutions)}\n(расстояние: {minDistance})",
                _ => $"Номера ближайших прецедентов: {string.Join(", ", nearestSolutions)}\n(расстояние: {minDistance})"
            });
            return builder.ToString();
        }
    }
}
