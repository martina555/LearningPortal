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
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using LearningPortal.Extensions;

namespace LearningPortal.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserExtensions _userExtensions;

        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
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

        // GET: Courses
        public IActionResult Index()
        {
            HomeBigModel model = new HomeBigModel();
            model.Courses = _context.Course.Include(x => x.Lessons).OrderByDescending(x => x.Created);

            return View(model);
        }

        [Authorize]
        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await GetCurrentUserAsync();
            string userId = user.Id;

            if (id == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("StudentCourse" + id))
            {
                var model = new UsersFromCourse();
                model.User = await _userManager.GetUsersInRoleAsync("StudentCourse" + id);
                model.Course = await _context.Course.Include(x => x.Lessons).Include(y => y.Questions).Include(z => z.Tests)
                    .SingleOrDefaultAsync(m => m.Id == id);

                if (model == null)
                {
                    return NotFound();
                }

                return View(model);
            }
            else
            {
                var model = new DetailsCourseAndCourseuser();
                model.CourseUser = _context.CourseUser.Where(x => x.CourseId == id && x.UserId == userId).FirstOrDefault();
                model.Course = await _context.Course.Include(x => x.Lessons).Include(y => y.Questions).Include(z => z.Tests).SingleOrDefaultAsync(m => m.Id == id);
                model.Users =  _userManager.GetUsersInRoleAsync("StudentCourse" + id).Result.ToList();
                
                return View("DetailsCourseAndCourseUser", model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // po kliknuti na startstudy ze z uzivatele stane student
        public async Task<IActionResult> IsStudent(int? id)
        {
            ApplicationUser user = await GetCurrentUserAsync();

            await _userExtensions.CreateRoleAddToUser("StudentCourse" + id, user); //student kurzu
            await _userExtensions.CreateRoleAddToUser("Student", user); // pokud studuje alespon jeden kurz

            // seznam kurzu studenta
            var currentCourseId = await _context.Course.Where(x => x.Id == id).Select(x => x.Id).FirstOrDefaultAsync();
            var userid = user.Id;
            CourseUser model = new CourseUser();
            model.UserId = userid;
            model.CourseId = currentCourseId;

            var courseUserList = _context.CourseUser.Add(model);
            _context.Update(user);
            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { Id = id });
        }

        public async Task<IActionResult> Search (string search)
        {
            HomeBigModel model = new HomeBigModel();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
               
                model.Courses = _context.Course.Where(x => x.Name.ToLower().Contains(search));
                model.Lessons = _context.Lesson.ToList();
                model.TotalLessons = await _context.Lesson.CountAsync();
                model.TotalCourses = await _context.Course.CountAsync();

                return PartialView("_CoursesList", model);
            }
            model.Courses = _context.Course.ToList();
            model.Lessons = _context.Lesson.ToList();
            model.TotalLessons = await _context.Lesson.CountAsync();
            model.TotalCourses = await _context.Course.CountAsync();

            return PartialView("_CoursesList", model);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Created,NumberOfLessons,Name")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { Id = course.Id });
            }
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course.Include(x=>x.Lessons).Include(x=>x.CourseUsers).Include(x=>x.Questions).SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Description,Created,NumberOfLessons,Id,Name")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", course);
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .SingleOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.SingleOrDefaultAsync(m => m.Id == id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
