using Microsoft.AspNetCore.Mvc;
using DistriLab2.Models;
using DistriLab2.Models.DB;
using System;
using Microsoft.EntityFrameworkCore;


namespace DistriLab2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
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
            if (ExisteDocument(_context, student.NumDocument)) {
                return BadRequest("El Numero de Documento  ya se encuentra registrado");
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
                codigoExiste = ExisteCodigo(context, codigoGenerado);

                if (codigoExiste)
                {
                    contador++;
                }

            } while (codigoExiste);

            return codigoGenerado;
        }


        public static bool ExisteCodigo(dblab2Context dbContext, int codigoGenerado)
        {
            return dbContext.Students.Any(e => e.CodStudent == codigoGenerado);
        }
        public static bool ExisteDocument(dblab2Context dbContext, string document)
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
            update.CodStudent = student.CodStudent;
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


    }
}