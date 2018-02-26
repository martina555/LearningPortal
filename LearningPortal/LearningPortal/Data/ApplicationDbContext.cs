using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LearningPortal.Models;
using LearningPortal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LearningPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<LearningPortal.Models.Course> Course { get; set; }

        public DbSet<LearningPortal.Models.Lesson> Lesson { get; set; }

        public DbSet<LearningPortal.Models.Question> Question { get; set; }

        public DbSet<LearningPortal.Models.Answer> Answer { get; set; }

        public DbSet<LearningPortal.Models.Test> Test { get; set; }
        
        public DbSet<CourseUser> CourseUser { get; set; }

        public DbSet<FinishedUserLesson> FinishedUserLesson { get; set; }

        public DbSet<LearningPortal.Models.ApplicationUser> ApplicationUser { get; set; }

        public DbSet<LearningPortal.Models.Note> Note { get; set; }


        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            // one to many questions
            modelbuilder.Entity<Question>()
                 .HasOne(e => e.Test)
                 .WithMany(g => g.Questions);

            // many to many users - courses
            modelbuilder.Entity<CourseUser>()
                .HasKey(uc => new { uc.CourseId, uc.UserId });

            // many to many users - lessons
            modelbuilder.Entity<FinishedUserLesson>()
                .HasKey(ul => new { ul.LessonId, ul.UserId });

            // one to many user - notes
            modelbuilder.Entity<Note>()
                 .HasOne(e => e.User)
                 .WithMany(g => g.Notes);

            base.OnModelCreating(modelbuilder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
    
    public static class DbInitializer
    {
            public static void Initialize(ApplicationDbContext context)
            {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Random random = new Random();

            // ---------------- Courses -----------------------------
            var courses = new List<Course>();
            int minNumberOfCourses = 50;
            for (int i = 1; i < minNumberOfCourses; i++)
            {
                courses.Add(new Course()
                {
                    Name = "Kurz cislo " + i,
                    Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Praesent id justo in neque elementum ultrices. Integer lacinia. Phasellus enim erat, vestibulum vel, aliquam a, posuere eu, velit. Phasellus rhoncus. Mauris tincidunt sem sed arcu.",
                    Created = DateTime.Now,
                });
            }
            context.Course.AddRange(courses);


            // ---------------- Lessons -----------------------------
            var lessons = new List<Lesson>();
            int minNumberOfLessons = 5;
            int maxNumberOfLessons = 15;
            int lessonNumber = 1;
            foreach (var course in courses) 
            {
                int increaseRepetetionRandomly = random.Next(0, maxNumberOfLessons);
                for (int repeats = 1; repeats < minNumberOfLessons + increaseRepetetionRandomly; repeats++)
                {
                    lessons.Add(new Lesson()
                    {
                        Name = "Lekce cislo" + lessonNumber,
                        Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Praesent id justo in neque elementum ultrices. Integer lacinia. Phasellus enim erat, vestibulum vel, aliquam a, posuere eu, velit. Phasellus rhoncus. Mauris tincidunt sem sed arcu.",
                        Video = "https://www.youtube.com/embed/lisiwUZJXqQ",
                        //Course = courses[random.Next(0, courses.Count - 1)],
                        Course = course
                });
                    lessonNumber++;
                }
            }
            context.Lesson.AddRange(lessons);

            // ---------------- Questions -----------------------------
            var questions = new List<Question>();
            int minNumberOfQuestions = 15;
            int maxNumberOfQuestions = 5;
            int questionNumber = 1;
            foreach (var course in courses)
            {
                int increaseRepetetionRandomly = random.Next(0, maxNumberOfQuestions);
                for (int repeats = 1; repeats < minNumberOfQuestions + increaseRepetetionRandomly; repeats++)
                {
                    questions.Add(new Question()
                    {
                        Name = "otazka cislo" + questionNumber,
                        //Course = courses[random.Next(0, courses.Count - 1)],
                        Course = course
                });
                    questionNumber++;
                }
            }
            context.Question.AddRange(questions);

            // ---------------- Answers -----------------------------
            var answers = new List<Answer>();
            int minNumberOfAnswers = 4;
            int maxNumberOfAnswers = 4;
            int answerNumber = 1;
            foreach (var question in questions)
            {
                int increaseRepetetionRandomly = random.Next(0, maxNumberOfAnswers);
                for (int repeats = 1; repeats < minNumberOfAnswers + increaseRepetetionRandomly; repeats++)
                {
                    answers.Add(new Answer()
                    {
                        Name = "odpoved cislo" + answerNumber,
                        Verity = false,
                        Question = question,
                    });
                    answerNumber++;
                }
            }
            context.Answer.AddRange(answers);

            context.SaveChanges();



            //Před uložením do databáze nejsou k dispozici ids
            var gropby1 = answers.GroupBy(x => x.QuestionId);   //GroupBy seřaď odpovědi podle id otázky (sekupení)

            var groupedAnswers = answers.GroupBy(x => x.QuestionId).Select(x => new { QuestionId = x.FirstOrDefault().QuestionId, AnswerIds = x.Select(y => y.Id) });

            foreach (var item in groupedAnswers)
            {
                int totalItems = item.AnswerIds.Count() - 1;
                int randomAnswerIndex = random.Next(0, totalItems);
                Answer answerToUpdate = answers.Where(x => x.Id == item.AnswerIds.ToList()[randomAnswerIndex]).FirstOrDefault();
                answerToUpdate.Verity = true;
                context.Update(answerToUpdate);
            }
            context.SaveChanges();
        }
    }
}