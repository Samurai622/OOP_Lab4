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
        Task UpdateSensorAsync(int id, SensorDto sensor); // ДОДАНО
        
        Task<DeviceDto> CreateDeviceAsync(DeviceDto device);
        Task UpdateDeviceAsync(int id, DeviceDto device);
        Task DeleteDeviceAsync(int id);
    }

    public class Task1ApiService : ITask1ApiService
    {
        private readonly HttpClient _httpClient = new();
        private const string BaseUrl = "http://localhost:3000/api/task1";

        private async Task EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Помилка API Node.js: {err}");
            }
        }

        public async Task<List<ChannelDto>> GetChannelsAsync() => await _httpClient.GetFromJsonAsync<List<ChannelDto>>($"{BaseUrl}/channels") ?? new();
        public async Task<ChannelDto> GetChannelAsync(int id) => await _httpClient.GetFromJsonAsync<ChannelDto>($"{BaseUrl}/channels/{id}") ?? new();

        public async Task<ChannelDto> CreateChannelAsync(ChannelDto channel)
        {
            var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/channels", channel);
            await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<ChannelDto>() ?? new();
        }
        public async Task UpdateChannelAsync(int id, ChannelDto channel) { var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/channels/{id}", channel); await EnsureSuccess(r); }
        public async Task DeleteChannelAsync(int id) { var r = await _httpClient.DeleteAsync($"{BaseUrl}/channels/{id}"); await EnsureSuccess(r); }

        public async Task<SensorDto> CreateSensorAsync(SensorDto sensor)
        {
            var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sensors", sensor);
            await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<SensorDto>() ?? new();
        }
        
        // ДОДАНО РЕАЛІЗАЦІЮ
        public async Task UpdateSensorAsync(int id, SensorDto sensor) 
        { 
            var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/sensors/{id}", sensor); 
            await EnsureSuccess(r); 
        }

        public async Task<DeviceDto> CreateDeviceAsync(DeviceDto device)
        {
            var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/devices", device);
            await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<DeviceDto>() ?? new();
        }
        public async Task UpdateDeviceAsync(int id, DeviceDto device) { var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/devices/{id}", device); await EnsureSuccess(r); }
        public async Task DeleteDeviceAsync(int id) { var r = await _httpClient.DeleteAsync($"{BaseUrl}/devices/{id}"); await EnsureSuccess(r); }
    }
}