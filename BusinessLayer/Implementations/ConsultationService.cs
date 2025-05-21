using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccess;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BusinessLayer.Implementations
{
    public class ConsultationService : IConsultationService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        
        public ConsultationService(MyDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> CreateConsultationAsync(ConsultationDto dto)
        {
            var newConsultation = _mapper.Map<Consultation>(dto);
            
            _context.Consultations.Add(newConsultation);
            await _context.SaveChangesAsync();
            return newConsultation.Id;
        }

        public async Task<ConsultationDto?> GetConsultationByIdAsync(Guid consultationId)
        {
            var consultation = await _context.Consultations
                .Include(c => c.Teacher)
                .Include(c => c.Materials)
                .Include(c => c.StudentLinks)
                    .ThenInclude(sl => sl.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == consultationId);
                
            if (consultation == null) return null;
            
            return _mapper.Map<ConsultationDto>(consultation);
        }

        public async Task<IEnumerable<ConsultationDto>> GetAllConsultationsAsync()
        {
            var consultations = await _context.Consultations
                .Include(c => c.Teacher)
                .Include(c => c.Materials)
                .Include(c => c.StudentLinks)
                    .ThenInclude(sl => sl.Student)
                .AsNoTracking()
                .ToListAsync();
                
            return _mapper.Map<IEnumerable<ConsultationDto>>(consultations);
        }

        public async Task BookConsultationAsync(Guid consultationId, Guid studentId)
        {
            var consultation = await _context.Consultations.FindAsync(consultationId);
            if (consultation == null)
                throw new KeyNotFoundException($"Consultation with ID {consultationId} not found.");
                
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
                
            var existingBooking = await _context.ConsultationStudents
                .AnyAsync(cs => cs.ConsultationId == consultationId && cs.StudentId == studentId);
                
            if (existingBooking)
                return; 
                
            var entry = new ConsultationStudent
            {
                ConsultationId = consultationId,
                StudentId = studentId,
                Attended = false
            };
            
            _context.ConsultationStudents.Add(entry);
            await _context.SaveChangesAsync();
        }
        public async Task CompleteConsultationAsync(Guid consultationId)
        {
            var consultation = await _context.Consultations.FindAsync(consultationId);
            if (consultation == null)
                throw new KeyNotFoundException($"Consultation with ID {consultationId} not found.");
                
            consultation.ConsultationStatus = "Completed";
            await _context.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<ConsultationDto>> GetTeacherConsultationsAsync(Guid teacherId)
        {
            var consultations = await _context.Consultations
                .Include(c => c.Teacher)
                .Include(c => c.Materials)
                .Include(c => c.StudentLinks)
                    .ThenInclude(sl => sl.Student)
                .Where(c => c.TeacherId == teacherId)
                .AsNoTracking()
                .ToListAsync();
                
            return _mapper.Map<IEnumerable<ConsultationDto>>(consultations);
        }
        
        public async Task<IEnumerable<ConsultationDto>> GetStudentConsultationsAsync(Guid studentId)
        {
            var consultations = await _context.Consultations
                .Include(c => c.Teacher)
                .Include(c => c.Materials)
                .Include(c => c.StudentLinks)
                    .ThenInclude(sl => sl.Student)
                .Where(c => c.StudentLinks.Any(sl => sl.StudentId == studentId))
                .AsNoTracking()
                .ToListAsync();
                
            return _mapper.Map<IEnumerable<ConsultationDto>>(consultations);
        }
        public async Task CancelConsultationAsync(Guid consultationId)
        {
            var cons = await _context.Consultations.FirstOrDefaultAsync(c => c.Id == consultationId);
            if (cons != null)
            {
                cons.ConsultationStatus = "Cancelled";
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateConsultationAsync(Guid consultationId, ConsultationDto dto)
        {
            var consultation = await _context.Consultations.FindAsync(consultationId);
            if (consultation == null)
                throw new KeyNotFoundException($"Consultation with ID {consultationId} not found.");

            // Update properties
            consultation.Topic = dto.Topic;
            consultation.Description = dto.Description;
            consultation.ScheduledAt = dto.ScheduledAt;
            consultation.DurationMinutes = dto.DurationMinutes;
            consultation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteConsultationAsync(Guid consultationId)
        {
            var consultation = await _context.Consultations.FindAsync(consultationId);
            if (consultation != null)
            {
                consultation.IsDeleted = true;
                consultation.DeletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        // Add a method to get all entities including deleted ones
        public async Task<IEnumerable<ConsultationDto>> GetAllConsultationsIncludingDeletedAsync()
        {
            var consultations = await _context.Consultations
                .IgnoreQueryFilters() // This bypasses the soft delete filter
                .Include(c => c.Teacher)
                .AsNoTracking()
                .ToListAsync();
                
            return _mapper.Map<IEnumerable<ConsultationDto>>(consultations);
        }

        public async Task<bool> IsStudentRegisteredForConsultationAsync(Guid consultationId, Guid studentId)
        {
            // Check if there is a link between the consultation and the student
            return await _context.ConsultationStudents
                .AnyAsync(cs => cs.ConsultationId == consultationId && cs.StudentId == studentId);
        }
    }
}
