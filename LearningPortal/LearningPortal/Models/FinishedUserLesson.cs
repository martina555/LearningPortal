using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class FinishedUserLesson
    {
        public int LessonId { get; set; }

        public string UserId { get; set; }

        public int CourseId { get; set; }
    }
}
