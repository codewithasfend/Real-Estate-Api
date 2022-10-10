using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Models;
using System.Linq;
using System.Security.Claims;

namespace RealEstateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        ApiDbContext _dbContext = new ApiDbContext();

        [HttpGet("PropertyList")]
        [Authorize]
        public IActionResult GetProperties(int categoryId)
        {
            var propertiesResult = _dbContext.Properties.Where(c => c.CategoryId==categoryId);
            if (propertiesResult == null)
            {
                return NotFound();
            }
            return Ok(propertiesResult);
        }


        [HttpGet("PropertyDetail")]
        [Authorize]
        public IActionResult GetPropertyDetail(int id)
        {
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null) return NotFound();

            var propertyResult = _dbContext.Properties.Find(id);

            if (propertyResult !=null)
            {
                var result = _dbContext.Properties
                 .Include(p => p.Bookmarks).Where(p => p.Id==id)
                 .Select(p => new
                 {
                     p.Id,
                     p.Name,
                     p.Detail,
                     p.Address,
                     p.Price,
                     p.ImageUrl,
                     p.User.Phone,
                     Bookmark = p.Bookmarks.FirstOrDefault(u => u.User_Id==user.Id)
                 }).FirstOrDefault();
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("TrendingProperties")]
        [Authorize]
        public IActionResult GetTrendingProperties()
        {
            var propertiesResult = _dbContext.Properties.Where(c => c.IsTrending == true);
            if (propertiesResult == null)
            {
                return NotFound();
            }
            return Ok(propertiesResult);
        }

        [HttpGet("SearchProperties")]
        [Authorize]
        public IActionResult GetSearchProperties(string address)
        {
            var propertiesResult = _dbContext.Properties.Where(p => p.Address.Contains(address));
            if (propertiesResult == null)
            {
                return NotFound();
            }
            return Ok(propertiesResult);
        }


        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] Property property)
        {
            if (property == null)
            {
                return NoContent();
            }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user==null) return NotFound();
                property.IsTrending = false;
                property.UserId = user.Id;
                _dbContext.Properties.Add(property);
                _dbContext.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
        }

    }
}
