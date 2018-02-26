using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class StudentDetail
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<CourseUser> CourseUser { get; set; }
    }
}
