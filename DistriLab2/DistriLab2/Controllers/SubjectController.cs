using DistriLab2.Models;
using DistriLab2.Models.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DistriLab2.Controllers
{
    [ApiController]
    [Route("subject")]
    public class SubjectController : Controller
    {

		private readonly dblab2Context dbSubjects;
        private int NUM_PAG = 10;

        public SubjectController(dblab2Context context){
            dbSubjects = context;
        }

        [Authorize]
        [HttpGet]
        [Route("getSubject")]
        public ActionResult<List<Subject>> GetSubjectsPags(int pagina){
            try {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                var client = dbSubjects.Subjects.OrderBy(x => x.CodSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToList();
                return Ok(client);
            }catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectNormal")]
        public ActionResult<List<Subject>> GetSubjectNormal(int pagina)
        {
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                var client = dbSubjects.Subjects.OrderBy(x => x.NameSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToList();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectFilterState")]
        public ActionResult<List<Subject>> GetSubjectFilterState(int pagina)
        {
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                var client = dbSubjects.Subjects.OrderBy(x => x.StatusSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToList();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectFilterCod")]
        public ActionResult<List<Subject>> GetSubjectFilterCod(int pagina)
        {
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                var client = dbSubjects.Subjects.OrderBy(x => x.CodSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToList();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectDecending")]
        public ActionResult<List<Subject>> GetSubjectDecending(int pagina)
        {
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                var client = dbSubjects.Subjects.OrderByDescending(x => x.NameSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToList();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubject/{CodSubject}")]
        public ActionResult<Subject> GetSubjectByCod(int CodSubject){
            try
            {
                var client = dbSubjects.Subjects.FindAsync(CodSubject);
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost]
        [Route("addSubject")]
        public async Task<IActionResult> AddSubject(RequestSubject subject){
            var cod = dbSubjects.Subjects.Count() + 1;
            Subject subAux = new(){
                CodSubject = cod,
                NameSubject = subject.NameSubject,
                Quotas = subject.Quotas,
                StatusSubject = subject.StatusSubject
            };
            Subject auxsub = dbSubjects.Subjects.Add(subAux).Entity;
            await dbSubjects.SaveChangesAsync();
            return Created($"/Subject/{subAux.CodSubject}", subAux);
        }

        [HttpPatch]
        [Route("updateSubject/{CodSubject}")]
        public async Task<IActionResult> updateSubject(int CodSubject, string NameSubject){
            try {
                var subject = await dbSubjects.Subjects.FindAsync(CodSubject);
                if (subject == null)
                {
                    return NotFound();
                }
                subject.NameSubject = NameSubject;
                await dbSubjects.SaveChangesAsync();
                return Ok(subject);
            }catch (Exception ex) {
                return BadRequest(ex.Message);
            }
 
        }

        [HttpPatch]
        [Route("updateStateSubject/{CodSubject}")]
        public async Task<IActionResult> updateStatusSubject(int CodSubject, string StatusSubject)
        {
            try
            {
                var subject = await dbSubjects.Subjects.FindAsync(CodSubject);
                if (subject == null)
                {
                    return NotFound();
                }
                subject.StatusSubject = StatusSubject;
                await dbSubjects.SaveChangesAsync();
                return Ok(subject);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPut]
        [Route("editSubject")]
        public async Task<IActionResult> PatchSubject(Subject subject){
            var update = await dbSubjects.Subjects.FindAsync(subject.CodSubject);

            if (update == null)
                return BadRequest();

            update.CodSubject = subject.CodSubject;
            update.NameSubject = subject.NameSubject;
            update.Quotas = subject.Quotas;
            update.StatusSubject = subject.StatusSubject;

            var aux = await dbSubjects.SaveChangesAsync() > 0;
            if (!aux)
            {
                return NoContent();
            }
            return Ok(update);
        }

        [HttpPatch]
        [Route("editSubject/{CodSubject}")]
        public async Task<ActionResult> Patch(int CodSubject, JsonPatchDocument<Subject> _subject){
            var subject = await dbSubjects.Subjects.FindAsync(CodSubject);
            if (subject == null)
            {
                return NotFound();
            }
            _subject.ApplyTo(subject, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
            await dbSubjects.SaveChangesAsync();
            return Ok(subject);
        }

	}
}

