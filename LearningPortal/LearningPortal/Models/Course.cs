using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LearningPortal.Models
{
    public class Course : BaseClass
    {
        public Course()
        {         
            CourseUsers = new Collection<CourseUser>();
            Lessons = new Collection<Lesson>();
        }
        [Required]
        public string Description { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
        // one course to many
        public virtual ICollection<Lesson> Lessons { get; set; }

        public int NumberOfLessons
        {
            get
            {
                if(Lessons.Any())
                return Lessons.Count;
                return 0;
            }
        }
   
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        // many to many
        public ICollection<CourseUser> CourseUsers { get; set; }
    }
}

