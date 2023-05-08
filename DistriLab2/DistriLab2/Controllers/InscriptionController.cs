using DistriLab2.Models;
using DistriLab2.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Cryptography;

namespace DistriLab2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("inscripcion")]
    public class InscriptionController : Controller
    {
        private readonly dblab2Context _context;
        private string TEXT_ALERT_STUDENT = "EL ESTUDIANTE NO SE ENCUENTRA ACTIVO";
        private string TEXT_ALERT_SUBJECT = "LA MATERIA NO SE ENCUENTRA ACTIVA";
        private string TEXT_ALERT_READY_INSCRIPTION = "LA MATERIA YA SE ENCUENTRA INSCRITA";
        private string NOT_FOUND_INSCRIPTION = "LA INSCRIPCIÓN NO EXISTE";
        private object idInscription;

        public InscriptionController(dblab2Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getInscriptions")]
        public ActionResult<ResponseInscriptionJoin> GetInscriptions()
        {

            var resultado = (from c1 in _context.Inscriptions
                             join c2 in _context.Students on c1.CodStudent equals c2.CodStudent
                             join c3 in _context.Subjects on c1.CodSubject equals c3.CodSubject
                             select new
                             {
                                 idInscription = c1.IdInscription,
                                 codStudent = c1.CodStudent,
                                 nameStudent = c2.FirstNameStudent + " " + c2.LastNameStudent,
                                 codSubject = c1.CodSubject,
                                 nameSubject = c3.NameSubject,
                                 dateRegistration = c1.DateRegistration.ToString("dd/MM/yyyy")
                              }).Take(200).ToList();
            return Ok(resultado);
        }

        [HttpGet]
        [Route("getInscriptionsN")]
        public ActionResult<object> GetInscriptionsWithName()
        {
            using (var contexto = _context)
            {
                var inscripciones = contexto.Inscriptions
                .Include(i => i.CodSubjectNavigation)
                .Include(i => i.CodStudentNavigation).Take(20)
                .ToList();

                foreach (var inscripcion in inscripciones)
                {
                    inscripcion.CodSubjectNavigation = contexto.Subjects.SingleOrDefault(m => m.CodSubject == inscripcion.CodSubject);
                    
                    inscripcion.CodStudentNavigation = contexto.Students.SingleOrDefault(e => e.CodStudent == inscripcion.CodStudent);
                }

                return inscripciones;
            }
        }

        [HttpGet]
        [Route("getInscription/{id}")]
        public async Task<ActionResult<Inscription>> GetInscription(int id)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);

            if (inscription == null)
            {
                return NotFound();
            }

            return inscription;
        }

        [HttpPost]
        [Route("addInscription")]
        public async Task<IActionResult> AddInscription(InscriptionR inscription)
        {
            if (validateActiveSubject(inscription.CodSubject, _context))
                return BadRequest(TEXT_ALERT_SUBJECT);
            if (validateActiveStudent(inscription.CodStudent, _context))
                return BadRequest(TEXT_ALERT_STUDENT);
            if (validateNotInscription(inscription.CodSubject, inscription.CodStudent, _context))
                return BadRequest(TEXT_ALERT_READY_INSCRIPTION);
            Inscription inscriptionAux = new()
            {
                IdInscription = incrementId(_context) + 1,
                CodStudent = inscription.CodStudent,
                CodSubject = inscription.CodSubject,
                DateRegistration = DateTime.UtcNow
            };
            Inscription auxins = _context.Inscriptions.Add(inscriptionAux).Entity;
            await _context.SaveChangesAsync();
            return Created($"/Inscription/{inscriptionAux.IdInscription}", inscriptionAux);
        }

        [HttpPut]
        [Route("editInscription")]
        public async Task<IActionResult> Put(InscriptionR inscription)
        {
            var update = await _context.Inscriptions.FindAsync(inscription.IdInscription);
            if (validateActiveSubject(inscription.CodSubject, _context))
                return BadRequest(TEXT_ALERT_SUBJECT);
            if (validateActiveStudent(inscription.CodStudent, _context))
                return BadRequest(TEXT_ALERT_STUDENT);
            if (validateNotInscription(inscription.CodSubject, inscription.CodStudent, _context))
                return BadRequest(TEXT_ALERT_READY_INSCRIPTION);
            if (update == null)
                return NotFound(NOT_FOUND_INSCRIPTION + inscription.IdInscription);
            update.IdInscription = inscription.IdInscription;
            update.CodStudent = inscription.CodStudent;
            update.CodSubject = inscription.CodSubject;
            update.DateRegistration = DateTime.UtcNow;

            var aux = await _context.SaveChangesAsync() > 0;
            if (!aux)
                return NoContent();
            return Ok(update);
        } 

        [HttpPatch]
        [Route("updateCodeSubject/{idInscription}")]
        public async Task<IActionResult> updateInscriptionCodeSubject(int idInscription, int codeSubject)
        {
            try
            {
                var inscription = await _context.Inscriptions.FindAsync(idInscription);
                if (inscription == null)
                    return NotFound(NOT_FOUND_INSCRIPTION);
                if (validateActiveSubject(codeSubject, _context))
                    return BadRequest(TEXT_ALERT_SUBJECT);
                inscription.CodSubject = codeSubject;
                await _context.SaveChangesAsync();
                return Ok(inscription);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Route("updateCodeStudent/{idInscription}")]
        public async Task<IActionResult> updateInscriptionCodeStudent(int idInscription, int codeStudent)
        {
            try
            {
                var inscription = await _context.Inscriptions.FindAsync(idInscription);
                if (inscription == null)
                    return NotFound(NOT_FOUND_INSCRIPTION);
                if (validateActiveStudent(codeStudent, _context))
                    return BadRequest(TEXT_ALERT_STUDENT);
                inscription.CodStudent = codeStudent;
                await _context.SaveChangesAsync();
                return Ok(inscription);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public static bool validateActiveStudent(int codStudent, dblab2Context dbContext)
        {
            return (dbContext.Students.Any(e => e.CodStudent == codStudent && e.StatusStudent == "I"));
        }

        public static bool validateActiveSubject(int codSubject, dblab2Context dbContext)
        {
            return (dbContext.Subjects.Any(e => e.CodSubject == codSubject && e.StatusSubject == "I"));
        }

        public static bool validateNotInscription(int codSubject, int codStudent, dblab2Context dbContext)
        {
            return (dbContext.Inscriptions.Any(e => e.CodSubject == codSubject && e.CodStudent == codStudent));
        }

        public static int incrementId(dblab2Context dbContext)
        {
            int count = dbContext.Inscriptions.Count();
            return count;
        }
    }
}