using Microsoft.EntityFrameworkCore;
namespace StudyPlanner.Models
{


    [PrimaryKey(nameof(AssignmentId), nameof(TagId))]
    public class AssignmentTag
    {
        public int AssignmentId { get; set; }
        public int TagId { get; set; }

        public Assignment Assignment { get; set; }
        public Tag Tag { get; set; }
    }
}
