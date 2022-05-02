#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtlasBlog1.Models;
using AtlasBlog1.Enums;
using AtlasBlog1.Data;

namespace AtlasBlog.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogPostsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the specified number of latest Posts
        /// </summary>
        /// <param name="num">integer count of records</param>
        /// <returns>
        /// Returns a list of Blog Posts
        /// </returns>
        [HttpGet("GetTopXPosts/{num:int}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetTopXPosts(int num)
        {
            //How would I return the top X latest production ready Posts that aren't deleted
            //The latest posts
            var posts = await _context.Posts
                                 .Where(b => !b.IsDeleted &&
                                            b.PostState == PostState.ProductionReady)
                                 .OrderByDescending(b => b.Created)
                                 .Take(num)
                                 .ToListAsync();

            return posts;
        }
    }
}
