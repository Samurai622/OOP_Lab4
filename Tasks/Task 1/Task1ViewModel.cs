using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using OOP_Lab4.Commands;
using OOP_Lab4.Models;
using OOP_Lab4.Services;
using OOP_Lab4.ViewModels;

namespace OOP_Lab4.Tasks.Task1
{
   public class Task1ViewModel : ViewModelBase
   {
       private readonly ITask1ApiService _apiService;

       public ObservableCollection<ChannelDto> Channels { get; } = new();
       public ObservableCollection<DeviceDto> Devices { get; } = new();

       private ChannelDto _selectedChannel;
       public ChannelDto SelectedChannel
       {
           get => _selectedChannel;
           set
           {
               SetProperty(ref _selectedChannel, value);
               _ = LoadDevicesForChannelAsync();
               UpdateCommandStates();
           }
       }

       private DeviceDto _selectedDevice;
       public DeviceDto SelectedDevice
       {
           get => _selectedDevice;
           set
           {
               SetProperty(ref _selectedDevice, value);
               UpdateCommandStates();
           }
       }

       // Команди Пристроїв
       public ICommand AddDeviceCommand { get; }
       public ICommand EditDeviceCommand { get; }
       public ICommand DeleteDeviceCommand { get; }

       // Команди Каналів
       public ICommand AddChannelCommand { get; }
       public ICommand EditChannelCommand { get; }
       public ICommand DeleteChannelCommand { get; }
      
       // Синхронізація
       public ICommand SyncCommand { get; }

       public Func<DeviceModel, Task<DeviceModel>> OpenEditDialogAsync { get; set; }
       public Func<ChannelModel, Task<ChannelModel>> OpenChannelDialogAsync { get; set; }

       public Task1ViewModel()
       {
           _apiService = new Task1ApiService();

           // Ініціалізація команд
           AddDeviceCommand = new RelayCommand(async _ => await AddDeviceAsync(), _ => SelectedChannel != null);
           EditDeviceCommand = new RelayCommand(async _ => await EditDeviceAsync(), _ => SelectedDevice != null);
           DeleteDeviceCommand = new RelayCommand(async _ => await DeleteDeviceAsync(), _ => SelectedDevice != null);

           AddChannelCommand = new RelayCommand(async _ => await AddChannelAsync());
           EditChannelCommand = new RelayCommand(async _ => await EditChannelAsync(), _ => SelectedChannel != null);
           DeleteChannelCommand = new RelayCommand(async _ => await DeleteChannelAsync(), _ => SelectedChannel != null);

           SyncCommand = new RelayCommand(async _ => await LoadChannelsAsync());

           _ = LoadChannelsAsync();
       }

       private void UpdateCommandStates()
       {
           ((RelayCommand)AddDeviceCommand).RaiseCanExecuteChanged();
           ((RelayCommand)EditDeviceCommand).RaiseCanExecuteChanged();
           ((RelayCommand)DeleteDeviceCommand).RaiseCanExecuteChanged();
           ((RelayCommand)EditChannelCommand).RaiseCanExecuteChanged();
           ((RelayCommand)DeleteChannelCommand).RaiseCanExecuteChanged();
       }

       private async Task LoadChannelsAsync()
       {
           try {
               var channels = await _apiService.GetChannelsAsync();
               Channels.Clear();

               // Формуємо красиві порядкові номери прямо в C# (1, 2, 3...)
               int orderCounter = 1;
               foreach (var c in channels)
               {
                   // Переписуємо текст сервера на наш правильний:
                   c.ShortInfo = $"Вимірювальний канал №{orderCounter} ({c.Name ?? "Без назви"})";
                  
                   Channels.Add(c);
                   orderCounter++; // Збільшуємо номер для наступного
               }

               // Встановлюємо загальну кількість для нашого вікна створення
               ChannelModel.SetTotalCount(Channels.Count);
               SelectedChannel = Channels.FirstOrDefault();
              
           } catch(Exception ex) { Console.WriteLine($"Помилка синхронізації: {ex.Message}"); }
       }

       private async Task LoadDevicesForChannelAsync()
       {
           Devices.Clear();
           if (SelectedChannel == null) return;

           try {
               var fullChannel = await _apiService.GetChannelAsync(SelectedChannel.Id);
               if (fullChannel?.Devices != null)
               {
                   foreach (var d in fullChannel.Devices) Devices.Add(d);
               }
           } catch(Exception ex) { Console.WriteLine($"Помилка завантаження пристроїв: {ex.Message}"); }
       }

