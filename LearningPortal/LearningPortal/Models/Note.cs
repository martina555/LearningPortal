using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class Note :BaseClass
    {
        private DateTime _created = DateTime.Now;
        public DateTime Created {
            get
            {
                return this._created;
            }
        }

        public string TextArea { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Deadline { get; set; }

        //note has one user
        public string UserId { get; set; } 
        public virtual ApplicationUser User { get; set; }
    }
}
