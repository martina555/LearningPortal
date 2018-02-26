using LearningPortal.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LearningPortal.Models
{
    public class CourseUser
    {

        public int CourseId { get; set; }
        public string UserId { get; set; }
        public Course Course { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int NumberOfFinishedLessons { get; set; }
        public int NumberOfTests { get; set; }
        public double ResultPercent { get; set; }
        public bool Finished { get; set; }
    }

    
}
