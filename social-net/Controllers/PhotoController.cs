using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using social_net.Data;
using social_net.Models;

namespace social_net.Controllers
{
    public class PhotoController : Controller
    {
        private readonly ILogger<PhotoController> _logger;
        private readonly AppDbContext _appDbContext;

        public PhotoController(ILogger<PhotoController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index(int photoId)
        {
            var photo = _appDbContext.Photos
                .Include(p => p.Album)
                .ThenInclude(a => a.User)
                .Include(p => p.Comments.OrderBy(c => c.AddedAt))
                .ThenInclude(p => p.User)
                .FirstOrDefault(p => p.Id.Equals(photoId));
            return View(photo);
        }

        [HttpPost]
        public IActionResult AddComment(string commentContent, int photoId, string currentUserId)
        {
            var photo = _appDbContext.Photos
            .FirstOrDefault(p => p.Id.Equals(photoId));

            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            // DE ADAUGAT IS ACCEPTED FEATURE
            var comment = new PhotoComment { User = currentUser, Text = commentContent, Photo = photo, AddedAt = DateTime.Now };

            _appDbContext.PhotoComments.Add(comment);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Photo", new { photoId = photoId });

        }
    }
}
