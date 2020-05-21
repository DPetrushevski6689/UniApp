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
    public class StudentsController : Controller
    {
        private readonly UniAppContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment webHostEnvironment;

        public StudentsController(UniAppContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment webhostpath)
        {
            _context = context;
            webHostEnvironment = webhostpath;
        }

        // GET: Students
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Index(string searchIndeks, string searchFirstName, string searchLastName)
        {
            IQueryable<Student> students = _context.Student.AsQueryable();

            if (!string.IsNullOrEmpty(searchIndeks))
            {
                students = students.Where(s => s.StudentId.Contains(searchIndeks));
            }
            if (!string.IsNullOrEmpty(searchFirstName))
            {
                students = students.Where(s => s.FirstName.Contains(searchFirstName));
            }
            if (!string.IsNullOrEmpty(searchLastName))
            {
                students = students.Where(s => s.LastName.Contains(searchLastName));
            }

            students = students.Include(s => s.Courses).ThenInclude(s => s.Course);

            var viewmodel = new StudentsFilter
            {
                Students = await students.ToListAsync()
            };

            return View(viewmodel);
        }

        // GET: Students/Details/5
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Courses).ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        /**
        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel,ProfilePicture")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }**/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(StudentsImageDemoModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = UploadedFile(model);

                Student student = new Student
                {
                    FirstName = model.Student.FirstName,
                    LastName = model.Student.LastName,
                    StudentId = model.Student.StudentId,
                    EnrollmentDate = model.Student.EnrollmentDate,
                    AcquiredCredits = model.Student.AcquiredCredits,
                    CurrentSemestar = model.Student.CurrentSemestar,
                    EducationLevel = model.Student.EducationLevel,
                    ProfilePicture = uniqueFileName,
                };
                _context.Student.Add(student);
                //dbContext.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private string UploadedFile(StudentsImageDemoModel model)
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


        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var viewmodel = new StudentsImageDemoModel
            {
                Student = student
            };

            return View(viewmodel);
        }

        /**
        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,FirstName,LastName,EnrollmentDate,AcquiredCredits,CurrentSemestar,EducationLevel,ProfilePicture")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }**/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, StudentsImageDemoModel model)
        {
            if (id != model.Student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    /**string uniqueFileName = UploadedFile(model);
                    Student student = new Student
                    {
                        FirstName = model.Student.FirstName,
                        LastName = model.Student.LastName,
                        StudentId = model.Student.StudentId,
                        EnrollmentDate = model.Student.EnrollmentDate,
                        AcquiredCredits = model.Student.AcquiredCredits,
                        CurrentSemestar = model.Student.CurrentSemestar,
                        EducationLevel = model.Student.EducationLevel,
                        ProfilePicture = uniqueFileName,
                    };**/
                    _context.Update(model.Student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(model.Student.Id))
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
            return View(model);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                 .Include(s => s.Courses).ThenInclude(s => s.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            _context.Student.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> ShowCourses(int id) //id na student
        {
            IQueryable<Enrollment> enrollments = _context.Enrollment.AsQueryable();

            if (id != null)
            {
                enrollments = enrollments.Where(e => e.StudentId == id);
            }

            enrollments = enrollments.Include(e => e.Student).Include(e => e.Course);

            var viewmodel = new StudentsShowCourses
            {
                Enrollments = await enrollments.ToListAsync()
            };

            return View(viewmodel);
        }

        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> AddProjects(int id) //id na enrollment
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = _context.Enrollment.Where(e => e.Id == id).Include(e => e.Student).Include(e => e.Course).First();

            var viewmodel = new AddProjectsModel
            {
                Enrollment = enrollment
            };

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Student")]
        public async Task<IActionResult> AddProjects(int id, AddProjectsModel viewmodel)
        {
            if (id != viewmodel.Enrollment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();

                    Enrollment enrollment = new Enrollment
                    {
                        Id = id,
                        ProjectUrl = viewmodel.ProjectUrl,
                        SeminarUrl = viewmodel.SeminarUrl
                    };

                    _context.Update(viewmodel.Enrollment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(viewmodel.Enrollment.StudentId))
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


        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}


