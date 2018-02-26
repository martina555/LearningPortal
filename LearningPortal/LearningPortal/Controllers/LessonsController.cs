using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearningPortal.Data;
using LearningPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Text;
using LearningPortal.Extensions;

namespace LearningPortal.Controllers
{
    [Authorize]
    public class LessonsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserExtensions _userExtensions;

        public LessonsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, IUserExtensions userExtensions)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userExtensions = userExtensions;
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        // GET: Lessons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Lesson.Include(l => l.Course);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Lessons/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lesson
                .Include(l => l.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        [Authorize]
        public async Task<IActionResult> DetailsFinishedLesson(int? id)
        {
            LessonUserAndLesson model = new LessonUserAndLesson();

            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();
            string userId = user.Id;

            model.FinishedUserLesson = _context.FinishedUserLesson.Where(x => x.LessonId == id && x.UserId == userId).FirstOrDefault();
            model.Lesson = await _context.Lesson.Include(l => l.Course).SingleOrDefaultAsync(m => m.Id == id);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteLesson(int id)
        {
            var user = await GetCurrentUserAsync();
            string userId = user.Id;
            int courseid = _context.Lesson.Where(x => x.Id == id).Select(x => x.Course.Id).FirstOrDefault();

            // pridani poctu hotovych lekci do courseuser
            CourseUser originalCourse = await _context.CourseUser.Where(x => x.CourseId == courseid && x.UserId == userId).FirstOrDefaultAsync();
            originalCourse.NumberOfFinishedLessons = originalCourse.NumberOfFinishedLessons + 1;

            //pridani hotovych lekci do FinishedUserLesson
            var lessonId = _context.Lesson.Where(x => x.Id == id).Select(x => x.Id).FirstOrDefault();

            //pridani role FinishedLesson
            await _userExtensions.CreateRoleAddToUser("FinishedLesson" + id, user);

            FinishedUserLesson polozkaLessonUser = new FinishedUserLesson();
            polozkaLessonUser.LessonId = lessonId;
            polozkaLessonUser.UserId = userId;
            polozkaLessonUser.CourseId = originalCourse.CourseId;

            _context.Update(originalCourse);
            _context.Add(polozkaLessonUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsFinishedLesson", "Lessons", new { id = lessonId });
        }

        public IActionResult Create(int id)
        {
            Lesson lesson = new Lesson();
            lesson.CourseId = _context.Course.Where(x => x.Id == id).Select(x => x.Id).FirstOrDefault();

            return View(lesson);
        }

        // POST: Lessons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,CourseId,Video,Name")] Lesson lesson)
        {
            if (ModelState.IsValid)
            {
                StringBuilder embedVideo = new StringBuilder(lesson.Video);
                embedVideo.Replace("watch?v=", "embed/");
                lesson.Video = embedVideo.ToString();

                _context.Add(lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", lesson);
            }
            return View(lesson);
        }

        // GET: Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lesson.Include(x=>x.Course).SingleOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }
            return View(lesson);
        }

        // POST: Lessons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Description,CourseId,Video,Id,Name")] Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LessonExists(lesson.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", lesson);
            }
            return View(lesson);
        }

        // GET: Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesson = await _context.Lesson
                .Include(l => l.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lesson == null)
            {
                return NotFound();
            }

            return View(lesson);
        }

        // POST: Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lesson.SingleOrDefaultAsync(m => m.Id == id);
            _context.Lesson.Remove(lesson);
            await _context.SaveChangesAsync();

            var courseId = lesson.CourseId;
            if(User.IsInRole("StudentCourse" + courseId))
            {
                return RedirectToAction("DetailsCourseAndCourseUser", "Courses", new { Id = courseId});
            }
            else
            {
                return RedirectToAction("Details", "Courses", new { Id = courseId });
            }
        }

        private bool LessonExists(int id)
        {
            return _context.Lesson.Any(e => e.Id == id);
        }
    }
}



