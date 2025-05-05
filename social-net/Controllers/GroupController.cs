using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using social_net.Data;
using social_net.Models;

namespace social_net.Controllers
{
    public class GroupController : Controller
    {
        private readonly ILogger<GroupController> _logger;
        private readonly AppDbContext _appDbContext;

        public GroupController(ILogger<GroupController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Index(int groupId)
        {
            var group = _appDbContext.Groups
                .Include(gr => gr.GroupMemberships)
                .ThenInclude(gm => gm.Member)
                .FirstOrDefault(gr => gr.Id.Equals(groupId));
            if (group != null)
            {
                return View(group);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateGroup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateGroup(string currentUserId, Group group)
        {
            if (ModelState.IsValid)
            {
                var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));
                var groupMembership = new GroupMembership { Group = group, Member = currentUser };

                _appDbContext.Groups.Add(group);
                _appDbContext.GroupMemberships.Add(groupMembership);
                _appDbContext.SaveChanges();


                return RedirectToAction("Index", "Group", new { groupId = group.Id});
            }
            return View();
        }

        [HttpPost]
        public IActionResult JoinGroup(string currentUserId, int groupId)
        {
            var currentUser = _appDbContext.Users.FirstOrDefault(u => u.Id.Equals(currentUserId));
            var group = _appDbContext.Groups.FirstOrDefault(gr => gr.Id.Equals(groupId));

            if (currentUser == null || group == null)
            {
                return NotFound();
            }

            var groupMembership = new GroupMembership { Group = group, Member = currentUser };
            _appDbContext.GroupMemberships.Add(groupMembership);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Group", new { groupId = group.Id });
        }
    }
}
