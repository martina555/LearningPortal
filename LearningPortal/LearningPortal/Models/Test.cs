using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class Test : BaseClass
    {
        public virtual ICollection<Question> Questions { get; set; }

        //public int Result { get; set; }
        //public double ResultPercent { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }
    }
}
