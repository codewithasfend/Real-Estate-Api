using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Models;
using System.Security.Claims;

namespace RealEstateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksController : ControllerBase
    {
        ApiDbContext _dbContext = new ApiDbContext();
        // GET: api/<BookmarksController>
        [HttpGet]
        public IActionResult Get()
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null) return NotFound();

            var bookmarks = from b in _dbContext.Bookmarks.Where(b => b.User_Id == user.Id)
                            join p in _dbContext.Properties on b.PropertyId equals p.Id
                            where b.Status == true
                            select new
                            {
                                b.Id,
                                p.Name,
                                p.Price,
                                p.ImageUrl,
                                p.Address,
                                b.Status,
                                b.User_Id,
                                b.PropertyId

                            };
            return Ok(bookmarks);
        }


        // POST api/<BookmarksController>
        [HttpPost]
        public IActionResult Post([FromBody] Bookmark bookmarkitem)
        {
            bookmarkitem.Status = true;
            _dbContext.Bookmarks.Add(bookmarkitem);
            _dbContext.SaveChanges();
            return Ok("Bookmark added");
        }

        // DELETE api/<BookmarksController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var bookmarkResult = _dbContext.Bookmarks.FirstOrDefault(b => b.Id == id);
            if (bookmarkResult == null)
            {
                return NotFound();
            }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user==null) return NotFound();
                if (bookmarkResult.User_Id == user.Id)
                {
                    _dbContext.Bookmarks.Remove(bookmarkResult);
                    _dbContext.SaveChanges();
                    return Ok("Bookmark deleted successfully");
                }
                return BadRequest();
            }
        }
    }
}
