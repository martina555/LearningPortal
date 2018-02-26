using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LearningPortal.Models;
using LearningPortal.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace LearningPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly Logger<LoggerFactory> _logger;
        private ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ILogger<HomeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
        public IActionResult Index()
        {
            HomeBigModel model = new HomeBigModel();
            model.Courses = _context.Course.Include(x => x.Lessons).OrderByDescending(x => x.Created).Take(10);
            model.TotalLessons = _context.Lesson.Count();
            model.TotalCourses = _context.Course.Count();
            
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin()
        {
            var currentUser = await GetCurrentUserAsync();
            if(currentUser != null)
            {
                await _signInManager.SignOutAsync();
            }

            await CreateRole("Admin");

            ApplicationUser userExists = await _userManager.FindByEmailAsync("admin@example.com");
            if (userExists == null)
            {
                var user = new ApplicationUser { UserName = "admin@example.com", Email = "admin@example.com" };
                var result = await _userManager.CreateAsync(user, "Admin@example.com9");
                var role = _userManager.AddToRoleAsync(user, "Admin");
                await _signInManager.RefreshSignInAsync(user);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                await _signInManager.SignInAsync(userExists, isPersistent: false);
                return RedirectToAction("Index");
            }
            return null;
        }

        public async Task<IActionResult> CreateRole(string role)
        {
            IdentityRole roleExists = await _roleManager.FindByNameAsync(role);
            if (roleExists == null)
            {
                var newrole = new IdentityRole { Name = role, NormalizedName = role };
                await _roleManager.CreateAsync(newrole);
            }
            return null;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
