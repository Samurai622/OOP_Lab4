using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Services;
using OOP_Lab4.Tasks.Task1; 
using OOP_Lab4.Tasks.Task2; 

namespace OOP_Lab4.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IBrowserService _browserService;
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        private bool _isTask1Active = true;
        public bool IsTask1Active { get => _isTask1Active; set => SetProperty(ref _isTask1Active, value); }

        private bool _isTask2Active = false;
        public bool IsTask2Active { get => _isTask2Active; set => SetProperty(ref _isTask2Active, value); }

        // ЗМІННІ ДЛЯ ЕКРАНУ БЛОКУВАННЯ
        private bool _isLocked = false;
        public bool IsLocked { get => _isLocked; set => SetProperty(ref _isLocked, value); }

        private string _unlockPassword = "";
        public string UnlockPassword { get => _unlockPassword; set => SetProperty(ref _unlockPassword, value); }

        private string _lockMessage = "Увага! Сервер заблокував ваш IP через підозру на DDoS.\nВведіть пароль адміністратора для скидання лімітів.";
        public string LockMessage { get => _lockMessage; set => SetProperty(ref _lockMessage, value); }

        public ICommand UnlockCommand { get; }
        public ICommand ShowTask1Command { get; }
        public ICommand ShowTask2Command { get; }
        public ICommand OpenWebCommand { get; }

        public MainViewModel(IBrowserService browserService)
        {
            _browserService = browserService;
            _currentViewModel = new Task1ViewModel(); 

            AppConfig.OnDdosBlocked = () => IsLocked = true;

            UnlockCommand = new RelayCommand(async _ => await TryUnlockAsync());

            ShowTask1Command = new RelayCommand(_ => { 
                CurrentViewModel = new Task1ViewModel();
                IsTask1Active = true; IsTask2Active = false; 
            });
            
            ShowTask2Command = new RelayCommand(_ => { 
                CurrentViewModel = new Task2ViewModel();
                IsTask1Active = false; IsTask2Active = true; 
            });
            
            OpenWebCommand = new RelayCommand(_ => {
                string webUrl = AppConfig.ApiBaseUrl.Replace("/api", "");
                _browserService.OpenUrl(webUrl); 
            }); 
        }

        private async Task TryUnlockAsync()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.PostAsJsonAsync($"{AppConfig.ApiBaseUrl}/unlock", new { password = UnlockPassword });
                
                if (response.IsSuccessStatusCode)
                {
                    IsLocked = false;
                    UnlockPassword = "";
                    LockMessage = "Увага! Сервер заблокував ваш IP через підозру на DDoS.\nВведіть пароль адміністратора для скидання лімітів.";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    var error = await response.Content.ReadFromJsonAsync<JsonElement>();
                    LockMessage = error.GetProperty("error").GetString() ?? "БАН!";
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<JsonElement>();
                    LockMessage = error.GetProperty("error").GetString();
                }
            }
            catch (Exception ex) { LockMessage = "Помилка зв'язку з сервером."; }
        }

        public string GetCurrentJson()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            if (CurrentViewModel is Task1ViewModel t1) return JsonSerializer.Serialize(t1.Channels, options);
            if (CurrentViewModel is Task2ViewModel t2) return JsonSerializer.Serialize(t2.Competitions, options);
            return "{}";
        }
    }
}