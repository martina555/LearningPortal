using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LearningPortal.Data;
using LearningPortal.Models;
using LearningPortal.Extensions;

namespace LearningPortal.Controllers
{
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Test.Include(t => t.Course);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Test.Include(x => x.Questions)
                .Include(t => t.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // GET: Tests/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Name");
            return View();
        }

        // POST: Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Result,CourseId,Id,Name")] Test test)
        {
            if (ModelState.IsValid)
            {
                _context.Add(test);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Name", test.CourseId);
            return View(test);
        }

        // GET: Tests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Test.SingleOrDefaultAsync(m => m.Id == id);
            if (test == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Name", test.CourseId);
            return View(test);
        }

        // POST: Tests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Result,CourseId,Id,Name")] Test test)
        {
            if (id != test.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(test);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(test.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Course, "Id", "Name", test.CourseId);
            return View(test);
        }

        // GET: Tests/Edit/5
        public async Task<IActionResult> Test(int? id)
        {
            Random random = new Random();
            TestBigModel model = new TestBigModel();

            List<Question> AllQuestions = await _context.Question.Include(x => x.Answers).Where(x => x.Course.Id == id).ToListAsync(); //Všechny otázky, které náleží danému kurzu includujeme také answers

            List<TestQuestion> testQuestions = new List<TestQuestion>(); //Vytvoříme si zatím prázdný list Test Questions a později ho otázku po otázce inicializujeme

            for (int i = 0; i < AllQuestions.Count; i++)  //Pro každou z otázek, které se nachází v Listu AllQuestions
            {
                TestQuestion testQuestion = new TestQuestion(); //Dočasý objekt TestQuestion existující pouze ve scope tohoto foreach loopu (při každém scope se vytváří znovu a znovu, dá se říct že se resetuje) 
                testQuestion.Question = AllQuestions[i];
                testQuestion.Answers = AllQuestions[i].Answers.ToList();
                testQuestion.Answers.Shuffle();
                
                testQuestions.Add(testQuestion); //Nakonec po inicializaci všech vlastností vložíme dočasný objekt do listu testQuestions
            }

            model.TestQuestions = testQuestions;
            model.TestQuestions.Shuffle();

            return View(model); //Nakonec list odesíláme do View jako model
        }

        // POST: Tests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Test(TestBigModel model)
        {
            int courseId = model.TestQuestions.FirstOrDefault().Question.CourseId;
            int totalQuestions = model.TestQuestions.Count();
            int totalTrueAnswers = 0;

            foreach (var item in model.TestQuestions)
            {
                int trueAnswer = await _context.Answer.Where(x => x.QuestionId == item.Question.Id && x.Verity == true).Select(x => x.Id).FirstOrDefaultAsync();
                if (item.Question.SelectedAnswer == trueAnswer)
                    totalTrueAnswers = totalTrueAnswers + 1;
            }

            double percent = ((double)totalTrueAnswers / totalQuestions) * 100;

            TestResult testResultModel = new TestResult();
            testResultModel.CourseId = courseId;
            testResultModel.Percent = percent;

            //zobrazeni vysledku testu
            CourseUser courseToAddResult = await _context.CourseUser.Where(x => x.CourseId == courseId).FirstOrDefaultAsync();
            courseToAddResult.ResultPercent = percent;
            courseToAddResult.NumberOfTests = courseToAddResult.NumberOfTests + 1;
            _context.Update(courseToAddResult);
            await _context.SaveChangesAsync();

            if (percent >= 80)
            {
                courseToAddResult.Finished = true;
                _context.Update(courseToAddResult);
                await _context.SaveChangesAsync();

                return View("TestSuccess", testResultModel);
            }
            return View("TestFailure", testResultModel);
        }




        // GET: Tests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Test
                .Include(t => t.Course)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // POST: Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _context.Test.SingleOrDefaultAsync(m => m.Id == id);
            _context.Test.Remove(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestExists(int id)
        {
            return _context.Test.Any(e => e.Id == id);
        }
    }
}
