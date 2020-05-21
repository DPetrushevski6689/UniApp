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
    public class TeachersAPIController : ControllerBase
    {
        private readonly UniAppContext _context;

        public TeachersAPIController(UniAppContext context)
        {
            _context = context;
        }

        // GET: api/TeachersAPI
        [HttpGet]
        public IEnumerable<Teacher> GetTeacher()
        {
            return _context.Teacher;
        }

        // GET: api/TeachersAPI/5
        [HttpGet("{id}")]
        public List<Teacher> GetTeacher(string searchFirstName, string searchLastName, string searchDegree, string searchRank)
        {
            //var teacher = await _context.Teacher.FindAsync(id);

            IQueryable<Teacher> teachers = _context.Teacher.AsQueryable();

            if (!string.IsNullOrEmpty(searchFirstName))
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
            if (!string.IsNullOrEmpty(searchRank))
            {
                teachers = teachers.Where(t => t.AcademicRank.Contains(searchRank));
            }



            /**if (teacher == null)
            {
                return NotFound();
            }**/

            return teachers.ToList();
        }




        // PUT: api/TeachersAPI/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeacher(int id, AddPointsModel viewmodel)
        {
            if (id != viewmodel.Enrollment.Id)
            {
                return BadRequest();
            }

            _context.Entry(viewmodel.Enrollment).State = EntityState.Modified;

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
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
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


        // POST: api/TeachersAPI
        [HttpPost]
        public async Task<IActionResult> PostTeacher([FromBody] Teacher teacher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Teacher.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacher", new { id = teacher.Id }, teacher);
        }

        // DELETE: api/TeachersAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();

            return Ok(teacher);
        }

        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }
    }
}