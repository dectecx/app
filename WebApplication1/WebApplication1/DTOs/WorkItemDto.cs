namespace WebApplication1.DTOs
{
    public class WorkItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }

    public class CreateWorkItemDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class UpdateWorkItemDto
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }
}
