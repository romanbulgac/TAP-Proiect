namespace BusinessLayer.DTOs
{
    public class MaterialDto
    {
        public Guid Id { get; set; }               // <-- Added
        public string Title { get; set; } = string.Empty;
        public string ResourceUri { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}