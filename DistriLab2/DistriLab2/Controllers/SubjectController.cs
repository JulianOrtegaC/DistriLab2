using DistriLab2.Models;
using DistriLab2.Models.DB;
using DistriLab2.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DistriLab2.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("subject")]
    public class SubjectController : Controller
    {

		private readonly dblab2Context dbSubjects;
        private int NUM_PAG = 10;
        private readonly ICacheService _cache;

        public SubjectController(dblab2Context context, ICacheService cache)
        {
            dbSubjects = context;
            _cache = cache;
        }

        [HttpGet]
        [Route("getSubject")]
        public async Task<IActionResult> GetSubjectsPags(int pagina) {
            var cacheData = _cache.GetData<IEnumerable<Subject>>("getSubjects" + pagina);
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                cacheData = await dbSubjects.Subjects.OrderBy(x => x.CodSubject).Skip(indiceInicio).Take(NUM_PAG).ToListAsync(); ;
                var expireTime = DateTimeOffset.Now.AddMinutes(2);
                _cache.SetData("getSubjects" + pagina, cacheData, expireTime);
                return Ok(new { IsRedis = false, data = cacheData });
            }catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectNormal")]
        public async Task<IActionResult> GetSubjectNormal(int pagina)
        {
            var cacheData = _cache.GetData<IEnumerable<Subject>>("getSubjectsN" + pagina);
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                cacheData = await dbSubjects.Subjects.OrderBy(x => x.NameSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToListAsync();
                var expireTime = DateTimeOffset.Now.AddMinutes(2);
                _cache.SetData("getSubjectsN" + pagina, cacheData, expireTime);
                return Ok(new { IsRedis = false, data = cacheData });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectFilterState")]
        public async Task<IActionResult> GetSubjectFilterState(int pagina)
        {
            var cacheData = _cache.GetData<IEnumerable<Subject>>("getSubjectsF" + pagina);
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                cacheData = await dbSubjects.Subjects.OrderBy(x => x.StatusSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToListAsync();
                var expireTime = DateTimeOffset.Now.AddMinutes(2);
                _cache.SetData("getSubjectsF" + pagina, cacheData, expireTime);
                return Ok(new { IsRedis = false, data = cacheData });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectFilterCod")]
        public async Task<IActionResult> GetSubjectFilterCod(int pagina)
        {
            var cacheData = _cache.GetData<IEnumerable<Subject>>("getSubjects" + pagina);
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                cacheData = await dbSubjects.Subjects.OrderBy(x => x.CodSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToListAsync();
                var expireTime = DateTimeOffset.Now.AddMinutes(2);
                _cache.SetData("getSubjects" + pagina, cacheData, expireTime);
                return Ok(new { IsRedis = false, data = cacheData });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubjectDecending")]
        public async Task<IActionResult> GetSubjectDecending(int pagina)
        {
            var cacheData = _cache.GetData<IEnumerable<Subject>>("getSubjectsD" + pagina);
            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            try
            {
                var countSubjects = dbSubjects.Subjects.Count();
                int numPags = (int)Math.Ceiling((double)countSubjects / NUM_PAG);
                int indiceInicio = (pagina - 1) * NUM_PAG;
                cacheData = await dbSubjects.Subjects.OrderByDescending(x => x.NameSubject)
                                         .Skip(indiceInicio)
                                         .Take(NUM_PAG)
                                         .ToListAsync();
                var expireTime = DateTimeOffset.Now.AddMinutes(2);
                _cache.SetData("getSubjectsD" + pagina, cacheData, expireTime);
                return Ok(new { IsRedis = false, data = cacheData });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getSubject/{CodSubject}")]
        public async Task<IActionResult> GetSubjectByCod(int CodSubject){
            var cacheData = _cache.GetData<Subject>("getSubject");
            if (cacheData != null)
            {
                return Ok(new { IsRedis = true, data = cacheData });
            }
            try
            {
                cacheData = await dbSubjects.Subjects.FindAsync(CodSubject);
                var expireTime = DateTimeOffset.Now.AddMinutes(2);
                _cache.SetData("getSubject", cacheData, expireTime);
                return Ok(new { IsRedis = false, data = cacheData });
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

