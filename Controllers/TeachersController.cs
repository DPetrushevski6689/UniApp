using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniApp.Data;
using UniApp.ImageDemoViewModels;
using UniApp.Models;
using UniApp.ViewModelsFilter;
using UniApp.ViewModelsFunctionalities;

namespace UniApp.Controllers
{
    public class TeachersController : Controller
    {
        private readonly UniAppContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment webHostEnvironment;

        public TeachersController(UniAppContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment whs)
        {
            _context = context;
            webHostEnvironment = whs;
        }

        // GET: Teachers
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Index(string searchFirstName, string searchLastName, string searchDegree, string searchAcademicRank)
        {
            IQueryable<Teacher> teachers = _context.Teacher.AsQueryable();

            if(!string.IsNullOrEmpty(searchFirstName))
            {
                teachers = teachers.Where(t => t.FirstName.Contains(searchFirstName));
            }
            if (!string.IsNullOrEmpty(searchLastName))
            {
                teachers = teachers.Where(t => t.LastName.Contains(searchLastName));
            }
            if (!string.IsNullOrEmpty(searchDegree))
            {
                teachers = teachers.Where(t => t.Degree.Contains(searchDegree));
            }
            if (!string.IsNullOrEmpty(searchAcademicRank))
            {
                teachers = teachers.Where(t => t.AcademicRank.Contains(searchAcademicRank));
            }

            teachers = teachers.Include(t => t.FirstCourses).Include(t => t.SecondCourses);

            var viewmodel = new TeachersFilter
            {
                Teachers = await teachers.ToListAsync()
            };

            return View(viewmodel);
        }

        // GET: Teachers/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .Include(t => t.FirstCourses).Include(t => t.SecondCourses)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        /**
        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate,ProfilePicture")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }**/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TeachersImageDemoModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                Teacher teacher = new Teacher
                {
                    FirstName = model.Teacher.FirstName,
                    LastName = model.Teacher.LastName,
                    Degree = model.Teacher.Degree,
                    AcademicRank = model.Teacher.AcademicRank,
                    OfficeNumber = model.Teacher.OfficeNumber,
                    HireDate = model.Teacher.HireDate,
                    ProfilePicture = uniqueFileName,
                };

                _context.Teacher.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();

        }

        private string UploadedFile(TeachersImageDemoModel model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


        // GET: Teachers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            var viewmodel = new TeachersImageDemoModel
            {
                Teacher = teacher
            };


            return View(viewmodel);
        }

        /**
        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Degree,AcademicRank,OfficeNumber,HireDate,ProfilePicture")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }**/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, TeachersImageDemoModel model)
        {
            if (id != model.Teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model.Teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(model.Teacher.Id))
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
            return View(model.Teacher);
        }

        // GET: Teachers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .Include(t => t.FirstCourses).Include(t => t.SecondCourses)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> CourseList(int? id /**int searchYear**/)
        {
            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();

            /**if (searchYear != null)
            {
                enrollments = enrollments.Where(e => e.Year == searchYear);
            }**/

            enrollments = enrollments.Where(e => e.CourseId == id).Include(e => e.Student).Include(e => e.Course);

            

            var viewmodel = new CourseListModel
            {
                Enrollments = await enrollments.ToListAsync()
            };

            return View(viewmodel);
        }

        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AddPoints(int? id) //enrollment id
        {
            if(id == null)
            {
                return NotFound();
            }

            var enrollment = _context.Enrollment.Where(e => e.Id == id).Include(e => e.Student).Include(e => e.Course).First();

            AddPointsModel viewmodel = new AddPointsModel
            {
                Enrollment = enrollment
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> AddPoints(int id, AddPointsModel viewmodel)
        {
            if(id == null)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(e => e.Id == id);
                    _context.Enrollment.RemoveRange(toBeRemoved);

                    _context.Enrollment.Add(new Enrollment
                    {
                        StudentId = viewmodel.Enrollment.StudentId,
                        CourseId = viewmodel.Enrollment.CourseId,
                        AdditionalPoints = viewmodel.inputAdditionalPoints,
                        ExamPoints = viewmodel.inputExamPoints,
                        ProjectPoints = viewmodel.inputProjectPoints,
                        SeminarPoints = viewmodel.inputSeminarPoints,
                        Grade = viewmodel.inputGrade,
                        FinishDate = viewmodel.inputFinishDate
                    });

                    _context.Update(viewmodel.Enrollment);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(id))
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
            return View(viewmodel);
        }

        private bool EnrollmentExists(int id)
        {
            throw new NotImplementedException();
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }
    }
}
