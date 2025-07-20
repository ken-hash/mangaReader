using MangaReaderBareBone.Data;
using MangaReaderBareBone.DTO;
using MangaReaderBareBone.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MangaReaderBareBone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdatesController : ControllerBase
    {
        private readonly MangaReaderBareBoneContext _context;

        public UpdatesController(MangaReaderBareBoneContext context)
        {
            _context = context;
        }

        [HttpGet("site")]
        [ResponseCache(Duration = 60 * 1)]
        public async Task<ActionResult<DateTime?>> GetLastUpdatedSite(string site)
        {
            if (_context.LastUpdates == null || string.IsNullOrEmpty(site))
                return NotFound();
            var res = await _context.LastUpdates.OrderBy(e=>e.LastUpdate).LastOrDefaultAsync(e => e.SiteName == site);
            return res?.LastUpdate;
        }

        [HttpPost("update")]
        public async Task<ActionResult<UpdatesDTO?>> Update([FromBody] UpdatesDTO postReq)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            if (_context.LastUpdates == null)
                return Problem("Can't connect to database");
            if (postReq.User is null || postReq.Url is null || postReq.Site is null)
                return BadRequest(ModelState);
            var newUpdate = new LastUpdates
            {
                SiteName = postReq.Site,
                SiteUrl = postReq.Url,
                LastUpdate = DateTime.Now,
                MachineName = postReq.User
            };


            _context.LastUpdates.Add(newUpdate);
            await _context.SaveChangesAsync();
            return Ok("Success");
        }
    }
}
