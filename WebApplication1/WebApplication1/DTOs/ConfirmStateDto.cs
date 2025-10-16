using System.Collections.Generic;

namespace WebApplication1.DTOs
{
    public class WorkItemStateDto
    {
        public int WorkItemId { get; set; }
        public bool IsConfirmed { get; set; }
    }

    public class ConfirmStateDto
    {
        public List<WorkItemStateDto>? States { get; set; }
    }
}
