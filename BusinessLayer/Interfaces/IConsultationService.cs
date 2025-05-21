using BusinessLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IConsultationService
    {
        Task<Guid> CreateConsultationAsync(ConsultationDto dto); // Was CreateConsultation
        Task<ConsultationDto?> GetConsultationByIdAsync(Guid consultationId); // Was GetConsultationById
        Task<IEnumerable<ConsultationDto>> GetAllConsultationsAsync(); // Was GetAllConsultations
        Task BookConsultationAsync(Guid consultationId, Guid studentId); // Was BookConsultation
        Task CompleteConsultationAsync(Guid consultationId); // Was CompleteConsultation
        Task UpdateConsultationAsync(Guid consultationId, ConsultationDto dto); // New method
        Task<IEnumerable<ConsultationDto>> GetTeacherConsultationsAsync(Guid teacherId); // Was GetTeacherConsultations
        Task<IEnumerable<ConsultationDto>> GetStudentConsultationsAsync(Guid studentId); // Was GetStudentConsultations
        Task CancelConsultationAsync(Guid consultationId); // Was CancelConsultation
        Task SoftDeleteConsultationAsync(Guid consultationId);
        Task<IEnumerable<ConsultationDto>> GetAllConsultationsIncludingDeletedAsync();
        Task<bool> IsStudentRegisteredForConsultationAsync(Guid consultationId, Guid studentId); // New method
    }
}
