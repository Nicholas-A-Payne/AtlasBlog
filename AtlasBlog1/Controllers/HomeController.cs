using AtlasBlog1.Data;
using AtlasBlog1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using X.PagedList;

namespace AtlasBlog1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? pageNum)
        {
            
            pageNum ??=  1;
            //ToPagedList always needs to know what page to render
            //PagedLists always need to be ordered explicitly
            var blogs = await _context.Blogs.OrderByDescending(b => b.Created).ToPagedListAsync(pageNum, 5);
            
            return View(blogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}