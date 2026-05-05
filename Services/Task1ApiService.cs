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
      
       // ДОДАНО ДЛЯ КАНАЛІВ
       Task<ChannelDto> CreateChannelAsync(ChannelDto channel);
       Task UpdateChannelAsync(int id, ChannelDto channel);
       Task DeleteChannelAsync(int id);

       Task<SensorDto> CreateSensorAsync(SensorDto sensor);
       Task<DeviceDto> CreateDeviceAsync(DeviceDto device);
       Task UpdateDeviceAsync(int id, DeviceDto device);
      
       // ДОДАНО ДЛЯ ПРИСТРОЇВ
       Task DeleteDeviceAsync(int id);
   }

   public class Task1ApiService : ITask1ApiService
   {
       private readonly HttpClient _httpClient;
       private const string BaseUrl = "http://localhost:3000/api/task1";

       public Task1ApiService() { _httpClient = new HttpClient(); }

       public async Task<List<ChannelDto>> GetChannelsAsync() => await _httpClient.GetFromJsonAsync<List<ChannelDto>>($"{BaseUrl}/channels");
       public async Task<ChannelDto> GetChannelAsync(int id) => await _httpClient.GetFromJsonAsync<ChannelDto>($"{BaseUrl}/channels/{id}");

       public async Task<ChannelDto> CreateChannelAsync(ChannelDto channel)
       {
           var res = await _httpClient.PostAsJsonAsync($"{BaseUrl}/channels", channel);
           res.EnsureSuccessStatusCode();
           return await res.Content.ReadFromJsonAsync<ChannelDto>();
       }

       public async Task UpdateChannelAsync(int id, ChannelDto channel)
       {
           var res = await _httpClient.PutAsJsonAsync($"{BaseUrl}/channels/{id}", channel);
           res.EnsureSuccessStatusCode();
       }

       public async Task DeleteChannelAsync(int id)
       {
           var res = await _httpClient.DeleteAsync($"{BaseUrl}/channels/{id}");
           res.EnsureSuccessStatusCode();
       }

       public async Task<SensorDto> CreateSensorAsync(SensorDto sensor)
       {
           var res = await _httpClient.PostAsJsonAsync($"{BaseUrl}/sensors", sensor);
           res.EnsureSuccessStatusCode();
           return await res.Content.ReadFromJsonAsync<SensorDto>();
       }

       public async Task<DeviceDto> CreateDeviceAsync(DeviceDto device)
       {
           var res = await _httpClient.PostAsJsonAsync($"{BaseUrl}/devices", device);
           res.EnsureSuccessStatusCode();
           return await res.Content.ReadFromJsonAsync<DeviceDto>();
       }

       public async Task UpdateDeviceAsync(int id, DeviceDto device)
       {
           var res = await _httpClient.PutAsJsonAsync($"{BaseUrl}/devices/{id}", device);
           res.EnsureSuccessStatusCode();
       }

       public async Task DeleteDeviceAsync(int id)
       {
           var res = await _httpClient.DeleteAsync($"{BaseUrl}/devices/{id}");
           res.EnsureSuccessStatusCode();
       }
   }
}
