using Microsoft.AspNetCore.Authorization;
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
                .ThenInclude(u => u.InitiatedFriendships)
                .ThenInclude(ifr => ifr.RecipientUser)
                .Include(p => p.Album)
                .ThenInclude(a => a.User)
                .ThenInclude(u => u.ReceivedFriendships)
                .ThenInclude(rf => rf.InitiatorUser)
                .Include(p => p.Comments.OrderBy(c => c.AddedAt))
                .ThenInclude(p => p.User)
                .FirstOrDefault(p => p.Id.Equals(photoId));

            return View(photo);
        }

        [HttpPost]
        public IActionResult AddComment(string commentContent, int photoId, string currentUserId)
        {
            var photo = _appDbContext.Photos
                .Include(p => p.Album)
                .ThenInclude(a => a.User)
                .FirstOrDefault(p => p.Id.Equals(photoId));

            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            var comment = new PhotoComment { User = currentUser, Text = commentContent, Photo = photo, AddedAt = DateTime.Now, IsAccepted = (currentUser == photo.Album.User) ? true : false };

            _appDbContext.PhotoComments.Add(comment);

            if (currentUser != photo.Album.User)
            {
                Notification notification = new Notification()
                {
                    User = photo.Album.User,
                    Content = currentUser.FirstName + " " + currentUser.LastName + " added a comment to your photo",
                    RedirectUrl = Url.Action("Index", "Photo", new { photoId = photoId })
                };
                _appDbContext.Notifications.Add(notification);
            }

            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Photo", new { photoId = photoId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCommentAdmin(string commentContent, int photoId, string currentUserId)
        {
            var photo = _appDbContext.Photos
                .Include(p => p.Album)
                .ThenInclude(a => a.User)
                .FirstOrDefault(p => p.Id.Equals(photoId));

            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            var comment = new PhotoComment { User = currentUser, Text = commentContent, Photo = photo, AddedAt = DateTime.Now, IsAccepted = true };

            _appDbContext.PhotoComments.Add(comment);

            if (currentUser != photo.Album.User)
            {
                Notification notification = new Notification()
                {
                    User = photo.Album.User,
                    Content = currentUser.FirstName + " " + currentUser.LastName + " added a comment to your photo",
                    RedirectUrl = Url.Action("Index", "Photo", new { photoId = photoId })
                };
                _appDbContext.Notifications.Add(notification);
            }

            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Photo", new { photoId = photoId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteCommentAdmin(int photoId, int commentId)
        {
            var commentToRemove = _appDbContext.PhotoComments
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id.Equals(commentId));

            if (commentToRemove != null)
            {
                _appDbContext.PhotoComments.Remove(commentToRemove);

                Notification notification = new Notification()
                {
                    User = commentToRemove.User,
                    Content = "One of your comments was removed!",
                    RedirectUrl = Url.Action("Index", "Photo", new {photoId = photoId})
                };

                _appDbContext.Notifications.Add(notification);

                _appDbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Photo", new { photoId = photoId });
        }

        public IActionResult DeleteOwnComment(int photoId, int commentId)
        {
            var commentToRemove = _appDbContext.PhotoComments
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id.Equals(commentId));

            if (commentToRemove != null)
            {
                _appDbContext.PhotoComments.Remove(commentToRemove);
                _appDbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Photo", new { photoId = photoId });
        }

        public IActionResult AcceptComment(int photoId, int commentId)
        {
            var comment = _appDbContext
                .PhotoComments
                .Include(c => c.User)
                .Include(c => c.Photo)
                .ThenInclude(p => p.Album)
                .ThenInclude(a => a.User)
                .FirstOrDefault(c => c.Id.Equals(commentId));

            comment.IsAccepted = true;


            Notification notification = new Notification()
            {
                User = comment.User,
                Content = comment.Photo.Album.User.FirstName + " " + comment.Photo.Album.User.LastName + " accepted your comment on their photo",
                RedirectUrl = Url.Action("Index", "Photo", new { photoId = photoId })
            };

            _appDbContext.PhotoComments.Update(comment);
            _appDbContext.Notifications.Add(notification);

            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Photo", new { photoId = photoId });
        }

        public IActionResult DeclineComment(int photoId, int commentId)
        {
            var comment = _appDbContext
                .PhotoComments
                .Include(c => c.User)
                .Include(c => c.Photo)
                .ThenInclude(p => p.Album)
                .ThenInclude(a => a.User)
                .FirstOrDefault(c => c.Id.Equals(commentId));


            Notification notification = new Notification()
            {
                User = comment.User,
                Content = comment.Photo.Album.User.FirstName + " " + comment.Photo.Album.User.LastName + " declined your comment on their photo",
                RedirectUrl = Url.Action("Index", "Photo", new { photoId = photoId })
            };

            _appDbContext.PhotoComments.Remove(comment);
            _appDbContext.Notifications.Add(notification);

            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Photo", new { photoId = photoId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeletePhotoAdmin(string profileUserId, int photoId, string? returnUrl)
        {
            var profileUser = _appDbContext
                .Users
                .FirstOrDefault(u => u.Id.Equals(profileUserId));

            var photoToRemove = _appDbContext
                .Photos
                .Include(p => p.Album)
                .ThenInclude(pa => pa.Photos)
                .FirstOrDefault(p => p.Id.Equals(photoId));

            if (photoToRemove != null)
            {
                if (photoToRemove.Album.Photos.Count == 1)
                {
                    var albumToRemove = _appDbContext.PhotoAlbums.FirstOrDefault(pa => pa.Id.Equals(photoToRemove.AlbumId));
                    
                    if (albumToRemove != null)
                    {
                        _appDbContext.PhotoAlbums.Remove(albumToRemove);
                    }
                }

                _appDbContext.Photos.Remove(photoToRemove);

                Notification notification = new Notification()
                {
                    User = profileUser,
                    Content = "One of your photos was removed!",
                    RedirectUrl = Url.Action("Index", "Profile", new {profileUserId = profileUserId})
                };

                _appDbContext.Notifications.Add(notification);

                _appDbContext.SaveChanges();
            }

            //return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });
            return Redirect(returnUrl ?? "/");
        }

        public IActionResult DeleteOwnPhoto(string profileUserId, int photoId, string? returnUrl)
        {
            var profileUser = _appDbContext
                .Users
                .FirstOrDefault(u => u.Id.Equals(profileUserId));

            var photoToRemove = _appDbContext
                .Photos
                .Include(p => p.Album)
                .ThenInclude(pa => pa.Photos)
                .FirstOrDefault(p => p.Id.Equals(photoId));

            if (photoToRemove != null)
            {
                if (photoToRemove.Album.Photos.Count == 1)
                {
                    var albumToRemove = _appDbContext.PhotoAlbums.FirstOrDefault(pa => pa.Id.Equals(photoToRemove.AlbumId));

                    if (albumToRemove != null)
                    {
                        _appDbContext.PhotoAlbums.Remove(albumToRemove);
                    }
                }

                _appDbContext.Photos.Remove(photoToRemove);
                _appDbContext.SaveChanges();
            }

            //return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });
            return Redirect(returnUrl ?? "/");
        }
    }
}
