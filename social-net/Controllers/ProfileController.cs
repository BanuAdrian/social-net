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
                .ToList()
                .Find(u => u.Id.Equals(profileUserId));
            //Console.WriteLine("S-a intrat in functia Index(), userId = " + userId);
            //Console.WriteLine(user?.FirstName);
            Console.WriteLine("S-a primit string-ul: " + profileUserId);
            if (profileUser != null)
            {
                return View(profileUser);
            }
            return RedirectToAction("Index", "Home");
        }

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

            Console.WriteLine("User " + profileUserId + " a primit o cerere de prietenie de la " + currentUserId);

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
                .FirstOrDefault(fr => (fr.InitiatorUserId == currentUserId  && fr.RecipientUserId == profileUserId
                                || (fr.InitiatorUserId == profileUserId && fr.RecipientUserId == currentUserId)));

            if (existingFriendship == null)
            {
                var friendship = new Friendship { InitiatorUser = profileUser, RecipientUser = currentUser };

                _appDbContext.Friendships.Add(friendship);
                var friendRequestToRemove = _appDbContext.FriendRequests.FirstOrDefault(fr => fr.SenderId == profileUserId && fr.ReceiverId == currentUserId);
                _appDbContext.FriendRequests.Remove(friendRequestToRemove);

                _appDbContext.SaveChanges();
            }

            Console.WriteLine("Prietenii lui " + currentUser.Email + ":");
            foreach (var friendship in currentUser.InitiatedFriendships)
            {
                Console.WriteLine(friendship.RecipientUser.Email);
            }
            foreach (var friendship in currentUser.ReceivedFriendships)
            {
                Console.WriteLine(friendship.InitiatorUser.Email);
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

        public IActionResult SearchProfile(string searchTerm)
        {
            //var user = _appDbContext.Users.ToList().Find(u => u.Id.Equals(searchTerm));
            ////Console.WriteLine("S-a intrat in functia Index(), userId = " + userId);
            ////Console.WriteLine(user?.FirstName);
            //Console.WriteLine("S-a primit string-ul: " + userId);
            //if (user != null)
            //{
            //    return View(user);
            //}
            //return RedirectToAction("Index", "Home");
            List<User> users = _appDbContext.Users.ToList().FindAll(u => u.FirstName.ToLower().Equals(searchTerm.ToLower()));
            users.AddRange(_appDbContext.Users.ToList().FindAll(u => u.LastName.ToLower().Equals(searchTerm.ToLower())));
            return View(users);
        }

        public IActionResult MessageBox(string profileUserId)
        {
            var profileUser = _appDbContext.Users
                .Include(u => u.SentMessages)
                .FirstOrDefault(u => u.Id.Equals(profileUserId));
            //var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));
            //Console.WriteLine("Message: profileUserId = " + profileUserId);
            return View(profileUser);

            //return Redirect(Url.RouteUrl(new { controller = "Message", action = "Index", profileUserId = profileUserId }) + "#" + "bottom");
        }

        [HttpPost]
        public IActionResult SendMessage(string messageContent, string currentUserId, string profileUserId)
        {
            var profileUser = _appDbContext.Users
                .Include(u => u.SentMessages)
                .FirstOrDefault(u => u.Id.Equals(profileUserId));
            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));
            //Console.WriteLine("Message: profileUserId = " + profileUserId);
            Console.WriteLine("MESAJ: " + messageContent);

            var msg = new FriendsMessage { Sender = currentUser, Receiver = profileUser, Content = messageContent, SentAt = DateTime.UtcNow };

            _appDbContext.FriendsMessages.Add(msg);
            _appDbContext.SaveChanges();

            //return RedirectToAction("Index", "Message", new { profileUserId = profileUserId });
            //return RedirectToAction("Index", "Message", new { profileUserId = profileUserId, fragment = "bottom" });
            return Redirect(Url.RouteUrl(new { controller = "Profile", action = "MessageBox", profileUserId = profileUserId }) + "#" + "bottom");
        }

        [HttpPost]
        public IActionResult CreatePost(string textContent, string currentUserId)
        {
            var currentUser = _appDbContext.Users
                .FirstOrDefault(u => u.Id.Equals(currentUserId));

            var textPost = new TextPost { User = currentUser, Text = textContent, PostedAt = DateTime.UtcNow };

            _appDbContext.TextPosts.Add(textPost);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Profile", new { profileUserId = currentUserId });
        }

    }
}
