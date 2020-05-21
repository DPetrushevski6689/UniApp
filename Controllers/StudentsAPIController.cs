using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniApp.Data;
using UniApp.Models;
using UniApp.ViewModelsFunctionalities;

namespace UniApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsAPIController : ControllerBase
    {
        private readonly UniAppContext _context;

        public StudentsAPIController(UniAppContext context)
        {
            _context = context;
        }

        // GET: api/StudentsAPI
        [HttpGet]
        public IEnumerable<Student> GetStudent()
        {
            return _context.Student;
        }

        // GET: api/StudentsAPI/5
        [HttpGet("{id}")]
        public List<Student> GetStudent(string searchIndeks, string searchFirstName, string searchLastName)
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

            return students.ToList();
        }


        // PUT: api/StudentsAPI/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, AddProjectsModel model)
        {
            if (id != model.Enrollment.StudentId)
            {
                return BadRequest();
            }

            _context.Entry(model.Enrollment.Student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                IQueryable<Enrollment> toBeRemoved = _context.Enrollment.Where(e => e.Id == id);
                _context.Enrollment.RemoveRange(toBeRemoved);

                _context.Enrollment.Add(new Enrollment
                {
                    StudentId = id,
                    CourseId = model.Enrollment.CourseId,
                    ProjectUrl = model.ProjectUrl,
                    SeminarUrl = model.SeminarUrl
                });

                _context.Update(model.Enrollment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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


        // POST: api/StudentsAPI
        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Student.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.Id }, student);
        }

        // DELETE: api/StudentsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}