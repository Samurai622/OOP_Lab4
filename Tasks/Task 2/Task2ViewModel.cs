using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Models;
using OOP_Lab4.Services;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4.Tasks.Task2
{
    public class Task2ViewModel : ViewModelBase
    {
        private readonly ITask2ApiService _apiService = new Task2ApiService();

        public ObservableCollection<CompetitionDto> Competitions { get; } = new();
        public ObservableCollection<PerformanceDto> Performances { get; } = new();

        private CompetitionDto _selectedComp;
        public CompetitionDto SelectedCompetition
        {
            get => _selectedComp;
            set { SetProperty(ref _selectedComp, value); _ = LoadPerformancesAsync(); UpdateCommands(); }
        }

        private PerformanceDto _selectedPerf;
        public PerformanceDto SelectedPerformance
        {
            get => _selectedPerf;
            set { SetProperty(ref _selectedPerf, value); UpdateCommands(); }
        }

        // Змінна для повідомлень (База пуста, Успішно, DDoS тощо)
        private string _syncStatusMessage = string.Empty;
        public string SyncStatusMessage
        {
            get => _syncStatusMessage;
            set => SetProperty(ref _syncStatusMessage, value);
        }

        public ICommand AddCompCmd { get; }
        public ICommand EditCompCmd { get; }
        public ICommand DelCompCmd { get; }
        
        public ICommand AddPerfCmd { get; }
        public ICommand EditPerfCmd { get; }
        public ICommand DelPerfCmd { get; }
        public ICommand SyncCmd { get; }

        public Func<CompetitionModel, Task<CompetitionModel>> OpenCompDialog { get; set; }
        public Func<PerformanceModel, Task<PerformanceModel>> OpenPerfDialog { get; set; }

        public Task2ViewModel()
        {
            AddCompCmd = new RelayCommand(async _ => await AddComp());
            EditCompCmd = new RelayCommand(async _ => await EditComp(), _ => SelectedCompetition != null);
            DelCompCmd = new RelayCommand(async _ => await DelComp(), _ => SelectedCompetition != null);

            AddPerfCmd = new RelayCommand(async _ => await AddPerf(), _ => SelectedCompetition != null);
            EditPerfCmd = new RelayCommand(async _ => await EditPerf(), _ => SelectedPerformance != null);
            DelPerfCmd = new RelayCommand(async _ => await DelPerf(), _ => SelectedPerformance != null);

            SyncCmd = new RelayCommand(async _ => await LoadCompetitionsAsync());
            
            _ = LoadCompetitionsAsync();
        }

        private void UpdateCommands()
        {
            ((RelayCommand)AddPerfCmd).RaiseCanExecuteChanged();
            ((RelayCommand)EditPerfCmd).RaiseCanExecuteChanged();
            ((RelayCommand)DelPerfCmd).RaiseCanExecuteChanged();
            ((RelayCommand)EditCompCmd).RaiseCanExecuteChanged();
            ((RelayCommand)DelCompCmd).RaiseCanExecuteChanged();
        }

        private async Task LoadCompetitionsAsync()
        {
            try {
                int? selectedId = SelectedCompetition?.Id;

                var comps = await _apiService.GetCompetitionsAsync();
                Competitions.Clear();
                foreach (var c in comps) 
                {
                    c.ShortInfo = CompetitionModel.ToShortString(c); 
                    Competitions.Add(c);
                }

                if (selectedId.HasValue)
                    SelectedCompetition = Competitions.FirstOrDefault(c => c.Id == selectedId.Value);
                else
                    SelectedCompetition = Competitions.FirstOrDefault();

                if (Competitions.Count == 0) SyncStatusMessage = "База даних пуста!";
                else SyncStatusMessage = "Успішно синхронізовано!";
                _ = Task.Delay(3000).ContinueWith(_ => SyncStatusMessage = string.Empty, TaskScheduler.FromCurrentSynchronizationContext());

            } catch(Exception ex) { 
                if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                else SyncStatusMessage = "Помилка з'єднання!";
                Console.WriteLine($"Помилка: {ex.Message}"); 
            }
        }

        private async Task LoadPerformancesAsync()
        {
            Performances.Clear();
            if (SelectedCompetition == null) return;
            try {
                var comp = await _apiService.GetCompetitionAsync(SelectedCompetition.Id);
                if (comp?.Performances != null)
                {
                    foreach (var p in comp.Performances) Performances.Add(p);
                }
                PerformanceModel.SetTotalCount(Performances.Count); 
            } catch(Exception ex) { 
                if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                Console.WriteLine($"Помилка: {ex.Message}"); 
            }
        }

        private async Task AddComp()
        {
            if (OpenCompDialog == null) return;
            var res = await OpenCompDialog(new CompetitionModel());
            if (res != null) { 
                try {
                    await _apiService.CreateCompetitionAsync(new CompetitionDto{Name=res.Name}); 
                    await LoadCompetitionsAsync(); 
                } catch(Exception ex) { 
                    if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                    Console.WriteLine($"Помилка: {ex.Message}"); 
                }
            }
        }

        private async Task EditComp()
        {
            if (OpenCompDialog == null || SelectedCompetition == null) return;
            var res = await OpenCompDialog(new CompetitionModel{Id=SelectedCompetition.Id, Name=SelectedCompetition.Name});
            if (res != null) { 
                try {
                    await _apiService.UpdateCompetitionAsync(res.Id, new CompetitionDto{Name=res.Name}); 
                    await LoadCompetitionsAsync(); 
                } catch(Exception ex) { 
                    if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                    Console.WriteLine($"Помилка: {ex.Message}"); 
                }
            }
        }

        private async Task DelComp()
        {
            if (SelectedCompetition == null) return;
            try { 
                await _apiService.DeleteCompetitionAsync(SelectedCompetition.Id); 
                await LoadCompetitionsAsync(); 
            } catch(Exception ex) { 
                if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                Console.WriteLine($"Помилка: {ex.Message}"); 
            }
        }

        private async Task AddPerf()
        {
            if (OpenPerfDialog == null || SelectedCompetition == null) return;
            PerformanceModel.SetTotalCount(Performances.Count); 
            var res = await OpenPerfDialog(new PerformanceModel());
            
            if (res != null)
            {
                try {
                    var partDto = await _apiService.CreateParticipantAsync(new ParticipantDto { FirstName=res.Participant.FirstName, LastName=res.Participant.LastName, BirthDate=res.Participant.BirthDate.ToString("yyyy-MM-dd") });
                    await _apiService.CreatePerformanceAsync(new PerformanceDto { CompetitionId=SelectedCompetition.Id, ParticipantId=partDto.Id, IsTeam=res.IsTeam, ResultScore=res.ResultScore });
                    await LoadPerformancesAsync(); 
                    await LoadCompetitionsAsync(); 
                } catch(Exception ex) { 
                    if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                    Console.WriteLine($"Помилка: {ex.Message}"); 
                }
            }
            else { PerformanceModel.SetTotalCount(Performances.Count); }
        }

        private async Task EditPerf()
        {
            if (OpenPerfDialog == null || SelectedPerformance == null) return;
            int num = Performances.IndexOf(SelectedPerformance) + 1;
            
            DateTimeOffset parsedDate = DateTimeOffset.Now;
            if (!string.IsNullOrEmpty(SelectedPerformance.Participant?.BirthDate))
                DateTimeOffset.TryParse(SelectedPerformance.Participant.BirthDate, out parsedDate);

            var editModel = new PerformanceModel(num) { 
                Id = SelectedPerformance.Id, 
                IsTeam = SelectedPerformance.IsTeam, 
                ResultScore = SelectedPerformance.ResultScore,
                Participant = new ParticipantModel { 
                    FirstName = SelectedPerformance.Participant?.FirstName ?? string.Empty, 
                    LastName = SelectedPerformance.Participant?.LastName ?? string.Empty, 
                    BirthDate = parsedDate 
                }
            };
            
            var res = await OpenPerfDialog(editModel);
            if (res != null)
            {
                try {
                    if (SelectedPerformance.ParticipantId.HasValue)
                    {
                        await _apiService.UpdateParticipantAsync(SelectedPerformance.ParticipantId.Value, new ParticipantDto {
                            FirstName = res.Participant.FirstName,
                            LastName = res.Participant.LastName,
                            BirthDate = res.Participant.BirthDate.ToString("yyyy-MM-dd")
                        });
                    }
                    await _apiService.UpdatePerformanceAsync(res.Id, new PerformanceDto { 
                        IsTeam = res.IsTeam, 
                        ResultScore = res.ResultScore 
                    });
                    
                    await LoadPerformancesAsync(); 
                    await LoadCompetitionsAsync(); 
                } catch(Exception ex) { 
                    if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                    Console.WriteLine($"Помилка: {ex.Message}"); 
                }
            }
        }

        private async Task DelPerf()
        {
            if (SelectedPerformance == null) return;
            try { 
                await _apiService.DeletePerformanceAsync(SelectedPerformance.Id); 
                await LoadPerformancesAsync(); 
                await LoadCompetitionsAsync(); 
            } catch(Exception ex) { 
                if (ex.Message.Contains("DDOS_BLOCK")) SyncStatusMessage = "Введіть пароль зліва!";
                Console.WriteLine($"Помилка: {ex.Message}"); 
            }
        }
    }
}