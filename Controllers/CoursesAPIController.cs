using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniApp.Data;
using UniApp.Models;
using UniApp.ViewModelsEdit;

namespace UniApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesAPIController : ControllerBase
    {
        private readonly UniAppContext _context;

        public CoursesAPIController(UniAppContext context)
        {
            _context = context;
        }

        // GET: api/CoursesAPI
        [HttpGet]
        public IEnumerable<Course> GetCourse()
        {
            return _context.Course;
        }

        // GET: api/CoursesAPI/5
        [HttpGet("{id}")]
        public List<Course> GetCourse(string searchTitle, string searchProgramme)
        {
            IQueryable<Course> courses = _context.Course.AsQueryable();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                courses = courses.Where(c => c.Title.Contains(searchTitle));
            }
            if (!string.IsNullOrEmpty(searchProgramme))
            {
                courses = courses.Where(c => c.Programme.Contains(searchProgramme));
            }


            /**if (course == null)
            {
                return NotFound();
            }**/

            return courses.ToList();
        }


        // PUT: api/CoursesAPI/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, CoursesStudentsEditClass viewmodel)
        {
            if (id != viewmodel.Course.Id)
            {
                return BadRequest();
            }

            _context.Entry(viewmodel.Course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                IEnumerable<int> listStudents = viewmodel.SelectedStudents;

                IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(s => !listStudents.Contains(s.StudentId) && s.CourseId == id);
                _context.Enrollment.RemoveRange(toBeRemoved);

                IEnumerable<int> existStudents = _context.Enrollment.Where(s => listStudents.Contains(s.StudentId) && s.CourseId == id)
                    .Select(s => s.StudentId);
                IEnumerable<int> newStudents = listStudents.Where(s => !existStudents.Contains(s));
                foreach (int studentId in newStudents)
                {
                    _context.Enrollment.Add(new Enrollment { StudentId = studentId, CourseId = id });
                }

                _context.Update(viewmodel.Course);
                await _context.SaveChangesAsync();
            }


            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        // POST: api/CoursesAPI
        [HttpPost]
        public async Task<IActionResult> PostCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Course.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // DELETE: api/CoursesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var course = await _context.Course.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Course.Remove(course);
            await _context.SaveChangesAsync();

            return Ok(course);
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.Id == id);
        }
    }
}