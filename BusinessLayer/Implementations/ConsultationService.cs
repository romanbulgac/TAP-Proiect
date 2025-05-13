using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccess;
using DataAccess.Models;

namespace BusinessLayer.Implementations
{
    public class ConsultationService : IConsultationService
    {
        private readonly MyDbContext _context;
        public ConsultationService(MyDbContext context) => _context = context;

        public Guid CreateConsultation(ConsultationDto dto)
        {
            // ...existing code to map DTO to entity...
            var newConsultation = new Consultation
            {
                Topic = dto.Topic,
                ScheduledAt = dto.ScheduledAt,
                DurationMinutes = dto.DurationMinutes,
                ConsultationLink = dto.ConsultationLink,
                TeacherId = dto.TeacherId
                // ...other fields...
            };
            _context.Consultations.Add(newConsultation);
            _context.SaveChanges();
            return newConsultation.Id;
        }

        public ConsultationDto? GetConsultationById(Guid consultationId)
        {
            var consultation = _context.Consultations
                .Where(c => c.Id == consultationId)
                .FirstOrDefault();
            if (consultation == null) return null;
            // map entity to DTO
            return new ConsultationDto
            {
                Id = consultation.Id,
                Topic = consultation.Topic,
                ScheduledAt = consultation.ScheduledAt,
                DurationMinutes = consultation.DurationMinutes
            };
        }

        public IEnumerable<ConsultationDto> GetAllConsultations()
        {
            // Simple retrieval with LINQ
            return _context.Consultations
                .Select(c => new ConsultationDto
                {
                    Id = c.Id,
                    Topic = c.Topic,
                    ScheduledAt = c.ScheduledAt,
                    DurationMinutes = c.DurationMinutes
                })
                .ToList();
        }

        public void BookConsultation(Guid consultationId, Guid studentId)
        {
            var entry = new ConsultationStudent
            {
                ConsultationId = consultationId,
                StudentId = studentId,
                Attended = false
            };
            _context.ConsultationStudents.Add(entry);
            _context.SaveChanges();
        }

        public void CancelConsultation(Guid consultationId)
        {
            var cons = _context.Consultations.FirstOrDefault(c => c.Id == consultationId);
            if (cons != null)
            {
                cons.ConsultationStatus = Consultation.Status.Cancelled;
                _context.SaveChanges();
            }
        }
    }
}
