namespace BusinessLayer.Interfaces
{
    public interface IConsultationService
    {
        // Create a new consultation
        Guid CreateConsultation(DTOs.ConsultationDto dto);

        // Retrieve consultation by ID
        DTOs.ConsultationDto? GetConsultationById(Guid consultationId);

        // List or filter consultations
        IEnumerable<DTOs.ConsultationDto> GetAllConsultations();

        // Book a consultation for a student
        void BookConsultation(Guid consultationId, Guid studentId);

        // Cancel a consultation
        void CancelConsultation(Guid consultationId);
    }
}
