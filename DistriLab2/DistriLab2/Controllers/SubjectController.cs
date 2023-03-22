using DistriLab2.Models.DB;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DistriLab2.Controllers
{
    [ApiController]
    [Route("subject")]
    public class SubjectController : Controller
    {

		private readonly dblab2Context _context;

        public SubjectController(dblab2Context context){
            _context = context;
        }

        [HttpGet]
        [Route("getSubject")]
        public ActionResult<List<Subject>> GetSubject(){
            try {
                var client = _context.Subjects.Take(20).ToList();
                return Ok(client);
            }catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubject/{CodSubject}")]
        public ActionResult<Subject> GetSubject(int CodSubject){
            try{
                var client = _context.Subjects.FindAsync(CodSubject);
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost]
        [Route("addSubject")]
        public async Task<IActionResult> AddSubject(Subject subject){
            Subject subAux = new(){
                CodSubject = subject.CodSubject,
                NameSubject = subject.NameSubject,
                Quotas = subject.Quotas,
                StatusSubject = subject.StatusSubject
            };
            Subject auxsub = _context.Subjects.Add(subAux).Entity;
            await _context.SaveChangesAsync();
            return Created($"/Subject/{subAux.CodSubject}", subAux);
        }

        [HttpPut]
        [Route("updateSubject/{CodSubject}")]
        public async Task<IActionResult> updateSubject(int CodSubject, string NameSubject){
            try {
                var subject = await _context.Subjects.FindAsync(CodSubject);
                if (subject == null)
                {
                    return NotFound();
                }
                subject.NameSubject = NameSubject;
                await _context.SaveChangesAsync();
                return Ok(subject);
            }catch (Exception ex) {
                return BadRequest(ex.Message);
            }
 
        }

        [HttpPatch]
        [Route("editSubject")]
        public async Task<IActionResult> PatchSubject(Subject subject){
            var update = await _context.Subjects.FindAsync(subject.CodSubject);

            if (update == null)
                return BadRequest();

            update.CodSubject = subject.CodSubject;
            update.NameSubject = subject.NameSubject;
            update.Quotas = subject.Quotas;
            update.StatusSubject = subject.StatusSubject;

            var aux = await _context.SaveChangesAsync() > 0;
            if (!aux)
            {
                return NoContent();
            }
            return Ok(update);
        }

        [HttpPatch]
        [Route("editSubject/{CodSubject}")]
        public async Task<ActionResult> Patch(int CodSubject, JsonPatchDocument<Subject> _subject){
            var subject = await _context.Subjects.FindAsync(CodSubject);
            if (subject == null)
            {
                return NotFound();
            }
            _subject.ApplyTo(subject, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
            await _context.SaveChangesAsync();
            return Ok(subject);
        }

	}
}

