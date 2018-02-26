using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class HomeBigModel
    {
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Lesson> Lessons { get; set; }
        public int TotalLessons { get; set; }
        public int TotalCourses { get; set; }
    }
}
