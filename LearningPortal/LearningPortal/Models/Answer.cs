using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class Answer : BaseClass
    {
        [DisplayName("True")]
        public bool Verity { get; set; }

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}