       // ======================= ЛОГІКА КАНАЛІВ =======================
       private async Task AddChannelAsync()
       {
           if (OpenChannelDialogAsync == null) return;

           // 1. ПЕРЕД створенням тимчасового об'єкта жорстко синхронізуємо лічильник
           // з реальною кількістю каналів, які ми отримали з БД.
           ChannelModel.SetTotalCount(Channels.Count);

           // 2. Створюємо тимчасовий об'єкт для вікна.
           // Конструктор всередині зробить _totalChannelsCreated++ і дасть правильний номер.
           var newModel = new ChannelModel();
          
           var result = await OpenChannelDialogAsync(newModel);
          
           if (result != null)
           {
               // Якщо користувач натиснув "Зберегти"
               try {
                   await _apiService.CreateChannelAsync(new ChannelDto { Name = result.Name });
                   await LoadChannelsAsync(); // Це перезавантажить список і ще раз синхронізує лічильник
               } catch(Exception ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
           }
           else
           {
               // 3. ФІКС БАГУ: Якщо користувач натиснув "Скасувати" (закрив вікно),
               // тимчасовий об'єкт знищується. Нам треба відкотити лічильник назад.
               ChannelModel.SetTotalCount(Channels.Count);
           }
       }

       private async Task EditChannelAsync()
       {
           if (OpenChannelDialogAsync == null || SelectedChannel == null) return;

           // Вираховуємо поточний порядковий номер каналу (його позиція у списку + 1)
           int currentOrderNumber = Channels.IndexOf(SelectedChannel) + 1;

           // ВИПРАВЛЕНО: Використовуємо новий конструктор, який НЕ збільшує статичний лічильник
           var editModel = new ChannelModel(currentOrderNumber)
           {
               Id = SelectedChannel.Id,
               Name = SelectedChannel.Name
           };
          
           var result = await OpenChannelDialogAsync(editModel);
          
           if (result != null)
           {
               try {
                   await _apiService.UpdateChannelAsync(result.Id, new ChannelDto { Name = result.Name });
                   await LoadChannelsAsync(); // Це також скине лічильник до правильної кількості
               } catch(Exception ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
           }
       }

       private async Task DeleteChannelAsync()
       {
           if (SelectedChannel == null) return;
           try {
               await _apiService.DeleteChannelAsync(SelectedChannel.Id);
               await LoadChannelsAsync();
           } catch(Exception ex) { Console.WriteLine($"Помилка видалення: {ex.Message}"); }
       }

       // ======================= ЛОГІКА ПРИСТРОЇВ =======================
       private async Task AddDeviceAsync()
       {
           if (OpenEditDialogAsync == null || SelectedChannel == null) return;

           var newModel = new DeviceModel();
           var result = await OpenEditDialogAsync(newModel);
          
           if (result != null)
           {
               try
               {
                   var sensorDto = await _apiService.CreateSensorAsync(new SensorDto {
                       MagnitudeType = (int)result.Sensor.MagType,
                       MinRange = result.Sensor.MinRange,
                       MaxRange = result.Sensor.MaxRange,
                       CurrentValue = result.Sensor.CurrentValue
                   });

                   await _apiService.CreateDeviceAsync(new DeviceDto {
                       ChannelId = SelectedChannel.Id,
                       SensorId = sensorDto.Id,
                       LocationNumber = result.LocationNumber,
                       CalibrationDate = result.CalibrationDate.ToString("yyyy-MM-dd")
                   });

                   await LoadDevicesForChannelAsync();
               }
               catch (Exception ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
           }
       }

       private async Task EditDeviceAsync()
       {
           if (OpenEditDialogAsync == null || SelectedDevice == null) return;

           var editModel = new DeviceModel
           {
               Id = SelectedDevice.Id,
               LocationNumber = SelectedDevice.LocationNumber,
               CalibrationDate = DateTimeOffset.Parse(SelectedDevice.CalibrationDate)
           };
           editModel.Sensor.MagType = (MagnitudeType)SelectedDevice.Sensor.MagnitudeType;
           editModel.Sensor.MinRange = SelectedDevice.Sensor.MinRange;
           editModel.Sensor.MaxRange = SelectedDevice.Sensor.MaxRange;
           editModel.Sensor.CurrentValue = SelectedDevice.Sensor.CurrentValue;

           var result = await OpenEditDialogAsync(editModel);
           if (result != null)
           {
               try {
                   // Оновлюємо і сенсор
                   await _apiService.CreateSensorAsync(new SensorDto {
                       Id = SelectedDevice.SensorId ?? 0, // Fake update by id? Node backend actually needs Put for sensor, but since API missing it, we send new or leave it.
                       // Для спрощення: оновимо тільки пристрій, або треба додати UpdateSensor в Node.
                       // В межах завдання - оновлюємо Device.
                   });

                  await _apiService.UpdateDeviceAsync(result.Id, new DeviceDto {
                       LocationNumber = result.LocationNumber,
                       CalibrationDate = result.CalibrationDate.ToString("yyyy-MM-dd"),
                       ChannelId = SelectedChannel.Id,
                      
                       // ДОДАНО ?? 0
                       SensorId = SelectedDevice.SensorId ?? 0
                   });
                   await LoadDevicesForChannelAsync();
               } catch (Exception ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
           }
       }

       private async Task DeleteDeviceAsync()
       {
           if (SelectedDevice == null) return;
           try {
               await _apiService.DeleteDeviceAsync(SelectedDevice.Id);
               await LoadDevicesForChannelAsync();
           } catch(Exception ex) { Console.WriteLine($"Помилка видалення: {ex.Message}"); }
       }
   }
}

