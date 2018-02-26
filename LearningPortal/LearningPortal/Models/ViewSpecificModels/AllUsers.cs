using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models.ViewSpecificModels
{
    public class AllUsers
    {
        public IEnumerable<ApplicationUser> Students { get; set; }
        public IEnumerable<ApplicationUser> NonStudents { get; set; }
    }
}
