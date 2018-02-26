using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LearningPortal.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        
        public ApplicationUser():base()
        {
            CourseUsers = new Collection<CourseUser>();
            FinishedUserLessons = new Collection<FinishedUserLesson>();
        }

        public ApplicationUser(string userName) : base(userName)
        {
            base.Email = userName;        
        }

        //many to many
        public ICollection<CourseUser> CourseUsers { get; set; }
        public ICollection<FinishedUserLesson> FinishedUserLessons { get; set; }

        //one to many note
        public virtual ICollection<Note> Notes { get; set; }

    }
}
