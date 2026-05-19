using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OOP_Lab4.Models;

namespace OOP_Lab4.Services
{
    public interface ITask2ApiService
    {
        Task<List<CompetitionDto>> GetCompetitionsAsync();
        Task<CompetitionDto> GetCompetitionAsync(int id);
        Task<CompetitionDto> CreateCompetitionAsync(CompetitionDto comp);
        Task UpdateCompetitionAsync(int id, CompetitionDto comp);
        Task DeleteCompetitionAsync(int id);
        Task<ParticipantDto> CreateParticipantAsync(ParticipantDto part);
        Task UpdateParticipantAsync(int id, ParticipantDto part); 
        Task<PerformanceDto> CreatePerformanceAsync(PerformanceDto perf);
        Task UpdatePerformanceAsync(int id, PerformanceDto perf);
        Task DeletePerformanceAsync(int id);
    }

    public class Task2ApiService : ITask2ApiService
    {
        private readonly HttpClient _httpClient = new();
        private string BaseUrl => $"{AppConfig.ApiBaseUrl}/task2";

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

        public async Task<List<CompetitionDto>> GetCompetitionsAsync() { PrepareHeaders(); var r = await _httpClient.GetAsync($"{BaseUrl}/competitions"); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<List<CompetitionDto>>() ?? new(); }
        public async Task<CompetitionDto> GetCompetitionAsync(int id) { PrepareHeaders(); var r = await _httpClient.GetAsync($"{BaseUrl}/competitions/{id}"); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<CompetitionDto>() ?? new(); }
        public async Task<CompetitionDto> CreateCompetitionAsync(CompetitionDto comp) { PrepareHeaders(); var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/competitions", comp); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<CompetitionDto>() ?? new(); }
        public async Task UpdateCompetitionAsync(int id, CompetitionDto comp) { PrepareHeaders(); var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/competitions/{id}", comp); await EnsureSuccess(r); }
        public async Task DeleteCompetitionAsync(int id) { PrepareHeaders(); var r = await _httpClient.DeleteAsync($"{BaseUrl}/competitions/{id}"); await EnsureSuccess(r); }
        public async Task<ParticipantDto> CreateParticipantAsync(ParticipantDto part) { PrepareHeaders(); var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/participants", part); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<ParticipantDto>() ?? new(); }
        public async Task UpdateParticipantAsync(int id, ParticipantDto part) { PrepareHeaders(); var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/participants/{id}", part); await EnsureSuccess(r); }
        public async Task<PerformanceDto> CreatePerformanceAsync(PerformanceDto perf) { PrepareHeaders(); var r = await _httpClient.PostAsJsonAsync($"{BaseUrl}/performances", perf); await EnsureSuccess(r); return await r.Content.ReadFromJsonAsync<PerformanceDto>() ?? new(); }
        public async Task UpdatePerformanceAsync(int id, PerformanceDto perf) { PrepareHeaders(); var r = await _httpClient.PutAsJsonAsync($"{BaseUrl}/performances/{id}", perf); await EnsureSuccess(r); }
        public async Task DeletePerformanceAsync(int id) { PrepareHeaders(); var r = await _httpClient.DeleteAsync($"{BaseUrl}/performances/{id}"); await EnsureSuccess(r); }
    }
}