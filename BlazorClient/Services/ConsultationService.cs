using BusinessLayer.DTOs;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;

namespace BlazorClient.Services
{
    public interface IConsultationService
    {
        Task<IEnumerable<ConsultationDto>> GetConsultationsAsync();
        Task<IEnumerable<ConsultationDto>> GetUpcomingConsultationsAsync();
        Task<ConsultationDto?> GetConsultationByIdAsync(Guid id);
        Task<ConsultationDto?> CreateConsultationAsync(ConsultationDto consultation);
        Task<bool> UpdateConsultationAsync(ConsultationDto consultation);
        Task<bool> CancelConsultationAsync(Guid id);
        Task<bool> LeaveReviewAsync(Guid consultationId, ReviewDto review);
        Task<IEnumerable<ConsultationDto>> GetTeacherConsultationsAsync();
        Task<IEnumerable<ConsultationDto>> GetStudentConsultationsAsync();
    }

    public class ConsultationService : IConsultationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ConsultationService> _logger;

        public ConsultationService(HttpClient httpClient, ILogger<ConsultationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ConsultationDto>> GetConsultationsAsync()
        {
            try
            {
                // Use manual HTTP call to handle custom JSON format
                var response = await _httpClient.GetAsync("api/Consultations");
                
                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received successful response from consultations endpoint: {ContentLength} bytes", content.Length);
                    
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            // First try to parse as dynamic objects
                            var dynamicResult = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(content, options);
                            
                            // Convert dynamic objects to ConsultationDto
                            var consultations = new List<ConsultationDto>();
                            
                            if (dynamicResult != null)
                            {
                                foreach (var item in dynamicResult)
                                {
                                    try
                                    {
                                        var consultation = new ConsultationDto
                                        {
                                            Id = Guid.Parse(item.GetProperty("id").GetString() ?? Guid.Empty.ToString()),
                                            Title = item.TryGetProperty("title", out var title) ? title.GetString() ?? string.Empty : string.Empty,
                                            Topic = item.TryGetProperty("topic", out var topic) ? topic.GetString() ?? string.Empty : string.Empty,
                                            Description = item.TryGetProperty("description", out var desc) ? desc.GetString() ?? string.Empty : string.Empty,
                                            ScheduledAt = item.TryGetProperty("scheduledAt", out var scheduled) ? 
                                                DateTime.Parse(scheduled.GetString() ?? DateTime.UtcNow.ToString("o")) : DateTime.UtcNow,
                                            DurationMinutes = item.TryGetProperty("durationMinutes", out var duration) ? duration.GetInt32() : 0,
                                            Status = item.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty,
                                            Location = item.TryGetProperty("location", out var location) ? location.GetString() ?? string.Empty : string.Empty,
                                            IsOnline = item.TryGetProperty("isOnline", out var isOnline) && isOnline.GetBoolean(),
                                            TeacherId = Guid.Parse(item.GetProperty("teacherId").GetString() ?? Guid.Empty.ToString()),
                                            TeacherName = item.TryGetProperty("teacherName", out var teacherName) ? teacherName.GetString() ?? string.Empty : string.Empty
                                        };
                                        
                                        consultations.Add(consultation);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Error converting JSON item to ConsultationDto");
                                    }
                                }
                            }
                            
                            return consultations;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing consultations JSON: {Content}", content);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to get consultations: {StatusCode}", response.StatusCode);
                }
                
                return new List<ConsultationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching consultations");
                return new List<ConsultationDto>();
            }
        }
        
        public async Task<IEnumerable<ConsultationDto>> GetUpcomingConsultationsAsync()
        {
            try
            {
                // Get consultations with error handling
                var response = await _httpClient.GetAsync("api/Consultations/upcoming");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received successful response from upcoming consultations: {Length} bytes", content?.Length ?? 0);
                    
                    // As a fallback, if empty array is returned, don't try to deserialize
                    if (string.IsNullOrWhiteSpace(content) || content == "[]")
                    {
                        _logger.LogInformation("Empty array returned from upcoming consultations endpoint");
                        return new List<ConsultationDto>();
                    }
                    
                    try
                    {
                        var options = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        
                        // Try to parse as array of JsonElements
                        var elements = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(content, options);
                        var result = new List<ConsultationDto>();
                        
                        // Return empty list if elements is null or empty
                        if (elements == null || elements.Length == 0)
                        {
                            return result;
                        }
                        
                        // Process each element
                        foreach (var item in elements)
                        {
                            var dto = new ConsultationDto();
                            
                            // Safely extract each property
                            try
                            {
                                if (item.TryGetProperty("id", out var id) && id.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    dto.Id = Guid.Parse(id.GetString());
                                }
                                
                                if (item.TryGetProperty("title", out var title) && title.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    dto.Title = title.GetString();
                                }
                                
                                if (item.TryGetProperty("topic", out var topic) && topic.ValueKind == System.Text.Json.JsonValueKind.String)
                                {
                                    dto.Topic = topic.GetString();
                                }
                                
                                result.Add(dto);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error parsing consultation element");
                            }
                        }
                        
                        return result;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing consultations: {Content}", content);
                    }
                }
                else
                {
                    _logger.LogWarning("Error from upcoming consultations endpoint: {StatusCode}", response.StatusCode);
                }
                
                // Always return an empty list as a fallback
                return new List<ConsultationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error fetching upcoming consultations");
                return new List<ConsultationDto>();
            }
        }
        
    public async Task<ConsultationDto?> GetConsultationByIdAsync(Guid id)
    {
        try
        {
            // Use manual HTTP call to handle custom JSON format
            var response = await _httpClient.GetAsync($"api/Consultations/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received successful response from consultation by id endpoint: {ContentLength} bytes", content.Length);
                
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        // Parse as JsonElement
                        var item = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content, options);
                        
                        // Convert to ConsultationDto
                        return new ConsultationDto
                        {
                            Id = item.TryGetProperty("id", out var idProp) ? Guid.Parse(idProp.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                            Title = item.TryGetProperty("title", out var title) ? title.GetString() ?? string.Empty : string.Empty,
                            Topic = item.TryGetProperty("topic", out var topic) ? topic.GetString() ?? string.Empty : string.Empty,
                            Description = item.TryGetProperty("description", out var desc) ? desc.GetString() ?? string.Empty : string.Empty,
                            ScheduledAt = item.TryGetProperty("scheduledAt", out var scheduled) ? 
                                DateTime.Parse(scheduled.GetString() ?? DateTime.UtcNow.ToString("o")) : DateTime.UtcNow,
                            DurationMinutes = item.TryGetProperty("durationMinutes", out var duration) ? duration.GetInt32() : 0,
                            Status = item.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty,
                            Location = item.TryGetProperty("location", out var location) ? location.GetString() ?? string.Empty : string.Empty,
                            IsOnline = item.TryGetProperty("isOnline", out var isOnline) && isOnline.GetBoolean(),
                            TeacherId = item.TryGetProperty("teacherId", out var teacherId) ? 
                                Guid.Parse(teacherId.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                            TeacherName = item.TryGetProperty("teacherName", out var teacherName) ? teacherName.GetString() ?? string.Empty : string.Empty
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing consultation JSON: {Content}", content);
                    }
                }
            }
            else
            {
                _logger.LogWarning("Failed to get consultation by ID {Id}: {StatusCode}", id, response.StatusCode);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching consultation {Id}", id);
            return null;
        }
    }
        
        public async Task<ConsultationDto?> CreateConsultationAsync(ConsultationDto consultation)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Consultations", consultation);
                
                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received successful response from create consultation endpoint: {ContentLength} bytes", content.Length);
                    
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            // Parse as JsonElement
                            var item = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content, options);
                            
                            // Convert to ConsultationDto
                            return new ConsultationDto
                            {
                                Id = item.TryGetProperty("id", out var idProp) ? Guid.Parse(idProp.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                                Title = item.TryGetProperty("title", out var title) ? title.GetString() ?? string.Empty : string.Empty,
                                Topic = item.TryGetProperty("topic", out var topic) ? topic.GetString() ?? string.Empty : string.Empty,
                                Description = item.TryGetProperty("description", out var desc) ? desc.GetString() ?? string.Empty : string.Empty,
                                ScheduledAt = item.TryGetProperty("scheduledAt", out var scheduled) ? 
                                    DateTime.Parse(scheduled.GetString() ?? DateTime.UtcNow.ToString("o")) : DateTime.UtcNow,
                                DurationMinutes = item.TryGetProperty("durationMinutes", out var duration) ? duration.GetInt32() : 0,
                                Status = item.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty,
                                Location = item.TryGetProperty("location", out var location) ? location.GetString() ?? string.Empty : string.Empty,
                                IsOnline = item.TryGetProperty("isOnline", out var isOnline) && isOnline.GetBoolean(),
                                TeacherId = item.TryGetProperty("teacherId", out var teacherId) ? 
                                    Guid.Parse(teacherId.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                                TeacherName = item.TryGetProperty("teacherName", out var teacherName) ? teacherName.GetString() ?? string.Empty : string.Empty
                            };
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing created consultation JSON: {Content}", content);
                        }
                    }
                }
                
                _logger.LogWarning("Failed to create consultation: {StatusCode}", response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating consultation");
                return null;
            }
        }
        
        public async Task<bool> UpdateConsultationAsync(ConsultationDto consultation)
        {
            try
            {
                // Convert the DateTime to ISO format for consistent serialization
                var serializable = new Dictionary<string, object>
                {
                    { "id", consultation.Id },
                    { "title", consultation.Title ?? string.Empty },
                    { "topic", consultation.Topic ?? string.Empty },
                    { "description", consultation.Description ?? string.Empty },
                    { "scheduledAt", consultation.ScheduledAt.ToString("o") }, // ISO 8601 format
                    { "durationMinutes", consultation.DurationMinutes },
                    { "status", consultation.Status ?? string.Empty },
                    { "location", consultation.Location ?? string.Empty },
                    { "isOnline", consultation.IsOnline },
                    { "teacherId", consultation.TeacherId },
                    { "teacherName", consultation.TeacherName ?? string.Empty }
                };
                
                var response = await _httpClient.PutAsJsonAsync($"api/Consultations/{consultation.Id}", serializable);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to update consultation: {StatusCode} - {Content}", 
                        response.StatusCode, await response.Content.ReadAsStringAsync());
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating consultation {Id}", consultation.Id);
                return false;
            }
        }
        
        public async Task<bool> CancelConsultationAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/Consultations/{id}/cancel", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling consultation {Id}", id);
                return false;
            }
        }
        
        public async Task<bool> LeaveReviewAsync(Guid consultationId, ReviewDto review)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/Consultations/{consultationId}/review", review);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving review for consultation {Id}", consultationId);
                return false;
            }
        }
        
        public async Task<IEnumerable<ConsultationDto>> GetTeacherConsultationsAsync()
        {
            try
            {
                // Use manual HTTP call to handle custom JSON format
                var response = await _httpClient.GetAsync("api/Consultations/teacher");
                
                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received successful response from teacher consultations endpoint: {ContentLength} bytes", content.Length);
                    
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            // First try to parse as dynamic objects
                            var dynamicResult = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(content, options);
                            
                            // Convert dynamic objects to ConsultationDto
                            var consultations = new List<ConsultationDto>();
                            
                            if (dynamicResult != null)
                            {
                                foreach (var item in dynamicResult)
                                {
                                    try
                                    {
                                        var consultation = new ConsultationDto
                                        {
                                            Id = item.TryGetProperty("id", out var idProp) ? Guid.Parse(idProp.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                                            Title = item.TryGetProperty("title", out var title) ? title.GetString() ?? string.Empty : string.Empty,
                                            Topic = item.TryGetProperty("topic", out var topic) ? topic.GetString() ?? string.Empty : string.Empty,
                                            Description = item.TryGetProperty("description", out var desc) ? desc.GetString() ?? string.Empty : string.Empty,
                                            ScheduledAt = item.TryGetProperty("scheduledAt", out var scheduled) ? 
                                                DateTime.Parse(scheduled.GetString() ?? DateTime.UtcNow.ToString("o")) : DateTime.UtcNow,
                                            DurationMinutes = item.TryGetProperty("durationMinutes", out var duration) ? duration.GetInt32() : 0,
                                            Status = item.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty,
                                            Location = item.TryGetProperty("location", out var location) ? location.GetString() ?? string.Empty : string.Empty,
                                            IsOnline = item.TryGetProperty("isOnline", out var isOnline) && isOnline.GetBoolean(),
                                            TeacherId = item.TryGetProperty("teacherId", out var teacherId) ? 
                                                Guid.Parse(teacherId.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                                            TeacherName = item.TryGetProperty("teacherName", out var teacherName) ? teacherName.GetString() ?? string.Empty : string.Empty
                                        };
                                        
                                        consultations.Add(consultation);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Error converting JSON item to ConsultationDto");
                                    }
                                }
                            }
                            
                            return consultations;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing teacher consultations JSON: {Content}", content);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to get teacher consultations: {StatusCode}", response.StatusCode);
                }
                
                return new List<ConsultationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teacher consultations");
                return new List<ConsultationDto>();
            }
        }
        
        public async Task<IEnumerable<ConsultationDto>> GetStudentConsultationsAsync()
        {
            try
            {
                // Use manual HTTP call to handle custom JSON format
                var response = await _httpClient.GetAsync("api/Consultations/student");
                
                if (response.IsSuccessStatusCode)
                {
                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Received successful response from student consultations endpoint: {ContentLength} bytes", content.Length);
                    
                    if (!string.IsNullOrEmpty(content))
                    {
                        try
                        {
                            // First try to parse as dynamic objects
                            var dynamicResult = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(content, options);
                            
                            // Convert dynamic objects to ConsultationDto
                            var consultations = new List<ConsultationDto>();
                            
                            if (dynamicResult != null)
                            {
                                foreach (var item in dynamicResult)
                                {
                                    try
                                    {
                                        var consultation = new ConsultationDto
                                        {
                                            Id = item.TryGetProperty("id", out var idProp) ? Guid.Parse(idProp.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                                            Title = item.TryGetProperty("title", out var title) ? title.GetString() ?? string.Empty : string.Empty,
                                            Topic = item.TryGetProperty("topic", out var topic) ? topic.GetString() ?? string.Empty : string.Empty,
                                            Description = item.TryGetProperty("description", out var desc) ? desc.GetString() ?? string.Empty : string.Empty,
                                            ScheduledAt = item.TryGetProperty("scheduledAt", out var scheduled) ? 
                                                DateTime.Parse(scheduled.GetString() ?? DateTime.UtcNow.ToString("o")) : DateTime.UtcNow,
                                            DurationMinutes = item.TryGetProperty("durationMinutes", out var duration) ? duration.GetInt32() : 0,
                                            Status = item.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty,
                                            Location = item.TryGetProperty("location", out var location) ? location.GetString() ?? string.Empty : string.Empty,
                                            IsOnline = item.TryGetProperty("isOnline", out var isOnline) && isOnline.GetBoolean(),
                                            TeacherId = item.TryGetProperty("teacherId", out var teacherId) ? 
                                                Guid.Parse(teacherId.GetString() ?? Guid.Empty.ToString()) : Guid.Empty,
                                            TeacherName = item.TryGetProperty("teacherName", out var teacherName) ? teacherName.GetString() ?? string.Empty : string.Empty
                                        };
                                        
                                        consultations.Add(consultation);
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Error converting JSON item to ConsultationDto");
                                    }
                                }
                            }
                            
                            return consultations;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing student consultations JSON: {Content}", content);
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to get student consultations: {StatusCode}", response.StatusCode);
                }
                
                return new List<ConsultationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching student consultations");
                return new List<ConsultationDto>();
            }
        }
    }
    
    // Review DTO for submitting consultation reviews
    public class ReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
