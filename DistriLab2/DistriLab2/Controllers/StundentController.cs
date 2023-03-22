using Microsoft.AspNetCore.Mvc;
using DistriLab2.Models;
using DistriLab2.Models.DB;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;

namespace DistriLab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private static string TEXT_ALERT_STUDENT_NOT_FOUND = "ESTUDIANTE NO ENCONTRADO";
        private static string TEXT_ALERT_NDOCUMENT_REGISTERED = "DOCUMENTO DE ESTUDIANTE YA ESTA REGISTRADO";
        private readonly dblab2Context _context;

        public StudentController(dblab2Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getStudent")]
        public ActionResult<Student> GetPerson()
        {
            var client = _context.Students.Take(20).ToList();
            return Ok(client);
        }


        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            if (DocumentExists(_context, student.NumDocument)) {
                return BadRequest(TEXT_ALERT_NDOCUMENT_REGISTERED);
            }
            else
            {

            student.CodStudent = GenerarCodigo(_context);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.CodStudent }, student);
            }
        }

        public static int GenerarCodigo(dblab2Context context)
        {
            int year = DateTime.Now.Year;
            int semestre = (DateTime.Now.Month <= 6) ? 1 : 2;
            int contador = 1;
            int codigoGenerado;
            bool codigoExiste;

            do
            {
                codigoGenerado = Int32.Parse($"{year}{semestre}{contador:D5}");
                codigoExiste = StudentExists(context, codigoGenerado);

                if (codigoExiste)
                {
                    contador++;
                }

            } while (codigoExiste);

            return codigoGenerado;
        }


        public static bool StudentExists(dblab2Context dbContext, int codigoGenerado)
        {
            return dbContext.Students.Any(e => e.CodStudent == codigoGenerado);
        }
        public static bool DocumentExists(dblab2Context dbContext, string document)
        {
            return dbContext.Students.Any(e => e.NumDocument == document);
        }


        [HttpPut]
        [Route("editStudent")]
        public async Task<IActionResult> Put(Student student)
        {
            var update = await _context.Students.FindAsync(student.CodStudent);

            if (update == null)
                return BadRequest();
          
            update.FirstNameStudent = student.FirstNameStudent;
            update.LastNameStudent = student.LastNameStudent;
            update.TypeDocument = student.TypeDocument;
            update.NumDocument = student.NumDocument;
            update.StatusStudent = student.StatusStudent;
            update.GenderStudent = student.GenderStudent;

            var aux = await _context.SaveChangesAsync() > 0;
            if (!aux)
            {
                return NoContent();
            }
            return Ok(update);
        }

        /*[HttpPatch]
        [Route("updateStudent/{codStudent}")]
        public async Task<IActionResult> updateInscriptionCodeStudent(int codStudent, JsonPatchDocument<Student> student)
        {
            var _studentAux = await _context.Students.FindAsync(codStudent);
            if (_studentAux == null)
            {
                return NotFound();
            }

            student.ApplyTo(_studentAux, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

            await _context.SaveChangesAsync();

            return Ok(_studentAux);
        }*/

        /*[HttpPatch]
        [Route("updateStudent/{codStudent}")]
        public async Task<IActionResult> updateInscriptionCodeStudent(int codStudent, JsonPatchDocument<Student>student)
        {
            try
            {
                var _studentAux = await _context.Students.FindAsync(codStudent);
                if (_studentAux == null)
                {
                    return BadRequest(TEXT_ALERT_STUDENT_NOT_FOUND);
                }else
                if(student.FirstNameStudent !="")
                    _studentAux.FirstNameStudent = student.FirstNameStudent;
                if (student.LastNameStudent != "")
                    _studentAux.LastNameStudent = student.LastNameStudent;
                if (student.TypeDocument != "")
                    _studentAux.TypeDocument = student.TypeDocument;
                if (student.NumDocument != "")
                    _studentAux.NumDocument = student.NumDocument;
                if (student.StatusStudent != "")
                    _studentAux.StatusStudent = student.StatusStudent;
                if (student.GenderStudent != "")
                    _studentAux.GenderStudent = student.GenderStudent;




                await _context.SaveChangesAsync();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }*/
        [HttpPatch]
        [Route("updateNameStudent/{CodStudent}")]
        public async Task<IActionResult> updateNameStudent(int CodStudent,string FirstNameStudent)
        {
            try
            {
                var student = await _context.Students.FindAsync(CodStudent);
                if (student == null)
                {
                    return NotFound();
                }
                student.FirstNameStudent = FirstNameStudent;
                await _context.SaveChangesAsync();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("updateLastNameStudent/{CodStudent}")]
        public async Task<IActionResult> updateLastNameStudent(int CodStudent, string LastNameStudent)
        {
            try
            {
                var student = await _context.Students.FindAsync(CodStudent);
                if (student == null)
                {
                    return NotFound();
                }
                student.LastNameStudent = LastNameStudent;
                await _context.SaveChangesAsync();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("updateTypeDocumentStudent/{CodStudent}")]
        public async Task<IActionResult> updateTypeDocumentStudent(int CodStudent, string TypeDocument)
        {
            try
            {
                var student = await _context.Students.FindAsync(CodStudent);
                if (student == null)
                {
                    return NotFound();
                }
                student.TypeDocument = TypeDocument;
                await _context.SaveChangesAsync();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("updateNumDocumentStudent/{CodStudent}")]
        public async Task<IActionResult> updateNumDocumentStudent(int CodStudent, string NumDocument)
        {
            try
            {
                var student = await _context.Students.FindAsync(CodStudent);
                if (student == null)
                {
                    return NotFound();
                }
                student.NumDocument = NumDocument;
                await _context.SaveChangesAsync();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("updateStatusStudent/{CodStudent}")]
        public async Task<IActionResult> updateStatusStudent(int CodStudent, string StatusStudent)
        {
            try
            {
                var student = await _context.Students.FindAsync(CodStudent);
                if (student == null)
                {
                    return NotFound();
                }
                student.StatusStudent = StatusStudent;
                await _context.SaveChangesAsync();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("updateGenderStudent/{CodStudent}")]
        public async Task<IActionResult> updateGenderStudent(int CodStudent, string GenderStudent)
        {
            try
            {
                var student = await _context.Students.FindAsync(CodStudent);
                if (student == null)
                {
                    return NotFound();
                }
                student.GenderStudent = GenderStudent;
                await _context.SaveChangesAsync();
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}