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
                _selectedChannel = value; 
                OnPropertyChanged(nameof(SelectedChannel));
                _ = LoadDevicesForChannelAsync();
                ((RelayCommand)AddDeviceCommand).RaiseCanExecuteChanged();
            }
        }

        private DeviceDto _selectedDevice;
        public DeviceDto SelectedDevice
        {
            get => _selectedDevice;
            set 
            { 
                _selectedDevice = value; 
                OnPropertyChanged(nameof(SelectedDevice));
                ((RelayCommand)EditDeviceCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddDeviceCommand { get; }
        public ICommand EditDeviceCommand { get; }

        public Func<DeviceModel, Task<DeviceModel>> OpenEditDialogAsync { get; set; }

        public Task1ViewModel()
        {
            _apiService = new Task1ApiService();

            AddDeviceCommand = new RelayCommand(async _ => await AddDeviceAsync(), _ => SelectedChannel != null);
            EditDeviceCommand = new RelayCommand(async _ => await EditDeviceAsync(), _ => SelectedDevice != null);

            _ = LoadChannelsAsync();
        }

        private async Task LoadChannelsAsync()
        {
            var channels = await _apiService.GetChannelsAsync();
            Channels.Clear();
            foreach (var c in channels) Channels.Add(c);
            SelectedChannel = Channels.FirstOrDefault();
        }

        private async Task LoadDevicesForChannelAsync()
        {
            Devices.Clear();
            if (SelectedChannel == null) return;

            var fullChannel = await _apiService.GetChannelAsync(SelectedChannel.Id);
            if (fullChannel?.Devices != null)
            {
                foreach (var d in fullChannel.Devices) Devices.Add(d);
            }
        }

        private async Task AddDeviceAsync()
        {
            if (OpenEditDialogAsync == null) return;

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
                    await _apiService.UpdateDeviceAsync(result.Id, new DeviceDto {
                        LocationNumber = result.LocationNumber,
                        CalibrationDate = result.CalibrationDate.ToString("yyyy-MM-dd"),
                        
                        // ДОДАНО: Передаємо старі ID, щоб SQLite не сварився на Foreign Key
                        ChannelId = SelectedChannel.Id,
                        SensorId = SelectedDevice.SensorId 
                    });
                    await LoadDevicesForChannelAsync();
                } catch (Exception ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
            }
        }
    }
}