using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OOP_Lab4.Models;

namespace OOP_Lab4.Services
{
    // ==========================================
    // 1. Інтерфейс (ОБОВ'ЯЗКОВО ПОВИНЕН БУТИ ТУТ)
    // ==========================================
    public interface ITask1ApiService
    {
        Task<List<ChannelDto>> GetChannelsAsync();
        Task<ChannelDto> GetChannelAsync(int id);
        Task<SensorDto> CreateSensorAsync(SensorDto sensor);
        Task<DeviceDto> CreateDeviceAsync(DeviceDto device);
        Task UpdateDeviceAsync(int id, DeviceDto device);
    }

    // ==========================================
    // 2. Реалізація
    // ==========================================
    public class Task1ApiService : ITask1ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:3000/api/task1";

        public Task1ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<ChannelDto>> GetChannelsAsync() =>
            await _httpClient.GetFromJsonAsync<List<ChannelDto>>($"{BaseUrl}/channels");

        public async Task<ChannelDto> GetChannelAsync(int id) =>
            await _httpClient.GetFromJsonAsync<ChannelDto>($"{BaseUrl}/channels/{id}");

        public async Task<SensorDto> CreateSensorAsync(SensorDto sensor)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sensors", sensor);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Node.js Error (Sensors): {error}");
            }
            return await response.Content.ReadFromJsonAsync<SensorDto>();
        }

        public async Task<DeviceDto> CreateDeviceAsync(DeviceDto device)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/devices", device);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Node.js Error (Devices): {error}");
            }
            return await response.Content.ReadFromJsonAsync<DeviceDto>();
        }

        public async Task UpdateDeviceAsync(int id, DeviceDto device)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/devices/{id}", device);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Node.js Error (Update): {error}");
            }
        }
    }
}