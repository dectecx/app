using System.Collections.Generic;

namespace WebApplication1.DTOs
{
    public class WorkItemStateDto
    {
        public int ItemId { get; set; }
        public bool IsChecked { get; set; }
    }

    public class ConfirmStateDto
    {
        public List<WorkItemStateDto>? States { get; set; }
    }
}
