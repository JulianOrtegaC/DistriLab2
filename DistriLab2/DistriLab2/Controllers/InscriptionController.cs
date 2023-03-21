using DistriLab2.Models.DB;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace DistriLab2.Controllers
{
    public class InscriptionController : Controller
    {
        private readonly dblab2Context _context;

        public InscriptionController(dblab2Context context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getInscription")]
        public ActionResult<Inscription> GetInscription()
        {
            var client = _context.Inscriptions.Take(20).ToList();
            return Ok(client);
        }

        [HttpPost]
        [Route("addInscription")]
        public async Task<IActionResult> AddInscription(Inscription inscription)
        {
            Inscription inscriptionAux = new()
            {
                IdInscription = inscription.IdInscription,
                CodStudent = inscription.CodStudent,
                CodSubject = inscription.CodSubject,
                DateRegistration = inscription.DateRegistration
            };
            Inscription auxins = _context.Inscriptions.Add(inscriptionAux).Entity;
            await _context.SaveChangesAsync();
            return Created($"/Inscription/{inscriptionAux.IdInscription}", inscriptionAux);
        }

        [HttpPut]
        [Route("editInscription")]
        public async Task<IActionResult> Put(Inscription inscription)
        {
            var update = await _context.Inscriptions.FindAsync(inscription.IdInscription);

            if (update == null)
                return BadRequest();
            update.IdInscription = inscription.IdInscription;
            update.CodStudent = inscription.CodStudent;
            update.CodSubject = inscription.CodSubject;
            update.DateRegistration = inscription.DateRegistration;
          
            var aux = await _context.SaveChangesAsync() > 0;
            if (!aux)
            {
                return NoContent();
            }
            return Ok(update);
        }

        [HttpPatch]
        [Route("editInscription/{id}")]
        public async Task<ActionResult> Patch(string id, JsonPatchDocument<Inscription> _inscription)
        {
            var inscription = await _context.Inscriptions.FindAsync(id);
            if (inscription == null)
            {
                return NotFound();
            }
            _inscription.ApplyTo(inscription, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);
            await _context.SaveChangesAsync();
            return Ok(inscription);
        }
    }
}