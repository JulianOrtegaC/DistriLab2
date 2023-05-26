using Microsoft.AspNetCore.Mvc;
using DistriLab2.Models;
using DistriLab2.Models.DB;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using DistriLab2.Service;

namespace DistriLab2.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private static string TEXT_ALERT_STUDENT_NOT_FOUND = "ESTUDIANTE NO ENCONTRADO";
        private static string TEXT_ALERT_NDOCUMENT_REGISTERED = "DOCUMENTO DE ESTUDIANTE YA ESTA REGISTRADO";
        private readonly dblab2Context _context;
        private readonly ICacheService _cache;

        public StudentController(dblab2Context context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        [Route("getStudent")]

        public async Task<IActionResult> GetPerson()
        {
            var cacheData = _cache.GetData<IEnumerable<Student>>("getStudent");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            cacheData = await _context.Students.Take(500).ToListAsync();
            var expireTime = DateTimeOffset.Now.AddMinutes(10);
            _cache.SetData<IEnumerable<Student>>("getStudent", cacheData, expireTime);
            return Ok(new { IsRedis = false, data = cacheData });
        }

        [HttpGet]
        [Route("getStudenttNormal")]
        public async Task<ActionResult<List<Student>>> GetStudentNormalAsync()
        {
            var cacheData = _cache.GetData<IEnumerable<Student>>("getStuN");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            cacheData = await _context.Students.Take(500).OrderBy(sub => sub.FirstNameStudent).ToListAsync();
            var expireTime = DateTimeOffset.Now.AddMinutes(10);
            _cache.SetData<IEnumerable<Student>>("getStuN", cacheData, expireTime);
            return Ok(new { IsRedis = false, data = cacheData });        
        }

        [HttpGet]
        [Route("getStudentFilterCod")]
        public async Task<ActionResult<List<Student>>> GetStudentFilterCodAsync()
        {
            var cacheData = _cache.GetData<IEnumerable<Student>>("getStuFC");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            cacheData = await _context.Students.Take(500).OrderBy(sub => sub.CodStudent).ToListAsync();
            var expireTime = DateTimeOffset.Now.AddMinutes(5);
            _cache.SetData<IEnumerable<Student>>("getStuFC", cacheData, expireTime);
            return Ok(new { IsRedis = false, data = cacheData });
        }

        [HttpGet]
        [Route("getStudentDecending")]
        public async Task<ActionResult<List<Student>>> GetStudentDecendingAsync()
        {
            var cacheData = _cache.GetData<IEnumerable<Student>>("getStuDe");
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            cacheData = await _context.Students.Take(500).OrderByDescending(sub => sub.FirstNameStudent).ToListAsync();
            var expireTime = DateTimeOffset.Now.AddMinutes(5);
            _cache.SetData<IEnumerable<Student>>("getStuDe", cacheData, expireTime);
            return Ok(new { IsRedis = false, data = cacheData });
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var cacheData = _cache.GetData<Student>("id");
            if (cacheData != null)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            cacheData = await _context.Students.FindAsync(id);
            var expireTime = DateTimeOffset.Now.AddMinutes(5);
            _cache.SetData<Student>("id", cacheData, expireTime);
            return Ok(new { IsRedis = false, data = cacheData });
        }

        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            if (DocumentExists(_context, student.NumDocument))
            {
                return BadRequest(TEXT_ALERT_NDOCUMENT_REGISTERED);
            }
            else
            {
                student.CodStudent = GenerarCodigo(_context);
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                var cacheData = await _context.Students.Take(500).ToListAsync();
                var expireTime = DateTimeOffset.Now.AddMinutes(10);
                _cache.SetData<IEnumerable<Student>>("getStudent", cacheData, expireTime);
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
            var cacheData = await _context.Students.ToListAsync();
            var expireTime = DateTimeOffset.Now.AddMinutes(10);
            _cache.SetData<IEnumerable<Student>>("getStudent", cacheData, expireTime);
            return Ok(update);
        }

        [HttpPatch]
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
    }
}