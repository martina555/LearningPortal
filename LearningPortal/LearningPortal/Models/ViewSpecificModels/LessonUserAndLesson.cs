using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class LessonUserAndLesson
    {
        public Lesson Lesson { get; set; }
        public FinishedUserLesson FinishedUserLesson { get; set; }
    }
}
