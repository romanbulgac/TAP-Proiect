namespace BusinessLayer.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public Guid ConsultationId { get; set; }
        public Guid StudentId { get; set; }
    }
}
