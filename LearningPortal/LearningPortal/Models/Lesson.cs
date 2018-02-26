using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class Lesson : BaseClass
    {
        public Lesson()
        {
            FinishedUserLessons = new Collection<FinishedUserLesson>();           
        }

        public string Description { get; set; }
        //lesson has one course
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public string Video { get; set; }

        //many to many
        public ICollection<FinishedUserLesson> FinishedUserLessons { get; set; }
    }
}
