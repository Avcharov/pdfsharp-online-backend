using Microsoft.AspNetCore.Mvc;
using pdfsharp_online_backend.Domain;

namespace pdfsharp_online_backend.Controllers
{
    [Route("api/explorer/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {

        private readonly AppDbContext _context;
        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [Route("{userId}")]
        public async Task<ActionResult<EditView>> GetProjectsByUserId(int userId)
        {
            return Ok(await _context.EditViews.AsNoTracking().Where(p => p.UserId == userId).ToListAsync());

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<EditView>> AddProject(EditView newProject)
        {
            if (newProject == null)
            {
                return BadRequest();
            }

            if (newProject.UserId == 0)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(newProject.Base64AttachmentCode))
            {
                return BadRequest(new { Message = "Attachment is empty" });
            }

            User user = _context.Users.FirstOrDefault(x => x.Id == newProject.UserId);

            if(user == null)
            {
                return NotFound();
            }

            await _context.EditViews.AddAsync(newProject);
            await _context.SaveChangesAsync();

            return Ok(newProject);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<EditView>> GetProjectById([FromQuery] int projectId)
        {
            return Ok(await _context.EditViews.AsNoTracking().FirstOrDefaultAsync(p => p.Id == projectId));

        }



    }
}
