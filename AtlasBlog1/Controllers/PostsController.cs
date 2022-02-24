#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasBlog1.Data;
using AtlasBlog1.Models;
using Microsoft.AspNetCore.Authorization;
using AtlasBlog1.Services;
using AtlasBlog1.Services.Interfaces;
using X.PagedList;

namespace AtlasBlog1.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;
        private readonly SearchService _searchService;

        public PostsController(ApplicationDbContext context,
                                SlugService slugService,
                                IImageService imageService,
                                SearchService searchService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
            _searchService = searchService;
        }

        // GET: Posts

        public async Task<IActionResult> BlogChildIndex(int blogId)
        {
            var children = await _context.Posts.Include(b => b.Blog)
                                                            .Where(b => b.BlogId == blogId)
                                                            .ToListAsync();
            return View("Index", children);
        }


        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts.Include(p => p.Blog);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(b => b.Blog)
                .Include(p => p.Tags)
                .Include(c => c.Comments)
                .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);


            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName");
            ViewData["TagIds"] = new MultiSelectList(_context.Tags, "Id", "TagItem");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,PostState,Body")] Post post,
                                                                                               IFormFile imageFile,
                                                                                               List<int> tagIds)
        {
            if (ModelState.IsValid)
            {
                if (imageFile is not null)
                {

                    post.ImageData = await _imageService.ConvertFileToByteArrayAsync(imageFile);
                    post.ImageType = imageFile.ContentType;
                }

                var slug = _slugService.UrlFriendly(post.Title, 100);

                //Ensure the Slug is unique in the DB, if not than throw a custom error
                var isUnipue = !_context.Posts.Any(b => b.Slug == slug);

                if (isUnipue)
                {
                    post.Slug = slug;
                }
                else
                {
                    ModelState.AddModelError("Title", "This title cannot be used (duplicate Slug)");
                    ModelState.AddModelError("", "Incorrect title");
                    ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", post.BlogId);
                    return View(post);
                }

                //CHeck for tags to add
                if (tagIds.Count > 0)
                {
                    var tag = _context.Tags;
                    foreach (var tagId in tagIds)
                    {
                        //I expect this line of code to add records into the PostTagsTable
                        post.Tags.Add(await tag.FindAsync(tagId));
                    }
                }

                post.Created = DateTime.UtcNow;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            

            var post = await _context.Posts
                                     .Include("Tags")
                                     .FirstOrDefaultAsync(b => b.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var tagIds = await post.Tags.Select(b => b.Id).ToListAsync();

            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", post.BlogId);
            ViewData["TagIds"] = new MultiSelectList(_context.Tags, "Id", "TagItem", tagIds);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Slug,Abstract,PostState,Body,Created")] Post post,
                                                                                                                     IFormFile imageFile,
                                                                                                                     List<int> tagIds)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (imageFile is not null)
                {
                    post.ImageData = await _imageService.ConvertFileToByteArrayAsync(imageFile);
                    post.ImageType = imageFile.ContentType;
                }

                var slug = _slugService.UrlFriendly(post.Title, 100);
                if (post.Slug != slug)
                {
                    //Ensure the Slug is unique in the DB, if not than throw a custom error
                    var isUnipue = !_context.Posts.Any(b => b.Slug == slug);

                    if (isUnipue)
                    {
                        post.Slug = slug;
                    }
                    else
                    {
                        ModelState.AddModelError("Title", "This title cannot be used (duplicate Slug)");
                        ModelState.AddModelError("", "Incorrect title");
                        ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", post.BlogId);
                        return View(post);
                    }
                }

                try
                {
                    post.Updated = DateTime.UtcNow;
                    post.Created = DateTime.SpecifyKind(post.Created, DateTimeKind.Utc);
                    _context.Update(post);
                    await _context.SaveChangesAsync();

                    ////Tag Management
                    var currentBlogPost = await _context.Posts
                                                        .Include("Tags")
                                                        .FirstOrDefaultAsync(b => b.Id == post.Id);

                    currentBlogPost.Tags.Clear();

                    var tags = _context.Tags;

                    if (tagIds.Count > 0)
                    {
                        foreach (var tagId in tagIds)
                        {
                            post.Tags.Add(await tags.FindAsync(tagId));
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }

                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", post.BlogId);
            ViewData["TagIds"] = new MultiSelectList(_context.Tags, "Id", "Text", tagIds);
            return View(post);
        }

        [AllowAnonymous]
        public async Task<IActionResult> SearchIndex(int? pageNum, string SearchItem)
        {
            pageNum ??= 1;
            var pageSize = 5;

            //I am going to use a search service to get all of the Posts that contain the SearchItem
            var posts = _searchService.ItemSearch(SearchItem);
            var pagedPosts = await posts.ToPagedListAsync(pageNum, pageSize);

            ViewData["SearchItem"] = SearchItem;
            return View(pagedPosts);
        }


        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
