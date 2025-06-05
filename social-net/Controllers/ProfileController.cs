using Azure.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using social_net.Data;
using social_net.Models;

namespace social_net.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly AppDbContext _appDbContext;

        public ProfileController(ILogger<ProfileController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index(string profileUserId)
        {
            var profileUser = _appDbContext.Users
                .Include(u => u.ReceivedFriendRequests)
                .Include(u => u.SentFriendRequests)
                .Include(u => u.InitiatedFriendships)
                .Include(u => u.ReceivedFriendships)
                .Include(u => u.TextPosts.OrderByDescending(t => t.PostedAt))
                .Include(u => u.PhotoAlbums)
                .ThenInclude(p => p.Photos)
                .ToList()
                .Find(u => u.Id.Equals(profileUserId));
            if (profileUser != null)
            {
                profileUser.PhotoAlbums = profileUser.PhotoAlbums.OrderByDescending(pa => pa.PostedAt).ToList();
                return View(profileUser);
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddFriend(string profileUserId, string currentUserId)
        {
            var profileUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(profileUserId));
            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            if (profileUser == null || currentUser == null)
            {
                return NotFound();
            }

            var existingRequest = _appDbContext.FriendRequests.FirstOrDefault(fr => fr.SenderId == currentUserId && fr.ReceiverId == profileUserId);

            if (existingRequest == null)
            {
                var friendRequest = new FriendRequest { Sender = currentUser, Receiver = profileUser };

                _appDbContext.FriendRequests.Add(friendRequest);
                _appDbContext.SaveChanges();
            }


            return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });
        }

        [HttpPost]
        public IActionResult AcceptFriend(string profileUserId, string currentUserId)
        {
            var profileUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(profileUserId));
            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            if (profileUser == null || currentUser == null)
            {
                return NotFound();
            }

            var existingFriendship = _appDbContext
                .Friendships
                .FirstOrDefault(fr => (fr.InitiatorUserId == currentUserId && fr.RecipientUserId == profileUserId
                                || (fr.InitiatorUserId == profileUserId && fr.RecipientUserId == currentUserId)));

            if (existingFriendship == null)
            {
                var friendship = new Friendship { InitiatorUser = profileUser, RecipientUser = currentUser };

                _appDbContext.Friendships.Add(friendship);
                var friendRequestToRemove = _appDbContext.FriendRequests.FirstOrDefault(fr => fr.SenderId == profileUserId && fr.ReceiverId == currentUserId);
                _appDbContext.FriendRequests.Remove(friendRequestToRemove);

                Notification notification = new Notification()
                {
                    User = profileUser,
                    Content = currentUser.FirstName + " " + currentUser.LastName + " accepted your friend request",
                    RedirectUrl = Url.Action("Index", "Profile", new { profileUserId = currentUserId })
                };

                _appDbContext.Notifications.Add(notification);

                _appDbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });

        }


        [HttpPost]
        public IActionResult DeclineFriend(string profileUserId, string currentUserId)
        {
            var profileUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(profileUserId));
            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            if (profileUser == null || currentUser == null)
            {
                return NotFound();
            }

            var friendRequestToRemove = _appDbContext.FriendRequests.FirstOrDefault(fr => fr.SenderId == profileUserId && fr.ReceiverId == currentUserId);
            _appDbContext.FriendRequests.Remove(friendRequestToRemove);

            _appDbContext.SaveChanges();


            return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });
        }

        [HttpPost]
        public IActionResult RemoveFriend(string profileUserId, string currentUserId)
        {
            var profileUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(profileUserId));
            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            if (profileUser == null || currentUser == null)
            {
                return NotFound();
            }

            var existingFriendship = _appDbContext
                .Friendships
                .FirstOrDefault(fr => (fr.InitiatorUserId == currentUserId && fr.RecipientUserId == profileUserId
                                || (fr.InitiatorUserId == profileUserId && fr.RecipientUserId == currentUserId)));

            if (existingFriendship != null)
            {
                _appDbContext.Friendships.Remove(existingFriendship);
                _appDbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });
        }

        public IActionResult SearchProfile(string searchTerm)
        {
            var users = _appDbContext
                .Users
                .Where(u =>
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    (u.FirstName + " " + u.LastName).ToLower().Contains(searchTerm) ||
                    (u.LastName + " " + u.FirstName).ToLower().Contains(searchTerm)
                )
                .ToList();
            
            

            return View(users);
        }

        [Authorize]
        public IActionResult MessageBox(string profileUserId)
        {
            var profileUser = _appDbContext.Users
                .Include(u => u.SentMessages)
                .FirstOrDefault(u => u.Id.Equals(profileUserId));
            return View(profileUser);

        }

        [HttpPost]
        public IActionResult SendMessage(string messageContent, string currentUserId, string profileUserId)
        {
            var profileUser = _appDbContext.Users
                .Include(u => u.SentMessages)
                .FirstOrDefault(u => u.Id.Equals(profileUserId));
            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));

            var msg = new FriendsMessage { Sender = currentUser, Receiver = profileUser, Content = messageContent, SentAt = DateTime.Now };

            _appDbContext.FriendsMessages.Add(msg);
            _appDbContext.SaveChanges();

            return Redirect(Url.RouteUrl(new { controller = "Profile", action = "MessageBox", profileUserId = profileUserId }) + "#" + "bottom");
        }

        [HttpPost]
        public IActionResult CreatePost(string textContent, string currentUserId, string? returnUrl)
        {
            var currentUser = _appDbContext.Users
                .FirstOrDefault(u => u.Id.Equals(currentUserId));

            if (textContent != null)
            {
                var textPost = new TextPost { User = currentUser, Text = textContent, PostedAt = DateTime.Now };

                _appDbContext.TextPosts.Add(textPost);
                _appDbContext.SaveChanges();
            }


            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        public IActionResult CreateAlbum(List<IFormFile> photos, string currentUserId, string? returnUrl)
        {
            if (photos == null || !photos.Any())
            {
                ModelState.AddModelError("", "Please select at least one photo.");
                return RedirectToAction("Index", "Profile", new { profileUserId = currentUserId });
            }

            var currentUser = _appDbContext.Users
                .FirstOrDefault(u => u.Id.Equals(currentUserId));

            var album = new PhotoAlbum { User = currentUser, PostedAt = DateTime.Now };
            _appDbContext.PhotoAlbums.Add(album);
            _appDbContext.SaveChanges();

            var userPhotosFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/users-photos", currentUserId);
            Directory.CreateDirectory(userPhotosFolder);

            foreach (var photoFile in photos)
            {
                if (photoFile.Length > 0)
                {
                    var filePath = Path.Combine(userPhotosFolder, photoFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        photoFile.CopyTo(stream);
                    }

                    var photo = new Photo { Album = album, ImagePath = "/users-photos/" + currentUserId + "/" + photoFile.FileName };
                    _appDbContext.Photos.Add(photo);
                }
            }

            _appDbContext.SaveChanges();
            //return RedirectToAction("Index", "Profile", new { profileUserId = currentUserId });
            return Redirect(returnUrl ?? "/");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeletePostAdmin(string profileUserId, int textPostId, string? returnUrl)
        {
            var profileUser = _appDbContext.Users
                .FirstOrDefault(u => u.Id.Equals(profileUserId));

            var textPostToRemove = _appDbContext.TextPosts.FirstOrDefault(tp => tp.Id.Equals(textPostId));

            if (textPostToRemove != null)
            {
                _appDbContext.TextPosts.Remove(textPostToRemove);

                var adminRoleId = _appDbContext.Roles
                                .Where(r => r.Name == "Admin")
                                .Select(r => r.Id)
                                .FirstOrDefault();


                bool isAdmin = _appDbContext.UserRoles.Any(ur => ur.UserId == profileUserId && ur.RoleId == adminRoleId);

                if (!isAdmin)
                {
                    Notification notification = new Notification()
                    {
                        User = profileUser,
                        Content = "One of your posts was removed!",
                        RedirectUrl = Url.Action("Index", "Profile", new { profileUserId = profileUserId })
                    };

                    _appDbContext.Notifications.Add(notification);
                }

                _appDbContext.SaveChanges();
            }

            //return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });
            return Redirect(returnUrl ?? "/");

        }

        public IActionResult DeleteOwnPost(string profileUserId, int textPostId, string? returnUrl)
        {
            var profileUser = _appDbContext.Users
                .FirstOrDefault(u => u.Id.Equals(profileUserId));

            var textPostToRemove = _appDbContext.TextPosts.FirstOrDefault(tp => tp.Id.Equals(textPostId));

            if (textPostToRemove != null)
            {
                _appDbContext.TextPosts.Remove(textPostToRemove);
                _appDbContext.SaveChanges();
            }

            //return RedirectToAction("Index", "Profile", new { profileUserId = profileUserId });
            //return Redirect(returnUrl ?? "/");
            return Redirect(returnUrl ?? "/");
        }

        public IActionResult ReadNotification(int notificationId)
        {
            var notification = _appDbContext.Notifications.FirstOrDefault(n => n.Id.Equals(notificationId));

            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            _appDbContext.SaveChanges();

            return Redirect(notification.RedirectUrl);
        }

        public IActionResult DeleteNotification(int notificationId, string? returnUrl)
        {
            var notification = _appDbContext.Notifications.FirstOrDefault(n => n.Id.Equals(notificationId));

            if (notification == null)
            {
                return NotFound();
            }

            _appDbContext.Notifications.Remove(notification);
            _appDbContext.SaveChanges();

            return Redirect(returnUrl ?? "/");
        }

    }
}
