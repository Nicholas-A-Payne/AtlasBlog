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

namespace AtlasBlog1.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly SlugService _slugService;

        public PostsController(ApplicationDbContext context,
                                SlugService slugService,
                                IImageService imageService)
        {
            _context = context;
            _slugService = slugService;
            _imageService = imageService;
        }

        // GET: Posts

        public async Task<IActionResult> BlogChildIndex (int blogId)
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
        return View();
    }

    // POST: Posts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,PostState,Body")] Post post)
    {
        if (ModelState.IsValid)
        {
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

        var post = await _context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }
        ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "BlogName", post.BlogId);
        return View(post);
    }

    // POST: Posts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Slug,Abstract,PostState,Body,Created")] Post post)
    {
        if (id != post.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {

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
        return View(post);
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
