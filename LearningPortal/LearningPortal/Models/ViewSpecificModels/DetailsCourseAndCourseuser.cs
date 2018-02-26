using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class DetailsCourseAndCourseuser
    {
        public Course Course { get; set; }
        public CourseUser CourseUser { get; set; }
        public IEnumerable<ApplicationUser> Users{ get; set; }
    }
}
