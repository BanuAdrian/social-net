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

            return RedirectToAction("Index", "Profile", new { userId = profileUserId });
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

            return RedirectToAction("Index", "Profile", new { userId = profileUserId });

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


            return RedirectToAction("Index", "Profile", new { userId = profileUserId });
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
    }
}
