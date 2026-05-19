using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OOP_Lab4.Models;

namespace OOP_Lab4.Services
{
    public interface ITask1ApiService
    {
        Task<List<ChannelDto>> GetChannelsAsync();
        Task<ChannelDto> GetChannelAsync(int id);
        Task<ChannelDto> CreateChannelAsync(ChannelDto channel);
        Task UpdateChannelAsync(int id, ChannelDto channel);
        Task DeleteChannelAsync(int id);
        Task<SensorDto> CreateSensorAsync(SensorDto sensor);
        Task UpdateSensorAsync(int id, SensorDto sensor); 
        Task<DeviceDto> CreateDeviceAsync(DeviceDto device);
        Task UpdateDeviceAsync(int id, DeviceDto device);
        Task DeleteDeviceAsync(int id);
    }

    public class Task1ApiService : ITask1ApiService
    {
        private readonly HttpClient _httpClient = new();
        private string BaseUrl => $"{AppConfig.ApiBaseUrl}/task1";

        private void PrepareHeaders()
        {
            _httpClient.DefaultRequestHeaders.Remove("x-admin-bypass");
            if (!string.IsNullOrEmpty(AppConfig.AdminPassword))
            {
                _httpClient.DefaultRequestHeaders.Add("x-admin-bypass", AppConfig.AdminPassword);
            }
        }

        private async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests) throw new Exception("DDOS_BLOCK");
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Помилка API: {err}");
            }
        }

        public async Task<List<ChannelDto>> GetChannelsAsync() { PrepareHeaders(); var r = await _httpClient.GetAsync($"{BaseUrl}/channels"); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<List<ChannelDto>>() ?? new(); }
        public async Task<ChannelDto> GetChannelAsync(int id) { PrepareHeaders(); var r = await _httpClient.GetAsync($"{BaseUrl}/channels/{id}"); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<ChannelDto>() ?? new(); }
        public async Task<ChannelDto> CreateChannelAsync(ChannelDto channel) { PrepareHeaders(); var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/channels", channel); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<ChannelDto>() ?? new(); }
        public async Task UpdateChannelAsync(int id, ChannelDto channel) { PrepareHeaders(); var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/channels/{id}", channel); await EnsureSuccess(r); }
        public async Task DeleteChannelAsync(int id) { PrepareHeaders(); var r = await _httpClient.DeleteAsync($"{BaseUrl}/channels/{id}"); await EnsureSuccess(r); }
        public async Task<SensorDto> CreateSensorAsync(SensorDto sensor) { PrepareHeaders(); var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sensors", sensor); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<SensorDto>() ?? new(); }
        public async Task UpdateSensorAsync(int id, SensorDto sensor) { PrepareHeaders(); var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/sensors/{id}", sensor); await EnsureSuccess(r); }
        public async Task<DeviceDto> CreateDeviceAsync(DeviceDto device) { PrepareHeaders(); var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/devices", device); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<DeviceDto>() ?? new(); }
        public async Task UpdateDeviceAsync(int id, DeviceDto device) { PrepareHeaders(); var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/devices/{id}", device); await EnsureSuccess(r); }
        public async Task DeleteDeviceAsync(int id) { PrepareHeaders(); var r = await _httpClient.DeleteAsync($"{BaseUrl}/devices/{id}"); await EnsureSuccess(r); }
    }
}