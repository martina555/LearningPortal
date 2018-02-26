using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class UsersFromCourse
    {
        public Course Course { get; set; }
        public IEnumerable<ApplicationUser> User { get; set; }
    }
}
