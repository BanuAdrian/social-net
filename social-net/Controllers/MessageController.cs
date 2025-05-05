using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using social_net.Data;
using social_net.Models;

namespace social_net.Controllers
{
    public class MessageController : Controller
    {
        private readonly ILogger<MessageController> _logger;
        private readonly AppDbContext _appDbContext;

        public MessageController(ILogger<MessageController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }


        public IActionResult Index(string profileUserId)
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
            return Redirect(Url.RouteUrl(new { controller = "Message", action = "Index", profileUserId = profileUserId }) + "#" + "bottom");
        }

    }
}
