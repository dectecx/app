namespace WebApplication1.Models
{
    public class UserWorkItemState
    {
        public int StateId { get; set; }
        public int UserId { get; set; }
        public int WorkItemId { get; set; }
        public bool IsChecked { get; set; }
        public bool IsConfirmed { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public User? User { get; set; }
        public WorkItem? WorkItem { get; set; }
    }
}
