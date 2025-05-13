using BlazorClient.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlazorClient.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly HttpClient _httpClient;

        public ConsultationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Consultation>> GetConsultationsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Consultation>>("api/Consultations") ?? new List<Consultation>();
        }
        // Implement other methods (GetById, Create, etc.)
    }

    public interface IConsultationService
    {
        Task<IEnumerable<Consultation>> GetConsultationsAsync();
        // Define other method signatures
    }
}
