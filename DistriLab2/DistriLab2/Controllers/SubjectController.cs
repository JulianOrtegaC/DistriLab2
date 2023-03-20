using System;
using DistriLab2.Models;
using DistriLab2.Models.DB;
using Microsoft.AspNetCore.Mvc;

namespace DistriLab2.Controllers
{
    [ApiController]
    [Route("subject")]
    public class SubjectController{

        private readonly dblab2Context _context;

        public SubjectController(dblab2Context context){
            _context = context;

		}

        [HttpGet]
        [Route("getSubject")]
        public ActionResult<Subject> Getsubject()
        {
            var client = _context.Subjects.Take(20).ToList();
            return Ok(client);
        }

        private ActionResult<Subject> Ok(object client)
        {
            throw new NotImplementedException();
        }

        /*
        [HttpPost]
        [Route("addSubject")]
        public async Task<IActionResult> AddSubject(Subject subject)
        {

            Subject studentAux = _context.Subjects.Add(subject).Entity;
            await _context.SaveChangesAsync();
            return Created($"/Subject/{subject.CodSubject}", subject);

        }*/

    }
}

