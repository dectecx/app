namespace WebApplication1.Models
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public ICollection<UserWorkItemState>? UserWorkItemStates { get; set; }
    }
}
