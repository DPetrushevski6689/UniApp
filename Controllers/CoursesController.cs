using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniApp.Data;
using UniApp.Models;
using UniApp.ViewModelsEdit;
using UniApp.ViewModelsFilter;
using UniApp.ViewModelsFunctionalities;

namespace UniApp.Controllers
{
    public class CoursesController : Controller
    {
        private readonly UniAppContext _context;

        public CoursesController(UniAppContext context)
        {
            _context = context;
        }

        // GET: Courses
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchTitle, string searchProgramme)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();

            if(!string.IsNullOrEmpty(searchTitle))
            {
                courses = courses.Where(c => c.Title.Contains(searchTitle));
            }
            if (!string.IsNullOrEmpty(searchProgramme))
            {
                courses = courses.Where(c => c.Programme.Contains(searchProgramme));
            }

            courses = courses.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher).Include(c => c.Students)
                .ThenInclude(c => c.Student);

            //var uniAppContext = _context.Course.Include(c => c.FirstTeacher).Include(c => c.SecondTeacher);

            var viewmodel = new CoursesFilter
            {
                Courses =await courses.ToListAsync()
            };
            return View(viewmodel);
        }

        // GET: Courses/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .Include(c => c.Students).ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName");
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Credits,Semester,Programme,EducationLevel,FirstTeacherId,SecondTeacherId")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", course.SecondTeacherId);
            return View(course);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Course.Where(c => c.Id == id).Include(c => c.Students).ThenInclude(c => c.Student).First();

            if (course == null)
            {
                return NotFound();
            }

            CoursesStudentsEditClass viewmodel = new CoursesStudentsEditClass
            {
                Course = course,
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName),"Id","FirstName"),
                SelectedStudents = course.Students.Select(s => s.StudentId)
            };

            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", course.SecondTeacherId);
            return View(viewmodel);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CoursesStudentsEditClass viewmodel)
        {
            if (id != viewmodel.Course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    IEnumerable<int> listStudents = viewmodel.SelectedStudents;

                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id)
                        .Select(s => s.StudentId);

                    IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                    foreach(int studentId in newStudents)
                    {
                        _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id });
                    }

                    _context.Update(viewmodel.Course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.Course.Id))
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
            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.SecondTeacherId);
            return View(viewmodel);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .Include(c => c.FirstTeacher)
                .Include(c => c.SecondTeacher)
                .Include(c => c.Students).ThenInclude(c => c.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Course.FindAsync(id);
            _context.Course.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollStudents(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var course = _context.Course.Where(c => c.Id == id).Include(c => c.FirstTeacher).Include(c => c.SecondTeacher)
                .Include(c => c.Students).ThenInclude(c => c.Student).First();

            if(course == null)
            {
                return NotFound();
            }

            CoursesEnrollStudentsClass viewmodel = new CoursesEnrollStudentsClass
            {
                Course = course,
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FirstName"),
                SelectedStudents = course.Students.Select(s => s.StudentId)
            };

            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.SecondTeacherId);
            return View(viewmodel);
        }

        [HttpPost, ActionName("EnrollStudents")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollStudents(int id,int inputYear,string inputSemester, CoursesEnrollStudentsClass viewmodel)
        {
            if(id != viewmodel.Course.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    IEnumerable<int> listStudents = viewmodel.SelectedStudents;

                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId)
                     && s.CourseId == id).Include(s => s.Course).Include(s => s.Student);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId)
                    
                    && s.CourseId == id).Include(s => s.Course).Include(s => s.Student)
                    .Select(s => s.StudentId); 

                    IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                    
                    foreach (int studentId in newStudents)
                    {
                        _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id, Year = inputYear, Semester = inputSemester});
                    }

                    _context.Update(viewmodel.Course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.Course.Id))
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

            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.SecondTeacherId);
            return View(viewmodel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnrollStudents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = _context.Course.Where(c => c.Id == id).Include(c => c.FirstTeacher).Include(c => c.SecondTeacher)
                .Include(c => c.Students).ThenInclude(c => c.Student).First();

            if (course == null)
            {
                return NotFound();
            }

            CoursesUnrollStudentsClass viewmodel = new CoursesUnrollStudentsClass
            {
                Course = course,
                StudentList = new MultiSelectList(_context.Student.OrderBy(s => s.FirstName), "Id", "FirstName"),
                SelectedStudents = course.Students.Select(s => s.StudentId)
            };

            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.SecondTeacherId);
            return View(viewmodel);
        }

        [HttpPost, ActionName("UnrollStudents")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnrollStudents(int id, DateTime inputFinishDate, CoursesUnrollStudentsClass viewmodel)
        {
            if (id != viewmodel.Course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    IEnumerable<int> listStudents = viewmodel.SelectedStudents;

                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId)
                     && s.CourseId == id).Include(s => s.Student).Include(s => s.Course);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId)
                    && s.CourseId == id).Include(s => s.Student).Include(s => s.Course)
                    .Select(s => s.StudentId);

                    IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));


                    foreach (int studentId in newStudents)
                    {
                        _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id, FinishDate = inputFinishDate });
                    }

                    _context.Update(viewmodel.Course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(viewmodel.Course.Id))
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

            ViewData["FirstTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.FirstTeacherId);
            ViewData["SecondTeacherId"] = new SelectList(_context.Set<Teacher>(), "Id", "FirstName", viewmodel.Course.SecondTeacherId);
            return View(viewmodel);
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}
