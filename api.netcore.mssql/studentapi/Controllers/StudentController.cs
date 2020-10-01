using studentapi.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace studentapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private DataContext _context = null;
        public StudentController(DataContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public ActionResult GetStudents()
        {
            return Ok(this._context.Students.ToList());
        }
    }
}