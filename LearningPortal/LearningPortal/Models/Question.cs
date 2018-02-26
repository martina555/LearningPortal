using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class Question : BaseClass
    {
        public virtual ICollection<Answer> Answers { get; set; }

        public int SelectedAnswer { set; get; }
        public int TrueAnswer { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
        public virtual Test Test { get; set; }
    }
}
